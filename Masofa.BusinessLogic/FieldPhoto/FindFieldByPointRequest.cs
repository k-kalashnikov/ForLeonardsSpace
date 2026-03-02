using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Dictionaries;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.FieldPhotoRequest
{
    public class FindFieldByPointRequest : IRequest<FieldLocationMatchDto>
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }

    public class FieldLocationMatchDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid? FieldId { get; set; }
        public string? FieldName { get; set; }
        public Guid? RegionId { get; set; }
        public string? RegionName { get; set; }
        public Guid? ParentRegionId { get; set; }
        public string? ParentRegionName { get; set; }
    }

    public class FindFieldByPointRequestHandler : IRequestHandler<FindFieldByPointRequest, FieldLocationMatchDto>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringDbContext;
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly ILogger<FindFieldByPointRequestHandler> _logger;

        public FindFieldByPointRequestHandler(
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            MasofaDictionariesDbContext dictionariesDbContext,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<FindFieldByPointRequestHandler> logger)
        {
            _cropMonitoringDbContext = cropMonitoringDbContext;
            _dictionariesDbContext = dictionariesDbContext;
            _businessLogicLogger = businessLogicLogger;
            _logger = logger;
        }

        public async Task<FieldLocationMatchDto> Handle(FindFieldByPointRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var field = await FindFieldGeometryAsync(request.Latitude, request.Longitude, cancellationToken);
                Region? region = null;
                Region? parentRegion = null;

                if (field?.RegionId != null)
                {
                    region = await _dictionariesDbContext.Regions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.Id == field.RegionId.Value, cancellationToken);
                }

                if (region == null)
                {
                    region = await FindRegionGeometryAsync(request.Latitude, request.Longitude, cancellationToken);
                }

                if (region?.ParentId != null)
                {
                    parentRegion = await _dictionariesDbContext.Regions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.Id == region.ParentId.Value, cancellationToken);
                }

                var result = new FieldLocationMatchDto
                {
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    FieldId = field?.Id,
                    FieldName = field?.Name,
                    RegionId = region?.Id,
                    RegionName = GetLocalizedName(region),
                    ParentRegionId = parentRegion?.Id,
                    ParentRegionName = GetLocalizedName(parentRegion)
                };

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.RegionName), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var message = $"Error during FindFieldByPointRequest: {ex.Message}";
                await _businessLogicLogger.LogCriticalAsync(message, requestPath);
                _logger.LogCritical(ex, message);
                throw;
            }
        }

        private async Task<Field?> FindFieldGeometryAsync(double latitude, double longitude, CancellationToken cancellationToken)
        {
            // First try exact match
            var exactMatch = await _cropMonitoringDbContext.Fields
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM ""Fields""
                    WHERE ""Status"" = {(int)StatusType.Active}
                      AND ""Polygon"" IS NOT NULL
                      AND ST_Contains(
                            ST_SetSRID(""Polygon""::geometry, 4326),
                            ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326))
                    ORDER BY COALESCE(""FieldArea"", 1000000000)
                    LIMIT 1")
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (exactMatch != null)
            {
                return exactMatch;
            }

            // If not found, try with 1 km tolerance (approximately 0.009 degrees at equator)
            // Using ST_DWithin with buffer for better accuracy
            const double toleranceDegrees = 0.009; // ~1 km in degrees

            return await _cropMonitoringDbContext.Fields
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM ""Fields""
                    WHERE ""Status"" = {(int)StatusType.Active}
                      AND ""Polygon"" IS NOT NULL
                      AND ST_DWithin(
                            ST_SetSRID(""Polygon""::geometry, 4326),
                            ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326),
                            {toleranceDegrees})
                    ORDER BY 
                        ST_Distance(
                            ST_SetSRID(""Polygon""::geometry, 4326),
                            ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326)
                        ) ASC,
                        COALESCE(""FieldArea"", 1000000000)
                    LIMIT 1")
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        private Task<Region?> FindRegionGeometryAsync(double latitude, double longitude, CancellationToken cancellationToken)
        {
            return _dictionariesDbContext.Regions
                .FromSqlInterpolated($@"
                    SELECT r.*
                    FROM ""Regions"" r
                    INNER JOIN ""RegionMaps"" rm ON rm.""Id"" = r.""RegionMapId""
                    WHERE rm.""Status"" = {(int)StatusType.Active}
                      AND rm.""Polygon"" IS NOT NULL
                      AND ST_Contains(
                            ST_SetSRID(rm.""Polygon""::geometry, 4326),
                            ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326))
                    ORDER BY COALESCE(r.""Level"", 0)
                    LIMIT 1")
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        private static string? GetLocalizedName(Region? region)
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

