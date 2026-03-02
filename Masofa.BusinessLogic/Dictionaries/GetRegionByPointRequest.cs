using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Dictionaries
{
    public class GetRegionByPointRequest : IRequest<Masofa.Common.Models.Dictionaries.Region?>
    {
        /// <summary>
        /// Широта
        /// </summary>
        [Required]
        public required double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [Required]
        public required double Longitude { get; set; }
    }

    public class GetRegionsByPointRequestHandler : IRequestHandler<GetRegionByPointRequest, Masofa.Common.Models.Dictionaries.Region?>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<GetRegulatoryDocumentationCustomRequestHandler> Logger { get; set; }

        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }

        public GetRegionsByPointRequestHandler(IBusinessLogicLogger businessLogicLogger, ILogger<GetRegulatoryDocumentationCustomRequestHandler> logger, MasofaDictionariesDbContext dictionariesDbContext)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            DictionariesDbContext = dictionariesDbContext;
        }

        public async Task<Masofa.Common.Models.Dictionaries.Region?> Handle(GetRegionByPointRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var point4326 = new NetTopologySuite.Geometries.Point(request.Longitude, request.Latitude) { SRID = 4326 };
                var point0 = new NetTopologySuite.Geometries.Point(request.Longitude, request.Latitude) { SRID = 0 };

                var regionMapsIds = await DictionariesDbContext.RegionMaps
                    .AsNoTracking()
                    .Where(rm => rm.Status == Masofa.Common.Models.StatusType.Active
                              && rm.Polygon != null
                              && (
                                  (rm.Polygon.SRID == 4362 && rm.Polygon.Covers(point4326)) ||
                                  (rm.Polygon.SRID == 0 && rm.Polygon.Covers(point0))
                              ))
                    .Select(rm => rm.Id)
                    .ToListAsync(cancellationToken);

                if (regionMapsIds.Count == 0)
                {
                    return null;
                }

                var regions = await DictionariesDbContext.Regions
                    .AsNoTracking()
                    .Where(r => r.Status == Masofa.Common.Models.StatusType.Active
                             && r.RegionMapId != null
                             && r.Level != null
                             && regionMapsIds.Contains(r.RegionMapId.Value))
                    .OrderByDescending(r => r.Level)
                    .ToListAsync(cancellationToken);
                if (regions.Count == 0)
                {
                    return null;
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(regions[0])), requestPath);

                return regions[0];
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}
