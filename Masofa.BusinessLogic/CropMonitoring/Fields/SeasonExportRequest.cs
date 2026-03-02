using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using NetTopologySuite.IO.Esri;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using KmlGeometry = SharpKml.Dom.Geometry;
using KmlInnerBoundary = SharpKml.Dom.InnerBoundary;
using KmlLinearRing = SharpKml.Dom.LinearRing;
using KmlOuterBoundary = SharpKml.Dom.OuterBoundary;
using KmlPolygon = SharpKml.Dom.Polygon;
using KmlVector = SharpKml.Base.Vector;
using NtsGeometry = NetTopologySuite.Geometries.Geometry;
using NtsGeometryCollection = NetTopologySuite.Geometries.GeometryCollection;
using NtsMultiPolygon = NetTopologySuite.Geometries.MultiPolygon;
using NtsPolygon = NetTopologySuite.Geometries.Polygon;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    public class SeasonExportRequest : IRequest<byte[]>
    {
        public BaseGetQuery<Masofa.Common.Models.CropMonitoring.Season> SeasonsQuery { get; set; }
        public FieldExportType FieldExportType { get; set; }
    }

    public class SeasonOrTemplateExportRequest : IRequest<byte[]>
    {
        public BaseGetQuery<Masofa.Common.Models.CropMonitoring.Season> SeasonsQuery { get; set; }
        public FieldExportType FieldExportType { get; set; }
        public bool IsTemplate { get; set; } = false!;
    }

    public class SeasonExportRequestHandler : IRequestHandler<SeasonOrTemplateExportRequest, byte[]>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IMediator Mediator { get; set; }

        private Dictionary<FieldExportType, Func<IEnumerable<Masofa.Common.Models.CropMonitoring.Season>, byte[]>> _exportActions = new()
        {
            {
                FieldExportType.CSV, CsvHandle
            },
            {
                FieldExportType.Kml, KmlHandle
            },
            {
                FieldExportType.GeoJson, GeoJsonHandle
            },
            {
                FieldExportType.ShapeFile, ShapeFileHandle
            }
        };

        private static readonly Dictionary<string, string> _db3NameMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // ≤ 10 символов, ASCII, верхний регистр
            ["id"] = "ID",
            ["createAt"] = "CREATE_AT",
            ["status"] = "STATUS",
            ["lastUpdateAt"] = "LUPD_AT",
            ["createUser"] = "CR_USER",
            ["lastUpdateUser"] = "LUPD_USER",
            ["isPublic"] = "ISPUBLIC",
            ["startDate"] = "START_DATE",
            ["endDate"] = "END_DATE",
            ["title"] = "TITLE",
            ["plantingDate"] = "PLAN_DATE",
            ["harvestingDate"] = "HARV_DATE",
            ["fieldId"] = "FIELD_ID",
            ["latitude"] = "LATITUDE",
            ["longitude"] = "LONGITUDE",
            ["cropId"] = "CROP_ID",
            ["varietyId"] = "VARIETY_ID",
            ["plantingDatePlan"] = "PDATE_PLAN",
            ["fieldArea"] = "FIELDAREA",
            ["harvestingDatePlan"] = "HDATE_PLAN",
            ["yieldHaFact"] = "YIELD_HA_F",
            ["yieldFact"] = "YIELD_F",
            ["yieldHaPlan"] = "YIELD_HA_P",
            ["yieldPlan"] = "YIELD_P",
            ["polygon"] = "POLYGON"
        };

        public SeasonExportRequestHandler(IBusinessLogicLogger businessLogicLogger, IMediator mediator)
        {
            BusinessLogicLogger = businessLogicLogger;
            Mediator = mediator;
        }

        public async Task<byte[]> Handle(SeasonOrTemplateExportRequest command, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                List<Masofa.Common.Models.CropMonitoring.Season>? seasons = [];
                if (command.IsTemplate)
                {
                    var newSeason = CreateDefaultInstance<Masofa.Common.Models.CropMonitoring.Season>();
                    newSeason.PolygonJson = "POLYGON ((0 0, 0 1, 1 1, 1 0, 0 0))";
                    seasons.Add(newSeason);
                }
                else
                {
                    var getRequest = new BaseGetRequest<Masofa.Common.Models.CropMonitoring.Season, MasofaCropMonitoringDbContext>()
                    {
                        Query = command.SeasonsQuery
                    };
                    seasons = await Mediator.Send(getRequest, cancellationToken);
                }

                return _exportActions[command.FieldExportType](seasons);
            }
            catch (Exception ex)
            {
                await BusinessLogicLogger.LogCriticalAsync(LogMessageResource.GenericError(requestPath, ex.Message), $"{nameof(SeasonExportRequestHandler)}=>{nameof(Handle)}");
                throw ex;
            }
        }

        public static T CreateDefaultInstance<T>() where T : new()
        {
            var instance = new T();
            var type = typeof(T);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.CanWrite) continue;

                var valueType = property.PropertyType;
                var defaultValue = GetDefaultValue(valueType);
                property.SetValue(instance, defaultValue);
            }

            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                var defaultValue = GetDefaultValue(fieldType);
                field.SetValue(instance, defaultValue);
            }

            return instance;
        }

        private static object? GetDefaultValue(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                if (underlyingType == typeof(Guid))
                {
                    return Guid.Empty;
                }

                return null;
            }

            if (type.IsValueType)
            {
                if (type == typeof(Guid))
                {
                    return Guid.Empty;
                }

                return Activator.CreateInstance(type)!;
            }

            if (type == typeof(string))
            {
                return string.Empty;
            }

            return null;
        }

        private static byte[] CsvHandle(IEnumerable<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var sb = new StringBuilder();
            // Заголовки (без геометрии)
            sb.AppendLine(string.Join(",",
                "Id", "CreateAt", "Status", "LastUpdateAt", "CreateUser", "LastUpdateUser", "IsPublic",
                "StartDate", "EndDate", "Title", "PlantingDate", "HarvestingDate", "FieldId", "Latitude",
                "Longitude", "CropId", "VarietyId", "PlantingDatePlan", "FieldArea", "HarvestingDatePlan",
                "YieldHaFact", "YieldFact", "YieldHaPlan", "YieldPlan", "Polygon"
                ));

            foreach (var s in seasons)
            {
                sb.AppendLine(string.Join(",",
                    Csv(s.Id),
                    Csv(s.CreateAt),
                    Csv(s.Status),
                    Csv(s.LastUpdateAt),
                    Csv(s.CreateUser),
                    Csv(s.LastUpdateUser),
                    Csv(s.IsPublic),
                    Csv(s.StartDate),
                    Csv(s.EndDate),
                    Csv(s.Title),
                    Csv(s.PlantingDate),
                    Csv(s.HarvestingDate),
                    Csv(s.FieldId),
                    Csv(s.Latitude),
                    Csv(s.Longitude),
                    Csv(s.CropId),
                    Csv(s.VarietyId),
                    Csv(s.PlantingDatePlan),
                    Csv(s.FieldArea),
                    Csv(s.HarvestingDatePlan),
                    Csv(s.YieldHaFact),
                    Csv(s.YieldFact),
                    Csv(s.YieldHaPlan),
                    Csv(s.YieldPlan),
                    Csv(s.PolygonJson)
                ));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static string Csv(object? v)
        {
            if (v is null) return "";
            return v switch
            {
                DateTime dt => Quote(dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
                DateOnly d => Quote(d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                bool b => Quote(b ? "true" : "false"),
                double d => Quote(d.ToString(CultureInfo.InvariantCulture)),
                float f => Quote(f.ToString(CultureInfo.InvariantCulture)),
                IFormattable f => Quote(f.ToString(null, CultureInfo.InvariantCulture) ?? ""),
                _ => Quote(v.ToString() ?? "")
            };

            static string Quote(string s)
            {
                var esc = s.Replace("\"", "\"\"");
                return $"\"{esc}\"";
            }
        }

        private static byte[] KmlHandle(IEnumerable<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var kmlDoc = new Document { Name = "Seasons" };
            foreach (var s in seasons)
            {
                if (s.Polygon is null) continue;

                var pm = new Placemark
                {
                    Name = string.IsNullOrWhiteSpace(s.Title) ? s.Id.ToString() : s.Title,
                    Geometry = ToKmlGeometry(s.Polygon),
                    ExtendedData = BuildSeasonExtendedData(s)
                };
                kmlDoc.AddFeature(pm);
            }
            var kmlBytes = SaveKmlToBytes(new Kml { Feature = kmlDoc });

            var csvBytes = Encoding.UTF8.GetBytes(BuildSeasonsCsv(seasons));

            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true, Encoding.UTF8))
            {
                AddEntry(zip, "seasons.kml", kmlBytes);
                AddEntry(zip, "seasons.csv", csvBytes);
            }
            return ms.ToArray();
        }

        private static KmlGeometry ToKmlGeometry(NtsGeometry ntsGeom)
        {
            if (ntsGeom is NtsPolygon poly)
            {
                return ToKmlPolygon(poly);
            }

            if (ntsGeom is NtsMultiPolygon mp)
            {
                var mg = new SharpKml.Dom.MultipleGeometry();
                for (int i = 0; i < mp.NumGeometries; i++)
                    mg.AddGeometry(ToKmlPolygon((NtsPolygon)mp.GetGeometryN(i)));
                return mg;
            }

            if (ntsGeom is NtsGeometryCollection gc)
            {
                var mg = new SharpKml.Dom.MultipleGeometry();
                for (int i = 0; i < gc.NumGeometries; i++)
                {
                    mg.AddGeometry(ToKmlGeometry(gc.GetGeometryN(i)));
                }

                return mg;
            }

            throw new NotSupportedException($"Geometry type {ntsGeom?.GeometryType} is not supported");
        }

        private static KmlPolygon? ToKmlPolygon(NtsPolygon? poly)
        {
            if (poly is null || poly.IsEmpty) return null;

            var outerRing = ToRingOrNull(poly.ExteriorRing?.CoordinateSequence);
            if (outerRing is null) return null;

            var kmlPoly = new KmlPolygon
            {
                OuterBoundary = new KmlOuterBoundary { LinearRing = outerRing }
            };

            for (int i = 0; i < poly.NumInteriorRings; i++)
            {
                var inner = ToRingOrNull(poly.GetInteriorRingN(i)?.CoordinateSequence);
                if (inner is not null)
                    kmlPoly.AddInnerBoundary(new KmlInnerBoundary { LinearRing = inner });
            }

            return kmlPoly;
        }

        private static KmlLinearRing? ToRingOrNull(NetTopologySuite.Geometries.CoordinateSequence? seq)
        {
            if (seq == null || seq.Count == 0) return null;

            var coords = seq.ToCoordinateArray();
            if (coords == null || coords.Length < 4) return null;

            var ring = new KmlLinearRing
            {
                Coordinates = new CoordinateCollection()
            };

            foreach (var c in coords)
            {
                if (c == null) continue;
                if (double.IsNaN(c.X) || double.IsNaN(c.Y)) continue;

                ring.Coordinates.Add(new Vector(c.Y, c.X, double.IsNaN(c.Z) ? 0 : c.Z));
            }

            if (ring.Coordinates.Count > 0)
            {
                var first = ring.Coordinates.First();
                var last = ring.Coordinates.Last();

                if (first.Latitude != last.Latitude || first.Longitude != last.Longitude)
                {
                    ring.Coordinates.Add(new KmlVector(first.Latitude, last.Longitude, first.Altitude ?? 0));
                }
            }

            return ring.Coordinates.Count >= 4 ? ring : (KmlLinearRing?)null;
        }

        private static ExtendedData BuildSeasonExtendedData(Masofa.Common.Models.CropMonitoring.Season s)
        {
            var ext = new ExtendedData();
            Add(ext, "id", s.Id);
            Add(ext, "createAt", s.CreateAt);
            Add(ext, "status", s.Status);
            Add(ext, "lastUpdateAt", s.LastUpdateAt);
            Add(ext, "createUser", s.CreateUser);
            Add(ext, "lastUpdateUser", s.LastUpdateUser);
            Add(ext, "isPublic", s.IsPublic);
            Add(ext, "startDate", s.StartDate);
            Add(ext, "endDate", s.EndDate);
            Add(ext, "title", s.Title);
            Add(ext, "plantingDate", s.PlantingDate);
            Add(ext, "harvestingDate", s.HarvestingDate);
            Add(ext, "fieldId", s.FieldId);
            Add(ext, "latitude", s.Latitude);
            Add(ext, "longitude", s.Longitude);
            Add(ext, "cropId", s.CropId);
            Add(ext, "varietyId", s.VarietyId);
            Add(ext, "plantingDatePlan", s.PlantingDatePlan);
            Add(ext, "fieldArea", s.FieldArea);
            Add(ext, "harvestingDatePlan", s.HarvestingDatePlan);
            Add(ext, "yieldHaFact", s.YieldHaFact);
            Add(ext, "yieldFact", s.YieldFact);
            Add(ext, "yieldHaPlan", s.YieldHaPlan);
            Add(ext, "yieldPlan", s.YieldPlan);
            Add(ext, "polygon", s.PolygonJson);
            return ext;
        }

        private static void Add(ExtendedData ext, string name, object? val)
        {
            if (val is null) return;
            ext.AddData(new Data
            {
                Name = name,
                Value = ConvertToString(val)
            });
        }

        private static string ConvertToString(object val)
        {
            return val switch
            {
                IFormattable fmt => fmt.ToString(null, CultureInfo.InvariantCulture),
                _ => val.ToString() ?? string.Empty
            };
        }

        private static byte[] SaveKmlToBytes(Kml kml)
        {
            using var ms = new MemoryStream();
            KmlFile.Create(kml, false).Save(ms);
            return ms.ToArray();
        }

        private static string BuildSeasonsCsv(IEnumerable<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",",
                "Id", "CreateAt", "Status", "LastUpdateAt", "CreateUser", "LastUpdateUser", "IsPublic",
                "StartDate", "EndDate", "Title", "PlantingDate", "HarvestingDate", "FieldId", "Latitude",
                "Longitude", "CropId", "VarietyId", "PlantingDatePlan", "FieldArea", "HarvestingDatePlan",
                "YieldHaFact", "YieldFact", "YieldHaPlan", "YieldPlan", "Polygon"
                ));

            foreach (var s in seasons)
            {
                sb.AppendLine(string.Join(",",
                    Csv(s.Id),
                    Csv(s.CreateAt),
                    Csv(s.Status),
                    Csv(s.LastUpdateAt),
                    Csv(s.CreateUser),
                    Csv(s.LastUpdateUser),
                    Csv(s.IsPublic),
                    Csv(s.StartDate),
                    Csv(s.EndDate),
                    Csv(s.Title),
                    Csv(s.PlantingDate),
                    Csv(s.HarvestingDate),
                    Csv(s.FieldId),
                    Csv(s.Latitude),
                    Csv(s.Longitude),
                    Csv(s.CropId),
                    Csv(s.VarietyId),
                    Csv(s.PlantingDatePlan),
                    Csv(s.FieldArea),
                    Csv(s.HarvestingDatePlan),
                    Csv(s.YieldHaFact),
                    Csv(s.YieldFact),
                    Csv(s.YieldHaPlan),
                    Csv(s.YieldPlan),
                    Csv(s.PolygonJson)
                ));
            }

            return sb.ToString();
        }

        private static void AddEntry(ZipArchive zip, string name, byte[] payload)
        {
            var e = zip.CreateEntry(name, CompressionLevel.Optimal);
            using var s = e.Open();
            s.Write(payload, 0, payload.Length);
        }

        private static byte[] GeoJsonHandle(IEnumerable<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var fc = new NetTopologySuite.Features.FeatureCollection();

            foreach (var s in seasons)
            {
                if (s.Polygon is null) continue;

                var attrs = new NetTopologySuite.Features.AttributesTable
                {
                    { "id", s.Id },
                    { "createAt", s.CreateAt },
                    { "status", s.Status },
                    { "lastUpdateAt", s.LastUpdateAt },
                    { "createUser", s.CreateUser },
                    { "lastUpdateUser", s.LastUpdateUser },
                    { "isPublic", s.IsPublic },
                    { "startDate", s.StartDate },
                    { "endDate", s.EndDate },
                    { "title", s.Title },
                    { "plantingDate", s.PlantingDate },
                    { "harvestingDate", s.HarvestingDate },
                    { "fieldId", s.FieldId },
                    { "latitude", s.Latitude },
                    { "longitude", s.Longitude },
                    { "cropId", s.CropId },
                    { "varietyId", s.VarietyId },
                    { "plantingDatePlan", s.PlantingDatePlan },
                    { "fieldArea", s.FieldArea },
                    { "harvestingDatePlan", s.HarvestingDatePlan },
                    { "yieldHaFact", s.YieldHaFact },
                    { "yieldFact", s.YieldFact },
                    { "yieldHaPlan", s.YieldHaPlan },
                    { "yieldPlan", s.YieldPlan },
                    { "polygon", s.PolygonJson }
                };

                fc.Add(new NetTopologySuite.Features.Feature(s.Polygon, attrs));
            }

            var geoJsonBytes = SerializeGeoJson(fc);

            // ----- CSV -----
            var csvText = BuildSeasonsCsv(seasons);
            var csvBytes = Encoding.UTF8.GetBytes(csvText);

            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true, Encoding.UTF8))
            {
                AddEntry(zip, "seasons.geojson", geoJsonBytes);
                AddEntry(zip, "seasons.csv", csvBytes);
            }
            return ms.ToArray();
        }

        private static byte[] SerializeGeoJson(NetTopologySuite.Features.FeatureCollection fc)
        {
            var writer = new GeoJsonWriter();       // из NetTopologySuite.IO.GeoJSON
            var json = writer.Write(fc);            // строка с корректным GeoJSON
            return Encoding.UTF8.GetBytes(json);
        }

        private static byte[] ShapeFileHandle(IEnumerable<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var features = new List<IFeature>();
            foreach (var s in seasons)
            {
                if (s.Polygon is null) continue;

                var attrs = new AttributesTable();

                AddAttrStr(attrs, "id", s.Id);
                AddAttrStr(attrs, "createAt", s.CreateAt);
                AddAttrStr(attrs, "status", s.Status);
                AddAttrStr(attrs, "lastUpdateAt", s.LastUpdateAt);
                AddAttrStr(attrs, "createUser", s.CreateUser);
                AddAttrStr(attrs, "lastUpdateUser", s.LastUpdateUser);
                AddAttrStr(attrs, "isPublic", s.IsPublic);
                AddAttrStr(attrs, "startDate", s.StartDate);
                AddAttrStr(attrs, "endDate", s.EndDate);
                AddAttrStr(attrs, "title", s.Title);
                AddAttrStr(attrs, "plantingDate", s.PlantingDate);
                AddAttrStr(attrs, "harvestingDate", s.HarvestingDate);
                AddAttrStr(attrs, "fieldId", s.FieldId);
                AddAttrStr(attrs, "latitude", s.Latitude);
                AddAttrStr(attrs, "longitude", s.Longitude);
                AddAttrStr(attrs, "cropId", s.CropId);
                AddAttrStr(attrs, "varietyId", s.VarietyId);
                AddAttrStr(attrs, "plantingDatePlan", s.PlantingDatePlan);
                AddAttrStr(attrs, "fieldArea", s.FieldArea);
                AddAttrStr(attrs, "harvestingDatePlan", s.HarvestingDatePlan);
                AddAttrStr(attrs, "yieldHaFact", s.YieldHaFact);
                AddAttrStr(attrs, "yieldFact", s.YieldFact);
                AddAttrStr(attrs, "yieldHaPlan", s.YieldHaPlan);
                AddAttrStr(attrs, "yieldPlan", s.YieldPlan);
                AddAttrStr(attrs, "polygon", s.PolygonJson);

                features.Add(new NetTopologySuite.Features.Feature(s.Polygon, attrs));
            }

            return ZipShapefileWithCsv("seasons", features, BuildSeasonsCsv(seasons));
        }

        private static void AddAttrStr(AttributesTable a, string canonicalKey, object? value)
        {
            var key = _db3NameMap.TryGetValue(canonicalKey, out var k) ? k : ToDbfKey(canonicalKey);
            var finalKey = key;
            int i = 1;
            while (a.Exists(finalKey))
            {
                var suffix = i.ToString(CultureInfo.InvariantCulture);
                var baseLen = Math.Max(1, 10 - suffix.Length - 1);
                finalKey = (key.Length > baseLen ? key[..baseLen] : key) + "_" + suffix;
                i++;
            }

            a.Add(finalKey, Str(value));
        }

        private static string ToDbfKey(string name)
        {
            var s = Regex.Replace(name ?? "F", "[^A-Za-z0-9_]", "");
            if (string.IsNullOrEmpty(s)) s = "F";
            if (s.Length > 10) s = s.Substring(0, 10);
            return s.ToUpperInvariant();
        }

        private static string Str(object? v)
        {
            if (v is null) return string.Empty;

            return v switch
            {
                string s => s,
                Guid g => g.ToString(),
                DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTime dt => dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                bool b => b ? "true" : "false",
                float or double or decimal or sbyte or byte or short or ushort or int or uint or long or ulong
                    => Convert.ToString(v, CultureInfo.InvariantCulture) ?? string.Empty,
                Enum e => Convert.ToInt32(e, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
                _ => v.ToString() ?? string.Empty
            };
        }

        private static byte[] ZipShapefileWithCsv(string baseName, IEnumerable<IFeature> features, string csvText)
        {
            var tmpDir = Path.Combine(Path.GetTempPath(), $"shape_export_{Guid.NewGuid():N}");
            Directory.CreateDirectory(tmpDir);

            var shp = Path.Combine(tmpDir, $"{baseName}.shp");
            var shx = Path.Combine(tmpDir, $"{baseName}.shx");
            var dbf = Path.Combine(tmpDir, $"{baseName}.dbf");
            var prj = Path.Combine(tmpDir, $"{baseName}.prj");
            var csv = Path.Combine(tmpDir, $"{baseName}.csv");

            const string Wgs84Wkt =
                "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563]]," +
                "PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433]]";

            using (var shpFs = File.Create(shp))
            using (var shxFs = File.Create(shx))
            using (var dbfFs = File.Create(dbf))
            using (var prjFs = File.Create(prj))
            {
                Shapefile.WriteAllFeatures(
                    features,
                    shpFs, shxFs, dbfFs,
                    prjFs, Wgs84Wkt, Encoding.UTF8);
            }

            // пишем CSV с атрибутами
            File.WriteAllText(csv, csvText, Encoding.UTF8);

            // пакуем в ZIP
            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true, Encoding.UTF8))
            {
                AddEntry(zip, $"{baseName}.shp", File.ReadAllBytes(shp));
                AddEntry(zip, $"{baseName}.shx", File.ReadAllBytes(shx));
                AddEntry(zip, $"{baseName}.dbf", File.ReadAllBytes(dbf));
                AddEntry(zip, $"{baseName}.prj", File.ReadAllBytes(prj));
                AddEntry(zip, $"{baseName}.csv", File.ReadAllBytes(csv));
            }

            TryDelete(tmpDir);
            return ms.ToArray();
        }

        private static void TryDelete(string dir)
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch
            {
                /* ignore */
            }
        }
    }
}
