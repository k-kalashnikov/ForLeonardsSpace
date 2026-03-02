using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Satellite
{
    [RequestPermission(ActionType = "Read")]
    public class GetSatelliteProductsWithIndicesRequest : IRequest<GetSatelliteProductsWithIndicesResponse>
    {
        public DateTime? Date { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ProductSourceType? SatelliteType { get; set; }
        public Guid? RegionId { get; set; }
        public int? Take { get; set; }
        public int Offset { get; set; } = 0;
    }

    public class GetSatelliteProductsWithIndicesResponse
    {
        public List<SatelliteProductWithIndicesDto> Data { get; set; } = new();
        public int Total { get; set; }
    }

    public class SatelliteProductWithIndicesDto
    {
        public Guid Id { get; set; }
        public Guid? PreviewImagePath { get; set; }
        public DateTime? OriginDate { get; set; }
        public string? Mission { get; set; }
        public Point? Center { get; set; }
        public SatelliteProductIndicesDto Indices { get; set; } = new();
        public long? SizeMb { get; set; }
        public Guid? MediadataPath { get; set; }
        public List<Guid> RegionIds { get; set; } = new();
    }

    public class SatelliteProductIndicesDto
    {
        public SatelliteIndexLayerDto? Arvi { get; set; }
        public SatelliteIndexLayerDto? Evi { get; set; }
        public SatelliteIndexLayerDto? Gndvi { get; set; }
        public SatelliteIndexLayerDto? Mndwi { get; set; }
        public SatelliteIndexLayerDto? Ndmi { get; set; }
        public SatelliteIndexLayerDto? Ndvi { get; set; }
        public SatelliteIndexLayerDto? Orvi { get; set; }
        public SatelliteIndexLayerDto? Osavi { get; set; }
    }

    public class SatelliteIndexLayerDto
    {
        public Guid? PreviewImagePath { get; set; }
        public Guid? FileStorageItemId { get; set; }
    }

    public class GetSatelliteProductsWithIndicesHandler : IRequestHandler<GetSatelliteProductsWithIndicesRequest, GetSatelliteProductsWithIndicesResponse>
    {
        private readonly MasofaCommonDbContext _dbCommonContext;
        private readonly MasofaIndicesDbContext _dbIndicesContext;
        private readonly MasofaSentinelDbContext _dbSentinelContext;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly ILogger<GetSatelliteProductsWithIndicesHandler> _logger;

        public GetSatelliteProductsWithIndicesHandler(
            MasofaCommonDbContext dbCommonContext,
            MasofaIndicesDbContext dbIndicesContext,
            MasofaSentinelDbContext dbSentinelContext,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<GetSatelliteProductsWithIndicesHandler> logger)
        {
            _dbCommonContext = dbCommonContext;
            _dbIndicesContext = dbIndicesContext;
            _dbSentinelContext = dbSentinelContext;
            _businessLogicLogger = businessLogicLogger;
            _logger = logger;
        }

        public async Task<GetSatelliteProductsWithIndicesResponse> Handle(GetSatelliteProductsWithIndicesRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                IQueryable<SatelliteProduct> query = _dbCommonContext.Set<SatelliteProduct>()
                    .AsNoTracking()
                    .Where(p => p.Status == StatusType.Active && p.OriginDate != null);

                if (request.SatelliteType.HasValue)
                {
                    query = query.Where(p => p.ProductSourceType == request.SatelliteType.Value);
                }

                if (request.RegionId.HasValue)
                {
                    var allowedProductIds = _dbCommonContext.Set<SatelliteRegionRelation>()
                        .Where(r => r.RegionId == request.RegionId.Value)
                        .Select(r => r.SatelliteProductId);

                    query = query.Where(p => allowedProductIds.Contains(p.Id));
                }

                if (request.StartDate.HasValue || request.EndDate.HasValue)
                {
                    var start = request.StartDate?.Date.ToUniversalTime();
                    var end = request.EndDate?.Date.ToUniversalTime();

                    if (start.HasValue && end.HasValue)
                        query = query.Where(p => p.OriginDate.HasValue && p.OriginDate.Value >= start.Value && p.OriginDate.Value <= end.Value);
                    else if (start.HasValue)
                        query = query.Where(p => p.OriginDate.HasValue && p.OriginDate.Value >= start.Value);
                    else if (end.HasValue)
                        query = query.Where(p => p.OriginDate.HasValue && p.OriginDate.Value <= end.Value);
                }
                else if (request.Date.HasValue)
                {
                    var dateOnly = DateTime.SpecifyKind(request.Date.Value.Date, DateTimeKind.Utc);
                    var nextDay = dateOnly.AddDays(1);
                    query = query.Where(p => p.OriginDate.HasValue && p.OriginDate.Value >= dateOnly && p.OriginDate.Value < nextDay);
                }

                var totalCount = await query.CountAsync(cancellationToken);

                query = query.OrderByDescending(p => p.OriginDate);
                if (request.Take.HasValue)
                {
                    query = query.Skip(request.Offset).Take(request.Take.Value);
                }

                var products = await query.Select(p => new
                {
                    p.Id,
                    p.ProductId,
                    p.MediadataPath,
                    p.PreviewImagePath,
                    p.OriginDate,
                    p.ProductSourceType,
                    p.Polygon
                }).ToListAsync(cancellationToken);

                var productIds = products.Select(p => p.Id).ToList();
                var productIdsAsStrings = products.Select(p => p.Id.ToString()).ToList();

                var sentinelMetadata = await _dbSentinelContext.Set<Sentinel2ProductMetadata>()
                    .AsNoTracking()
                    .Where(m => productIdsAsStrings.Contains(m.ProductId))
                    .Select(m => new { m.ProductId, m.ContentLength })
                    .ToListAsync(cancellationToken);

                var regionRelations = new Dictionary<Guid, List<Guid>>();
                if (productIds.Any())
                {
                    var relations = await _dbCommonContext.Set<SatelliteRegionRelation>()
                        .AsNoTracking()
                        .Where(r => productIds.Contains(r.SatelliteProductId))
                        .ToListAsync(cancellationToken);

                    regionRelations = relations
                        .GroupBy(r => r.SatelliteProductId)
                        .ToDictionary(g => g.Key, g => g.Select(r => r.RegionId).ToList());
                }

                var indicesData = new Dictionary<Guid, SatelliteProductIndicesDto>();
                foreach (var pid in productIds)
                {
                    indicesData[pid] = new SatelliteProductIndicesDto();
                }

                if (productIds.Any())
                {
                    SatelliteIndexLayerDto MapLayer(Guid? preview, Guid? fsId) =>
                        new SatelliteIndexLayerDto { PreviewImagePath = preview, FileStorageItemId = fsId };

                    var arviList = await _dbIndicesContext.ArviPolygons.AsNoTracking().Where(x => productIds.Contains(x.SatelliteProductId)).Select(x => new { x.SatelliteProductId, x.PreviewImagePath, x.FileStorageItemId }).ToListAsync(cancellationToken);
                    var eviList = await _dbIndicesContext.EviPolygons.AsNoTracking().Where(x => productIds.Contains(x.SatelliteProductId)).Select(x => new { x.SatelliteProductId, x.PreviewImagePath, x.FileStorageItemId }).ToListAsync(cancellationToken);
                    var gndviList = await _dbIndicesContext.GndviPolygons.AsNoTracking().Where(x => productIds.Contains(x.SatelliteProductId)).Select(x => new { x.SatelliteProductId, x.PreviewImagePath, x.FileStorageItemId }).ToListAsync(cancellationToken);
                    var mndwiList = await _dbIndicesContext.MndwiPolygons.AsNoTracking().Where(x => productIds.Contains(x.SatelliteProductId)).Select(x => new { x.SatelliteProductId, x.PreviewImagePath, x.FileStorageItemId }).ToListAsync(cancellationToken);
                    var ndmiList = await _dbIndicesContext.NdmiPolygons.AsNoTracking().Where(x => productIds.Contains(x.SatelliteProductId)).Select(x => new { x.SatelliteProductId, x.PreviewImagePath, x.FileStorageItemId }).ToListAsync(cancellationToken);
                    var ndviList = await _dbIndicesContext.NdviPolygons.AsNoTracking().Where(x => productIds.Contains(x.SatelliteProductId)).Select(x => new { x.SatelliteProductId, x.PreviewImagePath, x.FileStorageItemId }).ToListAsync(cancellationToken);
                    var orviList = await _dbIndicesContext.OrviPolygons.AsNoTracking().Where(x => productIds.Contains(x.SatelliteProductId)).Select(x => new { x.SatelliteProductId, x.PreviewImagePath, x.FileStorageItemId }).ToListAsync(cancellationToken);
                    var osaviList = await _dbIndicesContext.OsaviPolygons.AsNoTracking().Where(x => productIds.Contains(x.SatelliteProductId)).Select(x => new { x.SatelliteProductId, x.PreviewImagePath, x.FileStorageItemId }).ToListAsync(cancellationToken);

                    foreach (var item in arviList) indicesData[item.SatelliteProductId].Arvi = MapLayer(item.PreviewImagePath, item.FileStorageItemId);
                    foreach (var item in eviList) indicesData[item.SatelliteProductId].Evi = MapLayer(item.PreviewImagePath, item.FileStorageItemId);
                    foreach (var item in gndviList) indicesData[item.SatelliteProductId].Gndvi = MapLayer(item.PreviewImagePath, item.FileStorageItemId);
                    foreach (var item in mndwiList) indicesData[item.SatelliteProductId].Mndwi = MapLayer(item.PreviewImagePath, item.FileStorageItemId);
                    foreach (var item in ndmiList) indicesData[item.SatelliteProductId].Ndmi = MapLayer(item.PreviewImagePath, item.FileStorageItemId);
                    foreach (var item in ndviList) indicesData[item.SatelliteProductId].Ndvi = MapLayer(item.PreviewImagePath, item.FileStorageItemId);
                    foreach (var item in orviList) indicesData[item.SatelliteProductId].Orvi = MapLayer(item.PreviewImagePath, item.FileStorageItemId);
                    foreach (var item in osaviList) indicesData[item.SatelliteProductId].Osavi = MapLayer(item.PreviewImagePath, item.FileStorageItemId);
                }

                var result = products.Select(p =>
                {
                    Point? center = null;
                    if (p.Polygon != null && !p.Polygon.IsEmpty)
                    {
                        try
                        {
                            var envelope = p.Polygon.EnvelopeInternal;
                            center = new Point
                            {
                                Latitude = (decimal)(envelope.MinY + envelope.MaxY) / 2,
                                Longitude = (decimal)(envelope.MinX + envelope.MaxX) / 2
                            };
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Ошибка расчета центра для продукта {p.Id}: {ex.Message}");
                        }
                    }

                    var indices = indicesData.ContainsKey(p.Id) ? indicesData[p.Id] : new SatelliteProductIndicesDto();
                    var metadata = sentinelMetadata.FirstOrDefault(m => m.ProductId == p.Id.ToString());
                    var regions = regionRelations.ContainsKey(p.Id) ? regionRelations[p.Id] : new List<Guid>();

                    return new SatelliteProductWithIndicesDto
                    {
                        Id = p.Id,
                        PreviewImagePath = p.PreviewImagePath,
                        OriginDate = p.OriginDate,
                        Center = center,
                        Mission = p.ProductSourceType switch
                        {
                            ProductSourceType.Sentinel2 => "SENTINEL-2",
                            ProductSourceType.Landsat => "LANDSAT",
                            _ => "UNKNOWN"
                        },
                        Indices = indices,
                        MediadataPath = p.MediadataPath,
                        SizeMb = (metadata != null && metadata.ContentLength.HasValue)
                                 ? metadata.ContentLength.Value / (1024 * 1024)
                                 : null,
                        RegionIds = regions
                    };
                }).ToList();

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

                return new GetSatelliteProductsWithIndicesResponse
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