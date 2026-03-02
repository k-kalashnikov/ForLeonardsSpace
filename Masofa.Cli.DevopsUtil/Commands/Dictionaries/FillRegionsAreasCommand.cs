using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.DevopsUtil.Commands.Dictionaries
{
    [BaseCommand("Fill regions area", "Fill regions area")]
    public class FillRegionsAreasCommand : IBaseCommand
    {
        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }

        public FillRegionsAreasCommand(MasofaDictionariesDbContext dictionariesDbContext)
        {
            DictionariesDbContext = dictionariesDbContext;
        }

        public void Dispose()
        {
            Console.WriteLine("\nFillRegionsAreas END");
        }

        public async Task Execute()
        {
            Console.WriteLine("FillRegionsAreas START\n");

            var regionMaps = await DictionariesDbContext.RegionMaps.Where(rm => rm.Polygon != null)
                .ToListAsync();

            var regions = await DictionariesDbContext.Regions
                .Where(r => r.RegionMapId != null && regionMaps.Select(rm=> rm.Id).ToList().Contains(r.RegionMapId.Value))
                .ToListAsync();

            foreach (var region in regions)
            {
                var regionMap = regionMaps.FirstOrDefault(rm => rm.Id == region.RegionMapId);
                if (regionMap == null) continue;
                if (regionMap.Polygon == null) continue;

                List<NetTopologySuite.Geometries.Polygon> polygons = [];
                if (regionMap.Polygon is NetTopologySuite.Geometries.Polygon polygon)
                {
                    polygons.Add(polygon);
                }
                else if (regionMap.Polygon is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                {
                    polygons.AddRange(multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToArray());
                }

                region.RegionSquare = CalculateArea(polygons);
            }

            await DictionariesDbContext.SaveChangesAsync();
        }

        private static decimal CalculateArea(List<NetTopologySuite.Geometries.Polygon> polygons)
        {
            decimal result = 0;
            foreach (var polygon in polygons)
            {
                var c = polygon.Centroid;
                var lon = c.X;
                var lat = c.Y;

                int zone = (int)Math.Floor((lon + 180d) / 6d) + 1;
                bool north = lat >= 0;

                var csFactory = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
                var wgs84 = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
                var utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, north);

                var transformer = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory()
                    .CreateFromCoordinateSystems(wgs84, utm);

                var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), (int)utm.AuthorityCode);

                var coordinates = polygon.ExteriorRing.Coordinates
                    .Select(coord =>
                    {
                        var point = new[] { coord.X, coord.Y };
                        var transformed = transformer.MathTransform.Transform(point);
                        return new NetTopologySuite.Geometries.Coordinate(transformed[0], transformed[1]);
                    })
                    .ToArray();

                var linearRing = geometryFactory.CreateLinearRing(coordinates);
                var transformedPolygon = geometryFactory.CreatePolygon(linearRing);

                result += (decimal)(transformedPolygon.Area / 1_000_000);
            }

            return result;
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
