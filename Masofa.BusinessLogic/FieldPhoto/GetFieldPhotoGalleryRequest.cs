using Masofa.Common.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.FieldPhotoRequest
{
    public class GetFieldPhotoGalleryRequest : IRequest<FieldPhotoGalleryResponse>
    {
        public Guid? RegionId { get; set; }
        public Guid? ParentRegionId { get; set; }
        public Guid? FieldId { get; set; }
        public string? FieldName { get; set; }
        public IEnumerable<Guid> TagIds { get; set; } = Enumerable.Empty<Guid>();
        public DateTime? CaptureDateFrom { get; set; }
        public DateTime? CaptureDateTo { get; set; }
        public DateTime? UploadDateFrom { get; set; }
        public DateTime? UploadDateTo { get; set; }
        public string? Search { get; set; }
        public double? North { get; set; }
        public double? South { get; set; }
        public double? East { get; set; }
        public double? West { get; set; }
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 50;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }

    public class FieldPhotoGalleryResponse
    {
        public IReadOnlyList<FieldPhotoListItem> Items { get; init; } = Array.Empty<FieldPhotoListItem>();
        public int Total { get; init; }
    }

    public class FieldPhotoListItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid? FieldId { get; set; }
        public string? FieldName { get; set; }
        public Guid? RegionId { get; set; }
        public string? RegionName { get; set; }
        public Guid? ParentRegionId { get; set; }
        public string? ParentRegionName { get; set; }
        public Guid? FileStorageId { get; set; }
        public FileContentType FileContentType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime UploadDateUtc { get; set; }
        public DateTime? CaptureDateUtc { get; set; }
        public string? Description { get; set; }
        public IReadOnlyList<TagShortDto> Tags { get; set; } = Array.Empty<TagShortDto>();
    }

    public class TagShortDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class GetFieldPhotoGalleryRequestHandler : IRequestHandler<GetFieldPhotoGalleryRequest, FieldPhotoGalleryResponse>
    {
        private const string OwnerType = "Masofa.Common.Models.CropMonitoring.FieldPhoto";

        private readonly MasofaCropMonitoringDbContext _cropMonitoringDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly ILogger<GetFieldPhotoGalleryRequestHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public GetFieldPhotoGalleryRequestHandler(
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            MasofaCommonDbContext commonDbContext,
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaIdentityDbContext identityDbContext,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<GetFieldPhotoGalleryRequestHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager)
        {
            _cropMonitoringDbContext = cropMonitoringDbContext;
            _commonDbContext = commonDbContext;
            _dictionariesDbContext = dictionariesDbContext;
            _identityDbContext = identityDbContext;
            _businessLogicLogger = businessLogicLogger;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<FieldPhotoGalleryResponse> Handle(GetFieldPhotoGalleryRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Получаем текущего пользователя из токена
                Guid? currentUserId = null;
                var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    var currentUser = await _identityDbContext.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower().Equals(userName.ToLower()), cancellationToken);
                    if (currentUser != null)
                    {
                        currentUserId = currentUser.Id;
                    }
                }

                var photosQuery = _cropMonitoringDbContext.FieldPhotos
                    .AsNoTracking()
                    .Where(p => p.Status == StatusType.Active);

                // Фильтрация по текущему пользователю - показываем только свои фото
                if (currentUserId.HasValue)
                {
                    photosQuery = photosQuery.Where(p => p.CreateUser == currentUserId.Value);
                }
                else
                {
                    // Если пользователь не определен, возвращаем пустой результат
                    photosQuery = photosQuery.Where(p => false);
                }

                if (request.RegionId.HasValue)
                {
                    photosQuery = photosQuery.Where(p => p.RegionId == request.RegionId.Value);
                }

                if (request.ParentRegionId.HasValue)
                {
                    photosQuery = photosQuery.Where(p => p.ParentRegionId == request.ParentRegionId.Value);
                }

                if (request.FieldId.HasValue)
                {
                    photosQuery = photosQuery.Where(p => p.FieldId == request.FieldId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.FieldName))
                {
                    var normalizedFieldName = request.FieldName.Trim().ToLower();
                    // Получаем ID полей, которые соответствуют поисковому запросу по названию
                    var matchingFieldIds = await _cropMonitoringDbContext.Fields
                        .AsNoTracking()
                        .Where(f => f.Status == StatusType.Active && 
                                    !string.IsNullOrEmpty(f.Name) && 
                                    f.Name.ToLower().Contains(normalizedFieldName))
                        .Select(f => f.Id)
                        .ToListAsync(cancellationToken);

                    if (matchingFieldIds.Count > 0)
                    {
                        photosQuery = photosQuery.Where(p => p.FieldId.HasValue && matchingFieldIds.Contains(p.FieldId.Value));
                    }
                    else
                    {
                        // Если нет полей с таким названием, возвращаем пустой результат
                        photosQuery = photosQuery.Where(p => false);
                    }
                }

                if (request.CaptureDateFrom.HasValue)
                {
                    photosQuery = photosQuery.Where(p => p.CaptureDateUtc >= request.CaptureDateFrom.Value);
                }

                if (request.CaptureDateTo.HasValue)
                {
                    photosQuery = photosQuery.Where(p => p.CaptureDateUtc <= request.CaptureDateTo.Value);
                }

                if (request.UploadDateFrom.HasValue)
                {
                    photosQuery = photosQuery.Where(p => p.CreateAt >= request.UploadDateFrom.Value);
                }

                if (request.UploadDateTo.HasValue)
                {
                    photosQuery = photosQuery.Where(p => p.CreateAt <= request.UploadDateTo.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var normalized = request.Search.Trim().ToLower();
                    photosQuery = photosQuery.Where(p =>
                        (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(normalized)) ||
                        (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(normalized)));
                }

                // Фильтрация по bounds (видимая область карты)
                // Используем PostGIS функции для работы с геометрией Point
                if (request.North.HasValue && request.South.HasValue && request.East.HasValue && request.West.HasValue)
                {
                    var south = request.South.Value;
                    var north = request.North.Value;
                    var west = request.West.Value;
                    var east = request.East.Value;

                    // Используем raw SQL с PostGIS функциями для фильтрации точек внутри bounds
                    var photoIdsInBounds = await _cropMonitoringDbContext.FieldPhotos
                        .FromSqlInterpolated($@"
                            SELECT * FROM ""FieldPhotos""
                            WHERE ""Status"" = {(int)StatusType.Active}
                              AND ""Point"" IS NOT NULL
                              AND ST_Within(
                                    ""Point""::geometry,
                                    ST_MakeEnvelope({west}, {south}, {east}, {north}, 4326)
                                  )")
                        .AsNoTracking()
                        .Select(p => p.Id)
                        .ToListAsync(cancellationToken);

                    if (photoIdsInBounds.Count > 0)
                    {
                        photosQuery = photosQuery.Where(p => photoIdsInBounds.Contains(p.Id));
                    }
                    else
                    {
                        // Если нет фото в bounds, возвращаем пустой результат
                        photosQuery = photosQuery.Where(p => false);
                    }
                }

                var tagIds = (request.TagIds ?? Enumerable.Empty<Guid>()).Distinct().ToList();
                if (tagIds.Count > 0)
                {
                    // Сначала получаем ID фото с нужными тегами из отдельного контекста
                    // Находим фото, которые имеют хотя бы один из указанных тегов (логика ИЛИ)
                    var tagRelationsForFilter = await _commonDbContext.TagRelations
                        .AsNoTracking()
                        .Where(tr => tr.OwnerTypeFullName == OwnerType &&
                                     tagIds.Contains(tr.TagId) &&
                                     tr.Status == StatusType.Active)
                        .ToListAsync(cancellationToken);

                    // Группируем в памяти и находим фото, которые имеют хотя бы один из указанных тегов
                    var photoIdsWithTags = tagRelationsForFilter
                        .GroupBy(tr => tr.OwnerId)
                        .Select(g => g.Key)
                        .Distinct()
                        .ToList();

                    // Затем фильтруем основной запрос по полученным ID
                    if (photoIdsWithTags.Count > 0)
                    {
                        photosQuery = photosQuery.Where(p => photoIdsWithTags.Contains(p.Id));
                    }
                    else
                    {
                        // Если нет фото с нужными тегами, возвращаем пустой результат
                        photosQuery = photosQuery.Where(p => false);
                    }
                }

                photosQuery = ApplySorting(photosQuery, request.SortBy, request.SortDescending);

                var total = await photosQuery.CountAsync(cancellationToken);

                // Если указаны bounds, получаем все данные без пагинации (пагинация на фронте)
                // Иначе применяем пагинацию на беке
                var hasBounds = request.North.HasValue && request.South.HasValue && 
                               request.East.HasValue && request.West.HasValue;
                
                var photos = hasBounds
                    ? await photosQuery.ToListAsync(cancellationToken)
                    : await photosQuery
                        .Skip(Math.Max(request.Offset, 0))
                        .Take(Math.Max(request.Limit, 1))
                        .ToListAsync(cancellationToken);

                if (photos.Count == 0)
                {
                    return new FieldPhotoGalleryResponse
                    {
                        Items = Array.Empty<FieldPhotoListItem>(),
                        Total = total
                    };
                }

                var fieldIds = photos.Where(p => p.FieldId.HasValue).Select(p => p.FieldId!.Value).Distinct().ToList();
                var regionIds = photos.Where(p => p.RegionId.HasValue).Select(p => p.RegionId!.Value).Distinct().ToList();
                var parentRegionIds = photos.Where(p => p.ParentRegionId.HasValue).Select(p => p.ParentRegionId!.Value).Distinct().ToList();
                var fileIds = photos.Where(p => p.FileStorageId != Guid.Empty).Select(p => p.FileStorageId).Distinct().ToList();
                var photoIds = photos.Select(p => p.Id).ToList();

                var fields = fieldIds.Count == 0
                    ? new Dictionary<Guid, Field>()
                    : await _cropMonitoringDbContext.Fields
                        .AsNoTracking()
                        .Where(f => fieldIds.Contains(f.Id))
                        .ToDictionaryAsync(f => f.Id, cancellationToken);

                var regions = (regionIds.Count == 0 && parentRegionIds.Count == 0)
                    ? new Dictionary<Guid, Region>()
                    : await _dictionariesDbContext.Regions
                        .AsNoTracking()
                        .Where(r => regionIds.Contains(r.Id) || parentRegionIds.Contains(r.Id))
                        .ToDictionaryAsync(r => r.Id, cancellationToken);

                var fileStorageItems = fileIds.Count == 0
                    ? new Dictionary<Guid, FileStorageItem>()
                    : await _commonDbContext.FileStorageItems
                        .AsNoTracking()
                        .Where(fi => fileIds.Contains(fi.Id))
                        .ToDictionaryAsync(fi => fi.Id, cancellationToken);

                var tagRelations = await _commonDbContext.TagRelations
                    .AsNoTracking()
                    .Where(tr => photoIds.Contains(tr.OwnerId) &&
                                 tr.OwnerTypeFullName == OwnerType &&
                                 tr.Status == StatusType.Active)
                    .ToListAsync(cancellationToken);

                var tagIdsForPhotos = tagRelations.Select(tr => tr.TagId).Distinct().ToList();
                var tagsDictionary = tagIdsForPhotos.Count == 0
                    ? new Dictionary<Guid, Tag>()
                    : await _dictionariesDbContext.Tags
                        .AsNoTracking()
                        .Where(t => tagIdsForPhotos.Contains(t.Id) && t.Status == StatusType.Active)
                        .ToDictionaryAsync(t => t.Id, cancellationToken);

                var items = photos
                    .Select(photo =>
                    {
                        fields.TryGetValue(photo.FieldId ?? Guid.Empty, out var field);
                        Region? region = null;
                        Region? parentRegion = null;

                        if (photo.RegionId.HasValue)
                        {
                            regions.TryGetValue(photo.RegionId.Value, out region);
                        }

                        if (photo.ParentRegionId.HasValue)
                        {
                            regions.TryGetValue(photo.ParentRegionId.Value, out parentRegion);
                        }
                        else if (region?.ParentId != null && regions.TryGetValue(region.ParentId.Value, out var derivedParent))
                        {
                            parentRegion = derivedParent;
                        }

                        fileStorageItems.TryGetValue(photo.FileStorageId, out var fileStorageItem);

                        var relatedTagIds = tagRelations
                            .Where(tr => tr.OwnerId == photo.Id)
                            .Select(tr => tr.TagId)
                            .Distinct()
                            .ToList();

                        var tagDtos = relatedTagIds
                            .Where(tagId => tagsDictionary.ContainsKey(tagId) && tagsDictionary[tagId].Status == StatusType.Active)
                            .Select(tagId => new TagShortDto
                            {
                                Id = tagId,
                                Name = GetTagName(tagsDictionary[tagId])
                            })
                            .OrderBy(t => t.Name)
                            .ToList();

                        return new FieldPhotoListItem
                        {
                            Id = photo.Id,
                            Title = photo.Title,
                            FieldId = photo.FieldId,
                            FieldName = field?.Name,
                            RegionId = region?.Id,
                            RegionName = ResolveRegionName(region),
                            ParentRegionId = parentRegion?.Id,
                            ParentRegionName = ResolveRegionName(parentRegion),
                            FileStorageId = photo.FileStorageId != Guid.Empty ? photo.FileStorageId : fileStorageItem?.Id,
                            FileContentType = fileStorageItem?.FileContentType ?? FileContentType.Default,
                            Latitude = photo.Latitude,
                            Longitude = photo.Longitude,
                            UploadDateUtc = photo.CreateAt,
                            CaptureDateUtc = photo.CaptureDateUtc,
                            Description = photo.Description,
                            Tags = tagDtos
                        };
                    })
                    .ToList();

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, "result"), requestPath);

                return new FieldPhotoGalleryResponse
                {
                    Items = items,
                    Total = total
                };
            }
            catch (Exception ex)
            {
                var message = $"Error in GetFieldPhotoGalleryRequest: {ex.Message}";
                await _businessLogicLogger.LogCriticalAsync(message, requestPath);
                _logger.LogCritical(ex, message);
                throw;
            }
        }

        private static IQueryable<FieldPhoto> ApplySorting(IQueryable<FieldPhoto> query, string? sortBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return descending ? query.OrderByDescending(p => p.CreateAt) : query.OrderBy(p => p.CreateAt);
            }

            return sortBy.ToLowerInvariant() switch
            {
                "title" => descending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                "capturedateutc" => descending ? query.OrderByDescending(p => p.CaptureDateUtc) : query.OrderBy(p => p.CaptureDateUtc),
                "uploaddateutc" => descending ? query.OrderByDescending(p => p.CreateAt) : query.OrderBy(p => p.CreateAt),
                _ => descending ? query.OrderByDescending(p => p.CreateAt) : query.OrderBy(p => p.CreateAt)
            };
        }

        private static string GetTagName(Tag tag) => string.IsNullOrWhiteSpace(tag.Name) ? tag.Id.ToString() : tag.Name;

        private static string? ResolveRegionName(Region? region)
        {
            if (region == null)
            {
                return null;
            }

            var priority = new[] { "ru-RU", "uz-Latn-UZ", "en-US" };
            foreach (var culture in priority)
            {
                var value = region.Names[culture];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return region.Names.ValuesJson;
        }
    }
}

