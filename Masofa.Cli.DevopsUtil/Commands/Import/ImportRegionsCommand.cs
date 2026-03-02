using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Quartz.Util;

namespace Masofa.Cli.DevopsUtil.Commands.Import
{
    [BaseCommand("Import Region Polygons", "Import Region Polygons")]
    public class ImportRegionsCommand : IBaseCommand
    {
        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }

        private readonly Dictionary<string, string?> _mappingGid0 = new()
        {
            { "Samarqand'", "Samarqand" },
            { "Qaraqalpaqstan", "Qoraqalpog‘iston" },
            { "Toshkent", "Toshkent viloyati" },
            { "UZB.13_1", "Toshkent shahri" },
        };

        private readonly Dictionary<string, string?> _mappingName1 = new()
        {
            { "Bo'zsuv District", "Bo‘ston tumani" },
            { "Jalolquduq District", "Jalaquduq tumani" },
            { "Bog'dod District", "Bag‘dod tumani" },
            { "Yagiqo'rg'on District", "Yangiqo‘rg‘on tumani" },
            { "Koson District", "Koson tumani" },
            { "Shahrixon City", "Shahrisabz shahri" },
            { "Samarqand' City", "Samarqand shahri " },
            { "Ellikqala District", "Ellikqal‘a tumani" },
            { "O'rtachirchiq District", "O‘rta Chirchiq tumani" },
            { "Quyichirchiq District", "Quyi chirchiq tumani" },
            { "Yuqorichirchiq District", "Yuqori Chirchiq tumani" },
            { "Andijon District", "Andijon tumani" }
        };

        public ImportRegionsCommand(MasofaDictionariesDbContext dictionariesDbContext)
        {
            DictionariesDbContext = dictionariesDbContext;
        }

        public void Dispose()
        {
            Console.WriteLine("\nImportRegionsCommand END");
        }

