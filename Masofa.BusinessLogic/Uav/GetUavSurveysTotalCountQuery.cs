using Masofa.Common.Models;
using Masofa.Common.Models.Uav;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Masofa.BusinessLogic.Uav.Queries
{
    public class GetUavSurveysTotalCountQuery : IRequest<int>
    {
        public BaseGetQuery<UAVFlyPath> QueryOptions { get; set; }
    }

    public class GetUavSurveysTotalCountQueryHandler : IRequestHandler<GetUavSurveysTotalCountQuery, int>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;

        public GetUavSurveysTotalCountQueryHandler(
            MasofaUAVDbContext uavDbContext,
            MasofaCommonDbContext commonDbContext)
        {
            _uavDbContext = uavDbContext;
            _commonDbContext = commonDbContext;
        }

        public async Task<int> Handle(GetUavSurveysTotalCountQuery request, CancellationToken cancellationToken)
        {
            var queryOptions = request.QueryOptions;

            var q = _uavDbContext.Set<UAVFlyPath>()
                .AsNoTracking()
                .Where(s => s.Status != StatusType.Deleted);

            if (queryOptions?.Filters != null)
            {
                var tagFilter = queryOptions.Filters.FirstOrDefault(f => f.FilterField.Equals("TagIds", StringComparison.OrdinalIgnoreCase));
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

                        q = q.Where(s => surveyIdsWithTag.Contains(s.Id));
                    }
                }

                var relationKeys = new[] { "FieldId", "RegionId", "CropId", "FirmId" };

                foreach (var key in relationKeys)
                {
                    var filter = queryOptions.Filters.FirstOrDefault(f => f.FilterField.Equals(key, StringComparison.OrdinalIgnoreCase));

                    if (filter != null && filter.FilterValue != null && Guid.TryParse(filter.FilterValue.ToString(), out Guid id))
                    {
                        if (key == "FieldId")
                        {
                            q = q.Where(fp => _uavDbContext.Set<UAVPhotoCollection>().Any(col =>
                                col.UAVFlyPathId == fp.Id && _uavDbContext.Set<UAVPhotoCollectionRelation>()
                                .Any(rel => rel.UAVPhotoCollectionId == col.Id && rel.Status != StatusType.Deleted && rel.FieldId == id)));
                        }
                        else if (key == "RegionId")
                        {
                            q = q.Where(fp => _uavDbContext.Set<UAVPhotoCollection>().Any(col =>
                                col.UAVFlyPathId == fp.Id && _uavDbContext.Set<UAVPhotoCollectionRelation>()
                                .Any(rel => rel.UAVPhotoCollectionId == col.Id && rel.Status != StatusType.Deleted && rel.RegionId == id)));
                        }
                        else if (key == "CropId")
                        {
                            q = q.Where(fp => _uavDbContext.Set<UAVPhotoCollection>().Any(col =>
                                col.UAVFlyPathId == fp.Id && _uavDbContext.Set<UAVPhotoCollectionRelation>()
                                .Any(rel => rel.UAVPhotoCollectionId == col.Id && rel.Status != StatusType.Deleted && rel.CropId == id)));
                        }
                        else if (key == "FirmId")
                        {
                            q = q.Where(fp => _uavDbContext.Set<UAVPhotoCollection>().Any(col =>
                                col.UAVFlyPathId == fp.Id && _uavDbContext.Set<UAVPhotoCollectionRelation>()
                                .Any(rel => rel.UAVPhotoCollectionId == col.Id && rel.Status != StatusType.Deleted && rel.FirmId == id)));
                        }
                    }
                }

                var dateFilters = queryOptions.Filters.Where(f => f.FilterField.Equals("OriginalDate", StringComparison.OrdinalIgnoreCase));
                foreach (var dateFilter in dateFilters)
                {
                    if (dateFilter.FilterValue != null)
                    {
                        var filterString = dateFilter.FilterValue.ToString();
                        if (DateOnly.TryParseExact(filterString, new[] { "yyyy-MM-dd", "MM.dd.yyyy", "dd.MM.yyyy" },
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateOnly filterDate))
                        {
                            var op = dateFilter.FilterOperator;
                            q = q.Where(fp => _uavDbContext.Set<UAVPhotoCollection>()
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

                // --- Generic Filters (Excluding special ones) ---
                var ignoredFields = new[] { "TagIds", "OriginalDate", "FieldId", "RegionId", "CropId", "FirmId" };

                foreach (var filter in queryOptions.Filters.Where(f =>
                    !ignoredFields.Contains(f.FilterField, StringComparer.OrdinalIgnoreCase)))
                {
                    q = q.ApplyFiltering(filter);
                }
            }

            return await q.CountAsync(cancellationToken);
        }
    }
}