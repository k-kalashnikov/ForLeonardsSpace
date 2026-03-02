using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Masofa.Cli.DevopsUtil.Commands.Indices
{
    [BaseCommand("Create Random Demo Anomaly Polygons", "Create Random Demo Anomaly Polygons")]
    public class CreateRandomDemoAnomalyPolygons : IBaseCommand
    {
        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private MasofaIndicesDbContext IndicesDbContext { get; set; }

        private static readonly Random Random = new();

        private readonly Dictionary<AnomalyType, string> _colors = new()
        {
            [AnomalyType.Flooding] = "#1f78b4",
            [AnomalyType.Overwatering] = "#a6cee3",
            [AnomalyType.Drought] = "#e31a1c",
            [AnomalyType.SkippingOfSeedlings] = "#ff7f00",
            [AnomalyType.PrematureAging] = "#b2df8a",
            [AnomalyType.NutritionalStress] = "#6a3d9a"
        };

        public CreateRandomDemoAnomalyPolygons(MasofaCropMonitoringDbContext cropMonitoringDbContext, MasofaIdentityDbContext identityDbContext, MasofaIndicesDbContext indicesDbContext)
        {
            CropMonitoringDbContext = cropMonitoringDbContext;
            IdentityDbContext = identityDbContext;
            IndicesDbContext = indicesDbContext;
        }

        public void Dispose()
        {
            Console.WriteLine("\nCreateRandomPolygons END");
        }

        public async Task Execute()
        {
            Console.WriteLine("CreateRandomPolygons START\n");

            var lastUpdateUser = await IdentityDbContext.Users.FirstAsync(m => m.UserName.ToLower().Equals("admin"));

            var fieldId = new Guid("019b315c-59f8-7e86-a181-36598069192f");
            var regionId = new Guid("b17e8dbd-4ef5-450c-9412-83f126e2f9df");

            var seasons = await CropMonitoringDbContext.Seasons.Where(s => s.FieldId != null && s.FieldId.Value == fieldId).ToListAsync();

            List<AnomalyPolygon> anomalyPolygons = [];

            foreach (var season in seasons)
            {
                if (season.Polygon == null) continue;

                var anomalyPolygon = CreateGridPolygon(season.Polygon, 0.001);
                Console.WriteLine($"{season.Id}: {anomalyPolygon.ToText()}");

                var anomalyType = GetRandomAnomalyType();

                var newPolygon = new AnomalyPolygon()
                {
                    OriginalDate = DateTime.UtcNow,
                    ProductSourceType = ProductSourceType.Sentinel2,
                    SatelliteProductId = Guid.Empty,
                    AnomalyType = anomalyType,
                    Color = _colors.GetValueOrDefault(anomalyType, string.Empty),
                    Polygon = anomalyPolygon,
                    RegionId = regionId,
                    FieldId = fieldId,
                    SeasonId = season.Id,
                    CreateAt = DateTime.UtcNow,
                    CreateUser = lastUpdateUser.Id,
                    LastUpdateUser = lastUpdateUser.Id,
                    LastUpdateAt = DateTime.UtcNow,
                    Status = StatusType.Active
                };

                anomalyPolygons.Add(newPolygon);
            }

            await IndicesDbContext.AnomalyPolygons.AddRangeAsync(anomalyPolygons);
            await IndicesDbContext.SaveChangesAsync();
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        public static Polygon CreateGridPolygon(Polygon inputPolygon, double stepMeters = 10.0, GeometryFactory factory = null)
        {
            if (inputPolygon == null || inputPolygon.IsEmpty)
                throw new ArgumentException("Input polygon is null or empty.");

            factory ??= inputPolygon.Factory;

            var envelope = inputPolygon.EnvelopeInternal;
            var minX = envelope.MinX;
            var minY = envelope.MinY;
            var maxX = envelope.MaxX;
            var maxY = envelope.MaxY;

            var points = new List<Coordinate>();

            // Генерируем регулярную сетку с шагом stepMeters
            for (double y = minY; y <= maxY; y += stepMeters)
            {
                for (double x = minX; x <= maxX; x += stepMeters)
                {
                    var pt = factory.CreatePoint(new Coordinate(x, y));
                    if (inputPolygon.Covers(pt)) // учитывает дырки
                    {
                        points.Add(new Coordinate(x, y));
                    }
                }
            }

            if (points.Count == 0)
                return factory.CreatePolygon(); // пустой полигон

            // Чтобы создать полигон из точек сетки, нужен convex hull или custom алгоритм.
            // Но "полигон из сетки" — неоднозначное понятие. Чаще хотят convex hull или union буферов.
            // Вариант: делаем union кругов радиусом step/2 вокруг каждой точки и упрощаем до полигона.

            var geometries = new List<Geometry>();
            foreach (var coord in points)
            {
                // Каждую точку превращаем в маленький квадрат (или круг), чтобы потом объединить
                var buffered = factory.CreatePoint(coord).Buffer(stepMeters / 2.0, 1); // 1 — низкая детализация (квадрат)
                geometries.Add(buffered);
            }

            if (geometries.Count == 1)
                return geometries[0] as Polygon ?? factory.CreatePolygon();

            var union = factory.BuildGeometry(geometries).Union();
            if (union is Polygon poly)
                return poly;
            if (union is MultiPolygon mp && mp.NumGeometries > 0)
                return mp[0] as Polygon ?? factory.CreatePolygon(); // или обработать все части

            return factory.CreatePolygon();
        }

        private AnomalyType GetRandomAnomalyType()
        {
            AnomalyType[] values = { AnomalyType.Flooding, AnomalyType.Overwatering, AnomalyType.Drought, AnomalyType.SkippingOfSeedlings, AnomalyType.PrematureAging, AnomalyType.NutritionalStress };
            return values[Random.Next(values.Length)];
        }
    }
}