        public async Task Execute()
        {
            Console.WriteLine("ImportRegionsCommand START\n");

            List<string> attributeNames = ["name", "description", "stroke", "stroke-opacity", "fill-opacity", "GID_1", "GID_0", "COUNTRY", "NAME_1", "VARNAME_1", "NL_NAME_1", "TYPE_1", "ENGTYPE_1", "CC_1", "GID_2", "NAME_2", "VARNAME_2", "NL_NAME_2", "TYPE_2", "ENGTYPE_2"];

            List<string> l3ToSkip = [
                "Oxunboboev District",
                "Jizzax District",
                "Chortoq City",
                "Kosonsoy City",
                "Uchquduq City",
                "Aral Sea Waterbody",
                "Beruniy City",
                "Chimboy City",
                "Qo'ng'irot City",
                "To'rtko'l City",
                "Usmon Yusupov District",
                "Urgut City",
                "Sirdaryo City",
                "Denov City"
            ];

            List<MultiPolygonWithAttributes> mps = [];

            for (int i = 0; i < 3; i++)
            {
                var fileName = $"gadm41_UZB_{i}.json";
                if (!File.Exists(fileName))
                {
                    continue;
                }

                var geoJson = await File.ReadAllTextAsync(fileName);

                var reader = new NetTopologySuite.IO.GeoJsonReader();
                var uzb0FeatureCollection = reader.Read<NetTopologySuite.Features.FeatureCollection>(geoJson);

                foreach (var feature in uzb0FeatureCollection)
                {
                    Dictionary<string, object?> attributeValues = [];

                    var attributes = feature.Attributes;
                    foreach (var attribute in attributeNames)
                    {
                        if (attributes.Exists(attribute))
                        {
                            attributeValues[attribute] = attributes[attribute];
                        }
                    }

                    var factory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), 4326);
                    NetTopologySuite.Geometries.MultiPolygon? mp = null;
                    if (feature.Geometry is NetTopologySuite.Geometries.GeometryCollection geometryCollection)
                    {
                        var polygons = geometryCollection.Geometries.OfType<NetTopologySuite.Geometries.Polygon>().ToList();
                        if (polygons.Count == 0)
                        {
                            throw new InvalidOperationException("Нет ни одного Polygon в GeometryCollection.");
                        }

                        mp = factory.CreateMultiPolygon([.. polygons]);
                    }
                    else if (feature.Geometry is NetTopologySuite.Geometries.Polygon p)
                    {
                        mp = factory.CreateMultiPolygon([p]);
                    }

                    if (mp != null)
                    {
                        var newMpwa = new MultiPolygonWithAttributes()
                        {
                            MultiPolygon = mp,
                            Attributes = attributeValues,
                            Level = i
                        };
                        var name1 = attributeValues.GetValueOrDefault("NAME_1", string.Empty)?.ToString();
                        if (name1 != string.Empty && name1 != null && name1 == "Tashkent City") newMpwa.Level = 1;
                        mps.Add(newMpwa);
                    }
                }
            }

            var regions = await DictionariesDbContext.Regions.Where(r => r.Status == Common.Models.StatusType.Active
                                                                         && r.RegionMapId != null
                                                                         && (r.Comment == null || (
                                                                            r.Comment != null
                                                                            && !r.Comment.ToLower().Contains("test")
                                                                            && !r.Comment.ToLower().Contains("demo"))
                                                                         )).ToListAsync();

            var regionsL2 = regions.Where(r => r.Level == 2).ToList();
            var mpwasL2 = mps.Where(mp => mp.Level == 1).ToList();
            foreach (var mpwa in mpwasL2)
            {
                var progressLine = $"\rProcess level 2 {mpwasL2.IndexOf(mpwa) + 1} of {mpwasL2.Count()}";
                Console.Write(progressLine.PadRight(Console.WindowWidth - 1));

                var gid0 = mpwa.Attributes?.GetValueOrDefault("GID_0", string.Empty)?.ToString();
                if (gid0 == null || gid0.IsNullOrWhiteSpace())
                {
                    continue;
                }

                if (_mappingGid0.TryGetValue(gid0, out string? value))
                {
                    gid0 = value;
                }

                var connectedRegions = regionsL2.Where(r => r.Names["uz-Latn-UZ"].Replace("'", "").Contains(gid0.Replace("'", "‘"), StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (connectedRegions.Count == 1)
                {
                    var regionMapId = connectedRegions[0].RegionMapId;
                    var regionMap = await DictionariesDbContext.RegionMaps.FirstOrDefaultAsync(rm => rm.Id == regionMapId);
                    if (regionMap != null)
                    {
                        regionMap.Polygon = mpwa.MultiPolygon;
                    }
                }
            }
            Console.WriteLine();

            var regionsL3 = regions.Where(r => r.Level == 3).ToList();
            var mpwasL3 = mps.Where(mp => mp.Level == 2).ToList();
            foreach (var mpwa in mpwasL3)
            {
                var progressLine = $"\rProcess level 3 {mpwasL3.IndexOf(mpwa) + 1} of {mpwasL3.Count()}";
                Console.Write(progressLine.PadRight(Console.WindowWidth - 1));
                var name1 = mpwa.Attributes?.GetValueOrDefault("NAME_1", string.Empty)?.ToString();
                var varName2 = mpwa.Attributes?.GetValueOrDefault("VARNAME_2", string.Empty)?.ToString();
                if (name1 == null || name1.IsNullOrWhiteSpace())
                {
                    continue;
                }

                if (l3ToSkip.Contains($"{name1} {varName2}"))
                {
                    continue;
                }

                if (_mappingName1.TryGetValue($"{name1} {varName2}", out string? value))
                {
                    name1 = value;
                }

                var connectedRegions = regionsL3.Where(r =>
                    r.Names["uz-Latn-UZ"].Replace("'", "").Contains(name1.Replace("'", "‘").Replace("’", "‘"), StringComparison.CurrentCultureIgnoreCase)
                    && r.Names["en-US"].Contains(varName2, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (connectedRegions.Count == 1)
                {
                    var regionMapId = connectedRegions[0].RegionMapId;
                    var regionMap = await DictionariesDbContext.RegionMaps.FirstOrDefaultAsync(rm => rm.Id == regionMapId);
                    if (regionMap != null)
                    {
                        regionMap.Polygon = mpwa.MultiPolygon;
                    }
                }
            }
            Console.WriteLine();

            await DictionariesDbContext.SaveChangesAsync();
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        public static NetTopologySuite.Geometries.MultiPolygon ParseToMultiPolygon(string geoJson)
        {
            var reader = new NetTopologySuite.IO.GeoJsonReader();
            var featureCollection = reader.Read<NetTopologySuite.Features.FeatureCollection>(geoJson);

            if (featureCollection == null || featureCollection.Count == 0)
                throw new ArgumentException("GeoJSON не содержит Features.");

            var feature = featureCollection[0];

            if (feature.Geometry is NetTopologySuite.Geometries.GeometryCollection geometryCollection)
            {
                // Собираем все Polygon из GeometryCollection
                var polygons = geometryCollection.Geometries
                    .OfType<NetTopologySuite.Geometries.Polygon>()
                    .ToList();

                if (polygons.Count == 0)
                    throw new InvalidOperationException("Нет ни одного Polygon в GeometryCollection.");

                // Создаём MultiPolygon
                var factory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), 4326); // WGS84
                return factory.CreateMultiPolygon(polygons.ToArray());
            }

            throw new InvalidOperationException("Ожидается GeometryCollection, но получено: " + feature.Geometry.GetType().Name);
        }

        public class MultiPolygonWithAttributes
        {
            public int Level { get; set; }
            public Dictionary<string, object?>? Attributes { get; set; }
            public required NetTopologySuite.Geometries.MultiPolygon MultiPolygon { get; set; }
        }
    }
}
