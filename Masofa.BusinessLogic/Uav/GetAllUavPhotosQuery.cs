using Masofa.Common.Models;
using Masofa.Common.Models.Uav;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Uav
{
    public class UAVPhotoCollectionDto : UAVPhotoCollection
    {
        public List<UAVPhoto> UavPhotos { get; set; } = new();
    }

    public class UavRepresentativePhotoDto
    {
        public Guid? PhotoId { get; set; }
        public Guid? PreviewFileStorageId { get; set; }
        public DateOnly? OriginalDate { get; set; }
        public bool HasPhoto { get; set; }
        public Guid? RegionId { get; set; }
    }

    public class UAVFlyPathDto : UAVFlyPath
    {
        public List<UAVPhotoCollectionDto> UavPhotoCollections { get; set; } = new();
        public UavRepresentativePhotoDto RepresentativePhoto { get; set; }
        public string TagNames { get; set; }
        public List<Guid> TagIds { get; set; } = new();
    }

    public class GetAllUavSurveysQuery : IRequest<List<UAVFlyPathDto>>
    {
        public BaseGetQuery<UAVFlyPath> QueryOptions { get; set; }
    }

    public class GetAllUavSurveysQueryHandler : IRequestHandler<GetAllUavSurveysQuery, List<UAVFlyPathDto>>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;

        public GetAllUavSurveysQueryHandler(
            MasofaUAVDbContext uavDbContext,
            MasofaCommonDbContext commonDbContext,
            MasofaDictionariesDbContext dictionariesDbContext)
        {
            _uavDbContext = uavDbContext;
            _commonDbContext = commonDbContext;
            _dictionariesDbContext = dictionariesDbContext;
        }

        public async Task<List<UAVFlyPathDto>> Handle(GetAllUavSurveysQuery request, CancellationToken cancellationToken)
        {
            var options = request.QueryOptions;
            var query = _uavDbContext.Set<UAVFlyPath>()
                .AsNoTracking()
                .Where(s => s.Status != StatusType.Deleted)
                .AsQueryable();

            if (options?.Filters != null)
            {
                var tagFilter = options.Filters.FirstOrDefault(f => f.FilterField.Equals("TagIds", StringComparison.OrdinalIgnoreCase));
                if (tagFilter != null && tagFilter.FilterValue != null)
                {
                    if (Guid.TryParse(tagFilter.FilterValue.ToString(), out Guid tagId))
                    {
                        var surveyIdsWithTag = await _commonDbContext.TagRelations
                            .AsNoTracking()
                            .Where(t => t.TagId == tagId
                                     && t.OwnerTypeFullName == typeof(UAVFlyPath).FullName
                                     && t.Status == StatusType.Active)
                            .Select(t => t.OwnerId)
                            .ToListAsync(cancellationToken);
                        query = query.Where(s => surveyIdsWithTag.Contains(s.Id));
                    }
                }

                var relationKeys = new[] { "FieldId", "RegionId", "CropId", "FirmId" };
                foreach (var key in relationKeys)
                {
                    var filter = options.Filters.FirstOrDefault(f => f.FilterField.Equals(key, StringComparison.OrdinalIgnoreCase));
                    if (filter != null && filter.FilterValue != null && Guid.TryParse(filter.FilterValue.ToString(), out Guid id))
                    {
                        if (key == "FieldId")
                        {
                            query = query.Where(fp => _uavDbContext.Set<UAVPhotoCollection>().Any(col =>
                                col.UAVFlyPathId == fp.Id && _uavDbContext.Set<UAVPhotoCollectionRelation>()
                                .Any(rel => rel.UAVPhotoCollectionId == col.Id && rel.Status != StatusType.Deleted && rel.FieldId == id)));
                        }
                        else if (key == "RegionId")
                        {
                            query = query.Where(fp => _uavDbContext.Set<UAVPhotoCollection>().Any(col =>
                                col.UAVFlyPathId == fp.Id && _uavDbContext.Set<UAVPhotoCollectionRelation>()
                                .Any(rel => rel.UAVPhotoCollectionId == col.Id && rel.Status != StatusType.Deleted && rel.RegionId == id)));
                        }
                        else if (key == "CropId")
                        {
                            query = query.Where(fp => _uavDbContext.Set<UAVPhotoCollection>().Any(col =>
                                col.UAVFlyPathId == fp.Id && _uavDbContext.Set<UAVPhotoCollectionRelation>()
                                .Any(rel => rel.UAVPhotoCollectionId == col.Id && rel.Status != StatusType.Deleted && rel.CropId == id)));
                        }
                        else if (key == "FirmId")
                        {
                            query = query.Where(fp => _uavDbContext.Set<UAVPhotoCollection>().Any(col =>
                                col.UAVFlyPathId == fp.Id && _uavDbContext.Set<UAVPhotoCollectionRelation>()
                                .Any(rel => rel.UAVPhotoCollectionId == col.Id && rel.Status != StatusType.Deleted && rel.FirmId == id)));
                        }
                    }
                }

                var dateFilters = options.Filters.Where(f => f.FilterField.Equals("OriginalDate", StringComparison.OrdinalIgnoreCase));
                foreach (var dateFilter in dateFilters)
                {
                    if (dateFilter.FilterValue != null)
                    {
                        var filterString = dateFilter.FilterValue.ToString();
                        if (DateOnly.TryParseExact(filterString, new[] { "yyyy-MM-dd", "MM.dd.yyyy", "dd.MM.yyyy" },
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateOnly filterDate))
                        {
                            var op = dateFilter.FilterOperator;
                            query = query.Where(fp => _uavDbContext.Set<UAVPhotoCollection>()
                                .Any(col => col.UAVFlyPathId == fp.Id &&
                                            _uavDbContext.Set<UAVPhoto>()
                                                 .Any(p => p.UAVPhotoCollectionId == col.Id &&
                                                          p.Status != StatusType.Deleted &&
                                                          (
                                                            (op == FilterOperator.Equals && p.OriginalDate == filterDate) ||
                                                            (op == FilterOperator.GreaterThanOrEqual && p.OriginalDate >= filterDate) ||
                                                            (op == FilterOperator.LessThanOrEqual && p.OriginalDate <= filterDate) ||
                                                            (op == FilterOperator.GreaterThan && p.OriginalDate > filterDate) ||
                                                            (op == FilterOperator.LessThan && p.OriginalDate < filterDate) ||
                                                            (op == 0 && p.OriginalDate == filterDate)
                                                          )
                                            )));
                        }
                    }
                }

                var ignoredFields = new[] { "TagIds", "OriginalDate", "FieldId", "RegionId", "CropId", "FirmId" };
                foreach (var filter in options.Filters.Where(f =>
                    !ignoredFields.Contains(f.FilterField, StringComparer.OrdinalIgnoreCase)))
                {
                    query = query.ApplyFiltering(filter);
                }
            }

            if (!string.IsNullOrEmpty(options?.SortBy))
            {
                query = query.ApplyOrdering(options.SortBy, options.Sort);
            }
            else
            {
                query = query.OrderByDescending(x => x.CreateAt);
            }

            if (options != null && options.Take.HasValue)
            {
                query = query.Skip(options.Offset).Take(options.Take.Value);
            }

            var flyPaths = await query.ToListAsync(cancellationToken);
            if (!flyPaths.Any())
            {
                return new List<UAVFlyPathDto>();
            }

            var flyPathIds = flyPaths.Select(x => x.Id).ToList();

            var collections = await _uavDbContext.Set<UAVPhotoCollection>()
                .AsNoTracking()
                .Where(c => flyPathIds.Contains(c.UAVFlyPathId) && c.Status != StatusType.Deleted)
                .ToListAsync(cancellationToken);
            var collectionIds = collections.Select(c => c.Id).ToList();

            var photos = await _uavDbContext.Set<UAVPhoto>()
                .AsNoTracking()
                .Where(p => collectionIds.Contains(p.UAVPhotoCollectionId) && p.Status != StatusType.Deleted)
                .ToListAsync(cancellationToken);

            var relations = await _uavDbContext.Set<UAVPhotoCollectionRelation>()
                .AsNoTracking()
                .Where(r => collectionIds.Contains(r.UAVPhotoCollectionId) && r.Status != StatusType.Deleted)
                .ToListAsync(cancellationToken);

            var flyPathType = typeof(UAVFlyPath).FullName;
            var tagRelations = await _commonDbContext.TagRelations
                .AsNoTracking()
                .Where(t => flyPathIds.Contains(t.OwnerId) && t.OwnerTypeFullName == flyPathType && t.Status == StatusType.Active)
                .ToListAsync(cancellationToken);

            var allTagIds = tagRelations.Select(t => t.TagId).Distinct().ToList();

            var tagsMap = new Dictionary<Guid, string>();
            if (allTagIds.Any())
            {
                var tags = await _dictionariesDbContext.Tags
                    .AsNoTracking()
                    .Where(t => allTagIds.Contains(t.Id))
                    .Select(t => new { t.Id, t.Name })
                    .ToListAsync(cancellationToken);

                foreach (var t in tags)
                {
                    tagsMap[t.Id] = t.Name;
                }
            }

            var result = flyPaths.Select(fp =>
            {
                var mappedCollections = collections
                    .Where(c => c.UAVFlyPathId == fp.Id)
                    .Select(c => new UAVPhotoCollectionDto
                    {
                        Id = c.Id,
                        UAVFlyPathId = c.UAVFlyPathId,
                        Point = c.Point,
                        AnalysisOnly = c.AnalysisOnly,
                        PreviewFileStorageId = c.PreviewFileStorageId,
                        UavPhotos = photos
                            .Where(p => p.UAVPhotoCollectionId == c.Id)
                            .OrderBy(p => p.OriginalDate)
                            .ThenBy(p => p.Title)
                            .ToList()
                    }).ToList();

                UavRepresentativePhotoDto repPhotoDto = new UavRepresentativePhotoDto { HasPhoto = false };

                var representativePhotoInfo = mappedCollections
                     .SelectMany(c => c.UavPhotos.Select(p => new
                     {
                         Photo = p,
                         CollectionPreviewId = c.PreviewFileStorageId,
                         CollectionId = c.Id
                     }))
                     .OrderBy(x => x.Photo.OriginalDate)
                     .ThenBy(x => x.Photo.Title)
                     .FirstOrDefault();

                if (representativePhotoInfo != null)
                {
                    var regionId = relations
                        .Where(r => r.UAVPhotoCollectionId == representativePhotoInfo.CollectionId)
                        .Select(r => r.RegionId)
                        .FirstOrDefault();

                    repPhotoDto = new UavRepresentativePhotoDto
                    {
                        HasPhoto = true,
                        PhotoId = representativePhotoInfo.Photo.Id,
                        OriginalDate = representativePhotoInfo.Photo.OriginalDate,
                        PreviewFileStorageId = representativePhotoInfo.CollectionPreviewId,
                        RegionId = regionId
                    };
                }

                var currentTagIds = tagRelations
                    .Where(tr => tr.OwnerId == fp.Id)
                    .Select(tr => tr.TagId)
                    .ToList();

                var tagNamesList = currentTagIds
                    .Select(id => tagsMap.ContainsKey(id) ? tagsMap[id] : null)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                var dto = new UAVFlyPathDto
                {
                    Id = fp.Id,
                    Status = fp.Status,
                    Comment = fp.Comment,
                    CameraTypeId = fp.CameraTypeId,
                    DataTypeId = fp.DataTypeId,
                    FlyPath = fp.FlyPath,
                    ProcessingStatus = fp.ProcessingStatus,
                    UavPhotoCollections = mappedCollections
                        .OrderBy(c => c.UavPhotos.FirstOrDefault()?.OriginalDate)
                        .ThenBy(c => c.UavPhotos.FirstOrDefault()?.Title)
                        .ToList(),
                    RepresentativePhoto = repPhotoDto,
                    TagIds = currentTagIds,
                    TagNames = string.Join(", ", tagNamesList)
                };

                return dto;
            }).ToList();

            return result;
        }
    }
}