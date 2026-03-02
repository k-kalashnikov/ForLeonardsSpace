using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Masofa.BusinessLogic.Dictionaries
{
    public class AgroClimaticZoneCreateEventHandler : INotificationHandler<BaseCreateEvent<AgroclimaticZone, MasofaDictionariesDbContext>>
    {
        private ILogger<AgroClimaticZoneCreateEventHandler> Logger { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaUgmDbContext MasofaUgmDbContext { get; set; }
        private MasofaIBMWeatherDbContext MasofaIBMWeatherDbContext { get; set; }
        private MasofaEraDbContext MasofaEraDbContext { get; set; }

        public AgroClimaticZoneCreateEventHandler(ILogger<AgroClimaticZoneCreateEventHandler> logger, MasofaDictionariesDbContext masofaDictionariesDbContext, MasofaUgmDbContext masofaUgmDbContext, MasofaIBMWeatherDbContext masofaIBMWeatherDbContext, MasofaEraDbContext masofaEraDbContext)
        {
            Logger = logger;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            MasofaUgmDbContext = masofaUgmDbContext;
            MasofaIBMWeatherDbContext = masofaIBMWeatherDbContext;
            MasofaEraDbContext = masofaEraDbContext;
        }

        public async Task Handle(BaseCreateEvent<AgroclimaticZone, MasofaDictionariesDbContext> notification, CancellationToken cancellationToken)
        {
            try
            {
                if (notification?.Model == null)
                {
                    Logger.LogWarning("Событие создания агроклиматической зоны пришло без модели");
                    return;
                }

                if (notification.Model.Polygon == null)
                {
                    Logger.LogWarning(
                        "Агроклиматическая зона {AgroclimaticZoneId} создана без полигона", notification.Model.Id);
                    return;
                }

                Logger.LogInformation("Обработка события создания агроклиматической зоны: {AgroclimaticZoneId}", notification.Model.Id);

                var poly = notification.Model.Polygon;
                poly.SRID = 4326;
                
                var regions = await MasofaDictionariesDbContext.Regions
                    .Where(r => r.RegionMapId != null)
                    .ToListAsync(cancellationToken);

                var regionMaps = await MasofaDictionariesDbContext.RegionMaps
                    .Where(r => r.Polygon != null)
                    .Where(rm => rm.Polygon!.Intersects(poly))
                    .Where(r => r.Status == Masofa.Common.Models.StatusType.Active)
                    .ToListAsync(cancellationToken);

                var regionMapIds = regionMaps
                    .Select(r => r.Id)
                    .ToList();

                var ugmStations = await MasofaUgmDbContext.UgmWeatherStations
                    .Where(u => u.AgroclimaticZoneId == null)
                    .ToListAsync(cancellationToken);

                var eraStations = await MasofaEraDbContext.EraWeatherStations
                    .Where(e => e.AgroclimaticZoneId == null)
                    .ToListAsync(cancellationToken);

                var ibmStations = await MasofaIBMWeatherDbContext.IBMMeteoStations
                    .Where(e => e.AgroclimaticZoneId == null)
                    .ToListAsync(cancellationToken);

                var existingRegionIds = await MasofaDictionariesDbContext.AgroclimaticZoneRegionRelations
                    .Where(r => r.AgroclimaticZoneId == notification.Model.Id)
                    .Select(r => r.RegionId)
                    .ToHashSetAsync(cancellationToken);

                var newRelations = new List<AgroclimaticZoneRegion>();

                try
                {
                    foreach (var ugmStation in ugmStations)
                    {
                        Coordinate coord = new Coordinate((double)ugmStation.Longitude, (double)ugmStation.Latitude);
                        var ugmPoint = new Point(coord);
                        ugmPoint.SRID = 4326;

                        if (!notification.Model.Polygon.Covers(ugmPoint))
                        {
                            continue;
                        }

                        ugmStation.AgroclimaticZoneId = notification.Model.Id;
                    }

                    await MasofaUgmDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in relation UgmStation with AgroClimaticZone.AgroClimaticZoneId={Id}", notification.Model.Id);
                }

                try
                {
                    foreach (var eraStation in eraStations)
                    {
                        var point = eraStation.Point;
                        point.SRID = 4326;

                        if (!notification.Model.Polygon.Covers(point))
                        {
                            continue;
                        }

                        eraStation.AgroclimaticZoneId = notification.Model.Id;
                    }

                    await MasofaEraDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in relation EraStation with AgroClimaticZone.AgroClimaticZoneId={Id}", notification.Model.Id);
                }

                try
                {
                    foreach (var ibmStation in ibmStations)
                    {
                        var point = ibmStation.Point;
                        point.SRID = 4326;

                        if (!notification.Model.Polygon.Covers(point))
                        {
                            continue;
                        }

                        ibmStation.AgroclimaticZoneId = notification.Model.Id;
                    }

                    await MasofaIBMWeatherDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in relation IbmStation with AgroClimaticZone.AgroClimaticZoneId={Id}", notification.Model.Id);
                }

                foreach (var regionMapId in regionMapIds)
                {
                    var region = regions.Find(r => r.RegionMapId == regionMapId);
                    if (region == null) 
                    {
                        continue;
                    }

                    if (existingRegionIds.Contains(region.Id)) 
                    {
                        continue;
                    }

                    newRelations.Add(new AgroclimaticZoneRegion
                    {
                        RegionId = region.Id,
                        AgroclimaticZoneId = notification.Model.Id
                    });

                    existingRegionIds.Add(region.Id);
                }

                try
                {
                    await MasofaDictionariesDbContext.AgroclimaticZoneRegionRelations.AddRangeAsync(newRelations, cancellationToken);
                    await MasofaDictionariesDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Ошибка при сохранении в базу данных связь между агроклиматической зоной и регионом: {AgroclimaticZoneId}", notification.Model.Id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при обработке события создания агроклиматической зоны: {AgroclimaticZoneId}", notification.Model.Id);
                throw;
            }
        }
    }
}
