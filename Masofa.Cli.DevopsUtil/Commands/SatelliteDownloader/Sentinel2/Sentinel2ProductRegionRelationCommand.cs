using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
{
    [BaseCommand("Satellite: Add ProductRegion relation", "Добавление связи продукта и районов")]
    public class Sentinel2ProductRegionRelationCommand : IBaseCommand, IDisposable
    {
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaSentinelDbContext MasofaSentinelDbContext { get; set; }
        public Sentinel2ProductRegionRelationCommand(MasofaDictionariesDbContext masofaDictionariesDbContext, MasofaCommonDbContext masofaCommonDbContext, MasofaSentinelDbContext masofaSentinelDbContext)
        {
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            MasofaCommonDbContext = masofaCommonDbContext;
            MasofaSentinelDbContext = masofaSentinelDbContext;
        }

        public void Dispose()
        {
            
        }

        public async Task Execute()
        {
            Console.WriteLine("Start fetch Satellites and Regions");

            var inspireMetadata = await MasofaSentinelDbContext.SentinelInspireMetadata
                .ToDictionaryAsync(m => m.Id, m => m);

            var sentinel2Products = await MasofaSentinelDbContext.Sentinel2Products
                .ToListAsync();

            Console.WriteLine($"S2 products found: {sentinel2Products.Count}");

            var productIds = sentinel2Products
                .Select(s => s.SatellateProductId)
                .Distinct()
                .ToList();

            var satelliteProducts = await MasofaCommonDbContext.SatelliteProducts
                .Where(p => productIds.Contains(p.Id.ToString()) && p.Polygon != null)
                .ToListAsync();

            Console.WriteLine($"SatelliteProducts found: {satelliteProducts.Count}");

            var regions = await MasofaDictionariesDbContext.Regions
                    .Where(r => r.Level == 3)
                    .ToListAsync();

            Console.WriteLine($"Regions found: {regions.Count}");

            var regionMaps = await MasofaDictionariesDbContext.RegionMaps
                .Where(r => r.Polygon != null)
                .ToListAsync();

            Console.WriteLine($"regionMaps found: {regionMaps.Count}");
            Console.WriteLine("-----------------------------------------------");

            var relations = new List<SatelliteRegionRelation?>();
            var processed = 0;
            var createdRelations = 0;

            int total = sentinel2Products.Count;

            foreach (var s2p in sentinel2Products)
            {
                processed++;

                if (s2p.SentinelInspireMetadataId == null || s2p.SatellateProductId == null)
                {
                    continue;
                }

                var satelliteProduct = satelliteProducts
                    .FirstOrDefault(p => p.Id.ToString() == s2p.SatellateProductId);
                if (satelliteProduct == null || satelliteProduct.Polygon == null)
                {
                    continue;
                }
                var im = inspireMetadata[s2p.SentinelInspireMetadataId.Value];

                if (im == null)
                {
                    continue;
                }

                var poly = satelliteProduct.Polygon;
                poly.SRID = 4326;

                var regionMapIds = regionMaps
                    .Where(r => r.Polygon.Intersects(poly))
                    .Select(r => r.Id)
                    .ToList();

                //double westBound = (double)im.WestBoundLongitude;
                //double eastBound = (double)im.EastBoundLongitude;
                //double southBound = (double)im.SouthBoundLatitude;
                //double northBound = (double)im.NorthBoundLatitude;

                //var envelope = new Envelope(westBound, eastBound, southBound, northBound);
                //var geometryFactory = new GeometryFactory();
                //var geometry = geometryFactory.ToGeometry(envelope);

                //var regionMapIds = regionMaps
                //    .Where(r => r.Polygon.Intersects(geometry))
                //    .Select(r => r.Id)
                //    .ToList();

                foreach (var regionMapId in regionMapIds)
                {
                    var region = regions.Find(r => r.RegionMapId == regionMapId);
                    if (region == null) { continue; }

                    var exists = relations
                        .Any(r => r.SatelliteProductId == satelliteProduct.Id && r.RegionId == region.Id);

                    if (!exists)
                    {
                        relations.Add(new SatelliteRegionRelation()
                        {
                            RegionId = region.Id,
                            SatelliteProductId = satelliteProduct.Id
                        });

                        createdRelations++;
                    }
                }

                if (processed % 50 == 0)
                {
                    double percent = processed / (double)total * 100;
                    Console.WriteLine($"Progress: {processed}/{total} ({percent:F1}%), Relations created: {createdRelations}");
                }
            }

            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine($"Saving relations to DB: total = {createdRelations}");

            try
            {
                await MasofaCommonDbContext.SatelliteRegionRelations.AddRangeAsync(relations);
                await MasofaCommonDbContext.SaveChangesAsync();
                Console.WriteLine("Saved successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.InnerException);
            }
        }

        public Task Execute(string[] args)
        {
            return Execute();
        }
    }
}
