using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Models.Weather;
using Masofa.DataAccess;
using MediatR;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Satellite
{
    /// <summary>
    /// Запрос для получения спутниковых продуктов с расширенной фильтрацией
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class CustomGetSatelliteProductsRequest : IRequest<CustomGetSatelliteProductsResponse>
    {
        /// <summary>
        /// Количество записей для возврата (null = без лимита, для загрузки всех продуктов в видимой области)
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// Смещение для пагинации
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Поле для сортировки
        /// </summary>
        public string? SortBy { get; set; } = "CreateAt";

        /// <summary>
        /// Направление сортировки (-1 = desc, 1 = asc)
        /// </summary>
        public int Sort { get; set; } = -1;

        /// <summary>
        /// ID региона для фильтрации (level 3 - район)
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// ID родительского региона для фильтрации (level 2 - область)
        /// </summary>
        public Guid? ParentRegionId { get; set; }

        /// <summary>
        /// Тип спутника
        /// </summary>
        public ProductSourceType? SatelliteType { get; set; }

        /// <summary>
        /// Дата для фильтрации (конкретный день по OriginDate)
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Начало периода (включительно)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Конец периода (включительно)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Фильтр по тегам (ID тегов)
        /// </summary>
        public List<Guid>? TagIds { get; set; }

        /// <summary>
        /// Северная граница видимой области карты (широта, градусы)
        /// </summary>
        public decimal? North { get; set; }

        /// <summary>
        /// Южная граница видимой области карты (широта, градусы)
        /// </summary>
        public decimal? South { get; set; }

        /// <summary>
        /// Восточная граница видимой области карты (долгота, градусы)
        /// </summary>
        public decimal? East { get; set; }

        /// <summary>
        /// Западная граница видимой области карты (долгота, градусы)
        /// </summary>
        public decimal? West { get; set; }
    }

    /// <summary>
    /// Ответ с продуктами и метаданными
    /// </summary>
    public class CustomGetSatelliteProductsResponse
    {
        /// <summary>
        /// Список продуктов
        /// </summary>
        public List<SatelliteProductDto> Data { get; set; } = new();

        /// <summary>
        /// Общее количество записей (для пагинации)
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// DTO для продукта с дополнительными полями
    /// </summary>
    public class SatelliteProductDto
    {
        public Guid Id { get; set; }
        public string? ProductId { get; set; }
        public string? Name { get; set; }
        public string? FileName { get; set; }
        public string? Mission { get; set; }
        public string? Instrument { get; set; }
        public DateTime? OriginDate { get; set; }
        public string? SensingTime { get; set; }
        public long? SizeMb { get; set; }
        public Guid? PreviewImagePath { get; set; }
        public Guid? SentinelInspireMetadataId { get; set; }
        
        /// <summary>
        /// Путь к медиа-данным в FileStorage (для скачивания)
        /// </summary>
        public Guid? MediadataPath { get; set; }
        
        /// <summary>
        /// Доступные индексы (теги) для продукта
        /// </summary>
        public List<TagDto> Tags { get; set; } = new();
        
        /// <summary>
        /// Координаты границ (для центрирования карты)
        /// </summary>
        public BoundingBox? Bounds { get; set; }
        
        /// <summary>
        /// Центр продукта (для центрирования карты)
        /// </summary>
        public Point? Center { get; set; }
        
        /// <summary>
        /// Полигон продукта в формате WKT (для отображения на карте)
        /// </summary>
        public string? PolygonJson { get; set; }
        
        /// <summary>
        /// ID регионов уровня 2, которые пересекаются с полигоном продукта
        /// </summary>
        public List<Guid> RegionLevel2Ids { get; set; } = new();
    }

    /// <summary>
    /// Границы региона/продукта
    /// </summary>
    public class BoundingBox
    {
        public decimal West { get; set; }
        public decimal East { get; set; }
        public decimal South { get; set; }
        public decimal North { get; set; }
        
        /// <summary>
        /// Центр границ (для центрирования карты)
        /// </summary>
        public Point Center => new Point
        {
            Latitude = (North + South) / 2,
            Longitude = (East + West) / 2
        };
    }

    /// <summary>
    /// Точка на карте
    /// </summary>
    public class Point
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    /// <summary>
    /// Handler для CustomGetSatelliteProductsRequest
    /// </summary>
    public class CustomGetSatelliteProductsRequestHandler : IRequestHandler<CustomGetSatelliteProductsRequest, CustomGetSatelliteProductsResponse>
    {
        private readonly MasofaCommonDbContext _dbCommonContext;
        private readonly MasofaSentinelDbContext _dbSentinelContext;
        private readonly MasofaDictionariesDbContext _dbDictContext;
        private readonly ILogger<CustomGetSatelliteProductsRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public CustomGetSatelliteProductsRequestHandler(
            MasofaCommonDbContext dbCommonContext,
            MasofaSentinelDbContext dbSentinelContext,
            MasofaDictionariesDbContext dbDictContext,
            ILogger<CustomGetSatelliteProductsRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dbCommonContext = dbCommonContext;
            _dbSentinelContext = dbSentinelContext;
            _dbDictContext = dbDictContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<CustomGetSatelliteProductsResponse> Handle(CustomGetSatelliteProductsRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Базовый запрос для SatelliteProduct
                IQueryable<SatelliteProduct> query = _dbCommonContext.Set<SatelliteProduct>()
                    .AsNoTracking()
                    .Where(p => p.Status == StatusType.Active);

                // Фильтр по типу спутника
                if (request.SatelliteType.HasValue)
                {
                    query = query.Where(p => p.ProductSourceType == request.SatelliteType.Value);
                }
                else

                // Фильтр по дате/периоду:
                // ВАЖНО: для Sentinel2 "дата снимка" хранится в Inspire metadata (SentinelInspireMetadata.DateStamp).
                // Поэтому для Sentinel2 фильтруем по DateStamp (Sentinel DB) и присваиваем её в OriginDate (в DTO),
                // а для остальных источников используем SatelliteProduct.OriginDate (Common DB).
                if (request.StartDate.HasValue || request.EndDate.HasValue || request.Date.HasValue)
                {
                    DateOnly? startDay = null;
                    DateOnly? endDay = null; // inclusive day

                    if (request.Date.HasValue)
                    {
                        startDay = DateOnly.FromDateTime(request.Date.Value);
                        endDay = startDay;
                    }
                    else
                    {
                        if (request.StartDate.HasValue)
                            startDay = DateOnly.FromDateTime(request.StartDate.Value);
                        if (request.EndDate.HasValue)
                            endDay = DateOnly.FromDateTime(request.EndDate.Value);
                    }

                    // Фильтр по дате: используем только OriginDate из SatelliteProduct (Common DB)
                    // Метаданные Inspire получаем только для отображения, не для фильтрации
                    if (startDay.HasValue || endDay.HasValue || request.Date.HasValue)
                    {
                        DateTime? startDateUtc = startDay.HasValue 
                            ? new DateTime(startDay.Value.Year, startDay.Value.Month, startDay.Value.Day, 0, 0, 0, DateTimeKind.Utc) 
                            : null;
                        DateTime? endDateUtc = endDay.HasValue 
                            ? new DateTime(endDay.Value.Year, endDay.Value.Month, endDay.Value.Day, 23, 59, 59, DateTimeKind.Utc) 
                            : null;

                        if (startDay.HasValue && endDay.HasValue)
                        {
                            query = query.Where(p => p.OriginDate.HasValue && 
                                p.OriginDate.Value.Date >= startDateUtc.Value.Date && 
                                p.OriginDate.Value.Date <= endDateUtc.Value.Date);
                        }
                        else if (startDay.HasValue)
                        {
                            query = query.Where(p => p.OriginDate.HasValue && 
                                p.OriginDate.Value.Date >= startDateUtc.Value.Date);
                        }
                        else if (endDay.HasValue)
                        {
                            query = query.Where(p => p.OriginDate.HasValue && 
                                p.OriginDate.Value.Date <= endDateUtc.Value.Date);
                        }
                        else if (request.Date.HasValue)
                        {
                            var dateUtc = new DateTime(request.Date.Value.Year, request.Date.Value.Month, request.Date.Value.Day, 0, 0, 0, DateTimeKind.Utc);
                            query = query.Where(p => p.OriginDate.HasValue && 
                                p.OriginDate.Value.Date == dateUtc.Date);
                        }
                    }
                }

                // IMPORTANT (per requirement): фильтр по видимой области карты (bounding box) отключён.
                // Список должен зависеть от фильтров (дата/тип/теги/регион), а не от viewport.

                // Фильтр по тегам
                if (request.TagIds != null && request.TagIds.Any())
                {
                    var productIdsWithTags = await _dbCommonContext.Set<TagRelation>()
                        .AsNoTracking()
                        .Where(tr => request.TagIds.Contains(tr.TagId) && 
                                    tr.OwnerTypeFullName == typeof(SatelliteProduct).FullName)
                        .Select(tr => tr.OwnerId)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    if (productIdsWithTags.Any())
                    {
                        query = query.Where(p => productIdsWithTags.Contains(p.Id));
                    }
                    else
                    {
                        // Нет продуктов с указанными тегами - возвращаем пустой результат
                        query = query.Where(p => false);
                    }
                }

                // Фильтр по региону через RegionMap polygon
                // IMPORTANT: по требованию — применяем его ПОСЛЕ остальных фильтров (тип/даты/теги),
                // чтобы пересечение считалось на уже отфильтрованном наборе.
                // Поддерживаем как RegionId (level 3 - район), так и ParentRegionId (level 2 - область)
                if (request.RegionId.HasValue || request.ParentRegionId.HasValue)
                {
                    List<Guid> regionIdsToFilter = new List<Guid>();

                    if (request.RegionId.HasValue)
                    {
                        regionIdsToFilter.Add(request.RegionId.Value);
                    }

                    if (request.ParentRegionId.HasValue)
                    {
                        var childRegions = await _dbDictContext.Set<Masofa.Common.Models.Dictionaries.Region>()
                            .AsNoTracking()
                            .Where(r => r.ParentId == request.ParentRegionId.Value && r.Status == StatusType.Active)
                            .Select(r => r.Id)
                            .ToListAsync(cancellationToken);

                        regionIdsToFilter.AddRange(childRegions);
                    }

                    if (!regionIdsToFilter.Any())
                    {
                        query = query.Where(_ => false);
                    }
                    else
                    {
                        var regions = await _dbDictContext.Set<Masofa.Common.Models.Dictionaries.Region>()
                            .AsNoTracking()
                            .Where(r => regionIdsToFilter.Contains(r.Id) && r.Status == StatusType.Active)
                            .ToListAsync(cancellationToken);

                        // Для level=3 RegionMapId может быть null — поднимаемся к родителю
                        var regionMapIds = new HashSet<Guid>();
                        foreach (var region in regions)
                        {
                            Guid? mapId = region.RegionMapId;
                            Guid? parentId = region.ParentId;

                            var safety = 0;
                            while (!mapId.HasValue && parentId.HasValue && safety++ < 5)
                            {
                                var parent = await _dbDictContext.Set<Masofa.Common.Models.Dictionaries.Region>()
                                    .AsNoTracking()
                                    .Where(r => r.Id == parentId.Value && r.Status == StatusType.Active)
                                    .Select(r => new { r.ParentId, r.RegionMapId })
                                    .FirstOrDefaultAsync(cancellationToken);

                                if (parent == null) break;

                                mapId = parent.RegionMapId;
                                parentId = parent.ParentId;
                            }

                            if (mapId.HasValue)
                            {
                                regionMapIds.Add(mapId.Value);
                            }
                        }

                        if (!regionMapIds.Any())
                        {
                            query = query.Where(_ => false);
                        }
                        else
                        {
                            var regionMaps = await _dbDictContext.Set<RegionMap>()
                                .AsNoTracking()
                                .Where(rm => regionMapIds.Contains(rm.Id) && rm.Status == StatusType.Active && rm.Polygon != null)
                                .ToListAsync(cancellationToken);

                            if (!regionMaps.Any())
                            {
                                query = query.Where(_ => false);
                            }
                            else
                            {
                                // Собираем продукты, которые пересекаются ХОТЬ КРАЕШКОМ с выбранным регионом
                                // IMPORTANT: используем CommonDbContext (а не SentinelDbContext), т.к. продукты лежат в Common DB
                                var allProductIds = new HashSet<Guid>();

                                foreach (var regionMap in regionMaps)
                                {
                                    try
                                    {
                                        var regionPolygonWkt = regionMap.Polygon!.AsText();

                                        var productIdsInRegion = await _dbCommonContext.Set<SatelliteProduct>()
                                            .FromSqlRaw(@"
                                                SELECT DISTINCT sp.* FROM ""SatelliteProducts"" sp
                                                WHERE sp.""Polygon"" IS NOT NULL 
                                                  AND ST_Intersects(
                                                      ST_Transform(sp.""Polygon"", 4326),
                                                      ST_GeomFromText({0}, 4326)
                                                  )", regionPolygonWkt)
                                            .Select(sp => sp.Id)
                                            .ToListAsync(cancellationToken);

                                        foreach (var id in productIdsInRegion)
                                        {
                                            allProductIds.Add(id);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning(ex, $"Error using PostGIS for region filtering for region map {regionMap.Id}, skipping");
                                    }
                                }

                                if (allProductIds.Any())
                                {
                                    query = query.Where(p => allProductIds.Contains(p.Id));
                                }
                                else
                                {
                                    query = query.Where(_ => false);
                                }
                            }
                        }
                    }
                }

                // Получаем общее количество до применения пагинации
                var totalCount = await query.CountAsync(cancellationToken);

                // Применяем сортировку
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = request.Sort == -1
                        ? query.OrderByDescending(p => EF.Property<object>(p, request.SortBy))
                        : query.OrderBy(p => EF.Property<object>(p, request.SortBy));
                }

                // Применяем пагинацию
                if (request.Take.HasValue)
                {
                    query = query.Skip(request.Offset).Take(request.Take.Value);
                }

                var products = await query.ToListAsync(cancellationToken);

                // Для SizeMb: заранее подтянем длины файлов по MediadataPath (Zip) из FileStorageItems
                var mediadataIds = products
                    .Where(p => p.MediadataPath != Guid.Empty)
                    .Select(p => p.MediadataPath)
                    .Distinct()
                    .ToList();

                var mediaFileLengths = await _dbCommonContext.Set<FileStorageItem>()
                    .AsNoTracking()
                    .Where(f => mediadataIds.Contains(f.Id))
                    .Select(f => new { f.Id, f.FileLength })
                    .ToListAsync(cancellationToken);

                var mediaFileLengthById = mediaFileLengths.ToDictionary(x => x.Id, x => x.FileLength);

                // Получаем метаданные Sentinel2 и дополнительную информацию
                var productIds = products.Select(p => p.ProductId).ToList();
                var sentinel2Metadata = await _dbSentinelContext.Set<Sentinel2ProductMetadata>()
                    .AsNoTracking()
                    .Where(m => productIds.Contains(m.ProductId))
                    .ToListAsync(cancellationToken);

                // Получаем Inspire метаданные для дополнительной информации (если нужно)
                var sentinelProducts = await _dbSentinelContext.Set<Sentinel2ProductEntity>()
                    .AsNoTracking()
                    .Where(sp => productIds.Contains(sp.SatellateProductId))
                    .ToListAsync(cancellationToken);

                var inspireMetadataIds = sentinelProducts
                    .Where(sp => sp.SentinelInspireMetadataId.HasValue)
                    .Select(sp => sp.SentinelInspireMetadataId!.Value)
                    .ToList();

                var inspireMetadata = await _dbSentinelContext.Set<SentinelInspireMetadata>()
                    .AsNoTracking()
                    .Where(im => inspireMetadataIds.Contains(im.Id))
                    .ToListAsync(cancellationToken);

                // Получаем теги для продуктов
                var productGuids = products.Select(p => p.Id).ToList();
                var tagRelations = await _dbCommonContext.Set<TagRelation>()
                    .AsNoTracking()
                    .Where(tr => productGuids.Contains(tr.OwnerId) && 
                                tr.OwnerTypeFullName == typeof(SatelliteProduct).FullName)
                    .ToListAsync(cancellationToken);

                var tagIds = tagRelations.Select(tr => tr.TagId).Distinct().ToList();
                var tags = await _dbDictContext.Set<Tag>()
                    .AsNoTracking()
                    .Where(t => tagIds.Contains(t.Id) && t.Status == StatusType.Active)
                    .ToListAsync(cancellationToken);

                // Получаем регионы уровня 2 для пересечений с полигонами продуктов
                var regionLevel2Ids = new List<Guid>();
                var productRegionIntersections = new Dictionary<Guid, List<Guid>>();
                
                foreach (var product in products)
                {
                    if (!string.IsNullOrEmpty(product.PolygonJson))
                    {
                        try
                        {
                            // IMPORTANT:
                            // Этот запрос выполняется в Dictionaries DB (_dbDictContext), поэтому нельзя ссылаться на таблицу SatelliteProducts.
                            // Полигон продукта передаём как WKT (product.PolygonJson возвращает poly.AsText()) и делаем ST_Intersects.
                            var polygonWkt = product.PolygonJson.Trim();

                            var intersectingRegions = await _dbDictContext.Database.SqlQueryRaw<Guid>(
                                    @"SELECT r.""Id""
                                      FROM ""Regions"" r
                                      INNER JOIN ""RegionMaps"" rm ON r.""RegionMapId"" = rm.""Id""
                                      WHERE r.""Level"" = 2
                                        AND r.""Status"" = 1
                                        AND rm.""Polygon"" IS NOT NULL
                                        AND ST_Intersects(
                                          ST_Transform(CASE WHEN ST_SRID(rm.""Polygon"") = 0 THEN ST_SetSRID(rm.""Polygon"", 4326) ELSE rm.""Polygon"" END, 4326),
                                          ST_SetSRID(ST_GeomFromText({0}, 4326), 4326)
                                        )",
                                    polygonWkt)
                                .ToListAsync(cancellationToken);
                            
                            productRegionIntersections[product.Id] = intersectingRegions;
                            regionLevel2Ids.AddRange(intersectingRegions);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Error finding region intersections for product {product.Id}: {product.PolygonJson}");
                            productRegionIntersections[product.Id] = new List<Guid>();
                        }
                    }
                    else
                    {
                        productRegionIntersections[product.Id] = new List<Guid>();
                    }
                }

                // Формируем DTO
                var result = products.Select(p =>
                {
                    var metadata = sentinel2Metadata.FirstOrDefault(m => m.ProductId == p.ProductId);
                    var sentinelProd = sentinelProducts.FirstOrDefault(sp => sp.SatellateProductId == p.ProductId);
                    var inspire = sentinelProd != null && sentinelProd.SentinelInspireMetadataId.HasValue
                        ? inspireMetadata.FirstOrDefault(im => im.Id == sentinelProd.SentinelInspireMetadataId.Value)
                        : null;

                    // Получаем теги для данного продукта
                    var productTagRelations = tagRelations.Where(tr => tr.OwnerId == p.Id).ToList();
                    var productTags = productTagRelations
                        .Join(tags, tr => tr.TagId, t => t.Id, (tr, t) => new TagDto { Id = t.Id, Name = t.Name, Description = t.Description })
                        .ToList();

                    // Используем PolygonJson для получения bounds и center
                    BoundingBox? bounds = null;
                    Point? center = null;
                    
                    if (!string.IsNullOrEmpty(p.PolygonJson))
                    {
                        try
                        {
                            var polygonText = p.PolygonJson.Trim();

                            Geometry? geom = null;
                            if (polygonText.StartsWith("{"))
                            {
                                // GeoJSON
                                geom = new GeoJsonReader().Read<Geometry>(polygonText);
                            }
                            else
                            {
                                // WKT
                                geom = new WKTReader().Read(polygonText);
                            }

                            if (geom != null && !geom.IsEmpty)
                            {
                                var envelope = geom.EnvelopeInternal;
                                bounds = new BoundingBox
                                {
                                    West = (decimal)envelope.MinX,
                                    East = (decimal)envelope.MaxX,
                                    South = (decimal)envelope.MinY,
                                    North = (decimal)envelope.MaxY
                                };
                                
                                center = new Point
                                {
                                    Latitude = (decimal)(envelope.MinY + envelope.MaxY) / 2,
                                    Longitude = (decimal)(envelope.MinX + envelope.MaxX) / 2
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Error parsing polygon for product {p.Id}: {p.PolygonJson}");
                        }
                    }

                    return new SatelliteProductDto
                    {
                        Id = p.Id,
                        ProductId = p.ProductId,
                        Name = metadata?.Name ?? p.ProductId,
                        FileName = metadata?.Name,
                        Mission = p.ProductSourceType switch
                        {
                            ProductSourceType.Sentinel2 => "SENTINEL-2",
                            ProductSourceType.Landsat => "LANDSAT",
                            _ => "UNKNOWN"
                        },
                        Instrument = p.ProductSourceType switch
                        {
                            ProductSourceType.Sentinel2 => "MSI",
                            ProductSourceType.Landsat => "OLI_TIRS",
                            _ => "UNKNOWN"
                        },
                        // Дата снимка: для Sentinel2 берём DateStamp из Inspire metadata и кладём в OriginDate
                        OriginDate = inspire?.DateStamp ?? p.OriginDate,
                        SensingTime = (inspire?.DateStamp ?? p.OriginDate)?.ToString("yyyy-MM-dd HH:mm:ss"),
                        // SizeMb:
                        // 1) если есть ContentLength из Sentinel2ProductMetadata — используем его
                        // 2) иначе — fallback на FileStorageItem.FileLength по MediadataPath (zip)
                        SizeMb =
                            metadata?.ContentLength.HasValue == true
                                ? metadata.ContentLength.Value / (1024 * 1024)
                                : (mediaFileLengthById.TryGetValue(p.MediadataPath, out var fl) && fl.HasValue
                                    ? fl.Value / (1024 * 1024)
                                    : (long?)null),
                        PreviewImagePath = p.PreviewImagePath,
                        SentinelInspireMetadataId = sentinelProd?.SentinelInspireMetadataId,
                        MediadataPath = p.MediadataPath,
                        Tags = productTags,
                        Bounds = bounds,
                        Center = center,
                        PolygonJson = p.PolygonJson, // Добавляем PolygonJson для фронтенда
                        RegionLevel2Ids = productRegionIntersections.ContainsKey(p.Id) ? productRegionIntersections[p.Id] : new List<Guid>()
                    };
                }).ToList();

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

                return new CustomGetSatelliteProductsResponse
                {
                    Data = result,
                    Total = totalCount
                };
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

    }
}

