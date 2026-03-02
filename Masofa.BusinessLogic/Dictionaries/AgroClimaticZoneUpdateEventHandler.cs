using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Dictionaries
{
    public class AgroClimaticZoneUpdateEventHandler : INotificationHandler<BaseUpdateEvent<AgroclimaticZone, MasofaDictionariesDbContext>>
    {
        private ILogger<AgroClimaticZoneUpdateEventHandler> Logger { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaUgmDbContext MasofaUgmDbContext { get; set; }
        private MasofaIBMWeatherDbContext MasofaIBMWeatherDbContext { get; set; }
        private MasofaEraDbContext MasofaEraDbContext { get; set; }
        public AgroClimaticZoneUpdateEventHandler(ILogger<AgroClimaticZoneUpdateEventHandler> logger, MasofaDictionariesDbContext masofaDictionariesDbContext, MasofaUgmDbContext masofaUgmDbContext, MasofaIBMWeatherDbContext masofaIBMWeatherDbContext, MasofaEraDbContext masofaEraDbContext)
        {
            Logger = logger;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            MasofaUgmDbContext = masofaUgmDbContext;
            MasofaIBMWeatherDbContext = masofaIBMWeatherDbContext;
            MasofaEraDbContext = masofaEraDbContext;
        }

        public async Task Handle(BaseUpdateEvent<AgroclimaticZone, MasofaDictionariesDbContext> notification, CancellationToken cancellationToken)
        {
            try
            {
                if (notification?.CurrentModel == null)
                {
                    Logger.LogWarning("Событие обновления агроклиматической зоны пришло без CurrentModel");
                    return;
                }

                if (notification.CurrentModel.Polygon == null)
                {
                    Logger.LogWarning("Агроклиматическая зона {AgroclimaticZoneId} обновлена без полигона", notification.CurrentModel.Id);
                    return;
                }

                var zoneId = notification.CurrentModel.Id;

                Logger.LogInformation("Обработка события обновления агроклиматической зоны: {AgroclimaticZoneId}", zoneId);

                var poly = notification.CurrentModel.Polygon;
                poly.SRID = 4326;

                var regions = await MasofaDictionariesDbContext.Regions
                    .Where(r => r.RegionMapId != null)
                    .ToListAsync(cancellationToken);

                var regionMapIds = await MasofaDictionariesDbContext.RegionMaps
                    .Where(rm => rm.Polygon != null && rm.Status == StatusType.Active)
                    .Where(rm => rm.Polygon!.Intersects(poly))
                    .Select(rm => rm.Id)
                    .ToListAsync(cancellationToken);

                var regionByMapId = regions
                    .GroupBy(r => r.RegionMapId!.Value)
                    .ToDictionary(g => g.Key, g => g.First());

                var targetRegionIds = new HashSet<Guid>();
                foreach (var mapId in regionMapIds)
                {
                    if (regionByMapId.TryGetValue(mapId, out var region))
                    {
                        targetRegionIds.Add(region.Id);
                    }
                }

                var existingRelations = await MasofaDictionariesDbContext.AgroclimaticZoneRegionRelations
                    .Where(x => x.AgroclimaticZoneId == zoneId)
                    .ToListAsync(cancellationToken);

                var existingRegionIds = existingRelations
                    .Select(x => x.RegionId)
                    .ToHashSet();

                var toAdd = new List<AgroclimaticZoneRegion>();
                foreach (var regionId in targetRegionIds)
                {
                    if (existingRegionIds.Contains(regionId))
                        continue;

                    toAdd.Add(new AgroclimaticZoneRegion
                    {
                        AgroclimaticZoneId = zoneId,
                        RegionId = regionId
                    });
                }

                var toRemove = existingRelations
                    .Where(x => !targetRegionIds.Contains(x.RegionId))
                    .ToList();

                if (toRemove.Count > 0)
                {
                    MasofaDictionariesDbContext.AgroclimaticZoneRegionRelations.RemoveRange(toRemove);
                }

                if (toAdd.Count > 0)
                {
                    await MasofaDictionariesDbContext.AgroclimaticZoneRegionRelations.AddRangeAsync(toAdd, cancellationToken);
                }

                if (toAdd.Count > 0 || toRemove.Count > 0)
                {
                    await MasofaDictionariesDbContext.SaveChangesAsync(cancellationToken);
                }

                var ugmStations = await MasofaUgmDbContext.UgmWeatherStations
                    .Where(s => s.AgroclimaticZoneId == null || s.AgroclimaticZoneId == zoneId)
                    .ToListAsync(cancellationToken);

                foreach (var s in ugmStations)
                {
                    var p = new Point(new Coordinate((double)s.Longitude, (double)s.Latitude)) { SRID = 4326 };
                    var inside = poly.Covers(p);

                    if (inside)    
                    { 
                        s.AgroclimaticZoneId = zoneId; 
                    }
                    else if (s.AgroclimaticZoneId == zoneId)
                    { 
                        s.AgroclimaticZoneId = null; 
                    }
                }

                await MasofaUgmDbContext.SaveChangesAsync(cancellationToken);

                var eraStations = await MasofaEraDbContext.EraWeatherStations
                    .Where(s => s.AgroclimaticZoneId == null || s.AgroclimaticZoneId == zoneId)
                    .ToListAsync(cancellationToken);

                foreach (var s in eraStations)
                {
                    var p = s.Point;
                    p.SRID = 4326;

                    var inside = poly.Covers(p);

                    if (inside)
                    {
                        s.AgroclimaticZoneId = zoneId;
                    }
                    else if (s.AgroclimaticZoneId == zoneId)
                    {
                        s.AgroclimaticZoneId = null;
                    }
                }

                await MasofaEraDbContext.SaveChangesAsync(cancellationToken);

                var ibmStations = await MasofaIBMWeatherDbContext.IBMMeteoStations
                    .Where(s => s.AgroclimaticZoneId == null || s.AgroclimaticZoneId == zoneId)
                    .ToListAsync(cancellationToken);

                foreach (var s in ibmStations)
                {
                    var p = s.Point;
                    p.SRID = 4326;

                    var inside = poly.Covers(p);

                    if (inside)
                    {
                        s.AgroclimaticZoneId = zoneId;
                    }
                    else if (s.AgroclimaticZoneId == zoneId)
                    {
                        s.AgroclimaticZoneId = null;
                    }
                }

                await MasofaIBMWeatherDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при обработке события обновления агроклиматической зоны: {AgroclimaticZoneId}", notification?.CurrentModel?.Id);
                throw;
            }
        }
    }
}
