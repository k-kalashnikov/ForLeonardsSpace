using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
{
    [BaseCommand("Fill Sentinel2 Product Polygon", "Fill Sentinel2 Product Polygon")]
    public class Sentinel2ProductPolygonFillCommand : IBaseCommand
    {
        private MasofaSentinelDbContext SentinelDbContext { get; set; }
        private MasofaCommonDbContext CommonDbContext { get; set; }

        public Sentinel2ProductPolygonFillCommand(MasofaSentinelDbContext sentinelDbContext, MasofaCommonDbContext commonDbContext)
        {
            SentinelDbContext = sentinelDbContext;
            CommonDbContext = commonDbContext;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Execute()
        {
            var inspireMetadata = await SentinelDbContext.SentinelInspireMetadata
                .ToDictionaryAsync(m => m.Id, m => m);

            var sentinel2Products = await SentinelDbContext.Sentinel2Products
                .Where(p => p.SentinelInspireMetadataId != null 
                            && inspireMetadata.Keys.Contains(p.SentinelInspireMetadataId.Value))
                .ToListAsync();

            var satelliteProducts = await CommonDbContext.SatelliteProducts
                .Where(p => sentinel2Products.Select(s => s.SatellateProductId).Contains(p.Id.ToString())
                            && p.Polygon == null)
                .ToListAsync();

            foreach (var s2p in sentinel2Products)
            {
                if (s2p.SentinelInspireMetadataId == null || s2p.SatellateProductId == null)
                {
                    continue;
                }

                var im = inspireMetadata[s2p.SentinelInspireMetadataId.Value];

                if (im == null)
                {
                    continue;
                }

                var satelliteProduct = satelliteProducts
                    .FirstOrDefault(p => p.Id.ToString() == s2p.SatellateProductId);
                if (satelliteProduct == null || satelliteProduct.Polygon != null)
                {
                    continue;
                }

                satelliteProduct.Polygon = ToPolygon(im);
            }

            await CommonDbContext.SaveChangesAsync();
        }

        private static Polygon ToPolygon(SentinelInspireMetadata sim)
        {
            var west = (double)sim.WestBoundLongitude;
            var east = (double)sim.EastBoundLongitude;
            var south = (double)sim.SouthBoundLatitude;
            var north = (double)sim.NorthBoundLatitude;

            if (west >= east)
            {
                throw new ArgumentException("West must be < East.");
            }

            if (south >= north)
            {
                throw new ArgumentException("South must be < North.");
            }

            var coords = new[]
            {
                new Coordinate(west, south),
                new Coordinate(east, south),
                new Coordinate(east, north),
                new Coordinate(west, north),
                new Coordinate(west, south)
            };

            var factory = new GeometryFactory(new PrecisionModel(), 4326);
            var ring = factory.CreateLinearRing(coords);
            return factory.CreatePolygon(ring);
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
