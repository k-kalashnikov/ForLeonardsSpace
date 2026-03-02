using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
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
    public class FieldExportRequest : IRequest<byte[]>
    {
        public BaseGetQuery<Field> FieldsQuery { get; set; }
        public FieldExportType FieldExportType { get; set; }
    }

    public class FieldOrTemplateExportRequest : IRequest<byte[]>
    {
        public BaseGetQuery<Field> FieldsQuery { get; set; }
        public FieldExportType FieldExportType { get; set; }
        public bool IsTemplate { get; set; } = false!;
    }

    //TODO - Когда будем делать репортер перевести на него
    public class FieldExportRequestHandler : IRequestHandler<FieldOrTemplateExportRequest, byte[]>
    {
        private Dictionary<FieldExportType, Func<IEnumerable<Masofa.Common.Models.CropMonitoring.Field>, byte[]>> _exportActions = new Dictionary<FieldExportType, Func<IEnumerable<Masofa.Common.Models.CropMonitoring.Field>, byte[]>>()
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
            ["seasonId"] = "SEASON_ID",
            ["fieldId"] = "FIELD_ID",
            ["title"] = "TITLE",
            ["cropId"] = "CROP_ID",
            ["varietyId"] = "VARIETYID",
            ["plantingDate"] = "PLANT_DATE",
            ["harvestingDate"] = "HARV_DATE",
            ["yieldHa"] = "YIELD_HA",
            ["yield"] = "YIELD",
            ["latitude"] = "LAT",
            ["longitude"] = "LON",
        };

        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IMediator Mediator { get; set; }
        public FieldExportRequestHandler(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, IBusinessLogicLogger businessLogicLogger, IMediator mediator)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            BusinessLogicLogger = businessLogicLogger;
            Mediator = mediator;
        }

        public async Task<byte[]> Handle(FieldOrTemplateExportRequest command, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                List<Field>? fields = [];
                if (command.IsTemplate)
                {
                    var newField = CreateDefaultInstance<Field>();
                    newField.PolygonJson = "POLYGON ((0 0, 0 1, 1 1, 1 0, 0 0))";
                    fields.Add(newField);
                }
                else
                {
                    var getRequest = new BaseGetRequest<Field, MasofaCropMonitoringDbContext>()
                    {
                        Query = command.FieldsQuery
                    };
                    fields = await Mediator.Send(getRequest, cancellationToken);
                }

                return _exportActions[command.FieldExportType](fields);
            }
            catch (Exception ex)
            {
                await BusinessLogicLogger.LogCriticalAsync(LogMessageResource.GenericError(requestPath, ex.Message), $"{nameof(FieldExportRequestHandler)}=>{nameof(Handle)}");
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

        private static byte[] ShapeFileHandle(IEnumerable<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var features = new List<IFeature>();
            foreach (var f in fields)
            {
                if (f.Polygon is null) continue;

                var attrs = new AttributesTable();

                AddAttrStr(attrs, "fieldId", f.Id);
                AddAttrStr(attrs, "fieldName", f.Name);
                AddAttrStr(attrs, "comment", f.Comment);
                AddAttrStr(attrs, "regionId", f.RegionId);
                AddAttrStr(attrs, "externalId", f.ExternalId);
                AddAttrStr(attrs, "fieldArea", f.FieldArea);

                features.Add(new NetTopologySuite.Features.Feature(f.Polygon, attrs));
            }

            return ZipShapefileWithCsv("fields", features, BuildFieldsCsv(fields));
        }
        private static byte[] GeoJsonHandle(IEnumerable<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var fc = new NetTopologySuite.Features.FeatureCollection();

            foreach (var f in fields)
            {
                if (f.Polygon is null) continue;

                var attrs = new NetTopologySuite.Features.AttributesTable
                {
                    { "fieldId",    f.Id },
                    { "fieldName",  f.Name },
                    { "comment",    f.Comment },
                    { "regionId",   f.RegionId },
                    { "externalId", f.ExternalId },
                    { "fieldArea",  f.FieldArea }
                };

                fc.Add(new NetTopologySuite.Features.Feature(f.Polygon, attrs));
            }

            var geoJsonBytes = SerializeGeoJson(fc);

            // ----- CSV -----
            var csvText = BuildFieldsCsv(fields);
            var csvBytes = Encoding.UTF8.GetBytes(csvText);

            // ----- ZIP: fields.geojson + fields.csv -----
            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true, Encoding.UTF8))
            {
                AddEntry(zip, "fields.geojson", geoJsonBytes);
                AddEntry(zip, "fields.csv", csvBytes);
            }
            return ms.ToArray();
        }
        private static byte[] KmlHandle(IEnumerable<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var kmlDoc = new Document { Name = "Fields" };
            foreach (var f in fields)
            {
                if (f.Polygon is null) continue;

                var pm = new Placemark
                {
                    Name = string.IsNullOrWhiteSpace(f.Name) ? f.Id.ToString() : f.Name,
                    Geometry = ToKmlGeometry(f.Polygon),
                    ExtendedData = BuildFieldExtendedData(f)
                };
                kmlDoc.AddFeature(pm);
            }
            var kmlBytes = SaveKmlToBytes(new Kml { Feature = kmlDoc });

            var csvBytes = Encoding.UTF8.GetBytes(BuildFieldsCsv(fields));

            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true, Encoding.UTF8))
            {
                AddEntry(zip, "fields.kml", kmlBytes);
                AddEntry(zip, "fields.csv", csvBytes);
            }
            return ms.ToArray();
        }
        private static byte[] CsvHandle(IEnumerable<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var sb = new StringBuilder();
            // Заголовки (без геометрии)
            sb.AppendLine(string.Join(",",
                "Id", "Name", "FieldArea", "RegionId", "ExternalId", "SoilTypeId",
                "AgroclimaticZoneId", "Comment", "AgricultureProducerId", "IrrigationTypeId",
                "IrrigationSourceId", "WaterSaving", "SoilIndex", "Control",
                "CreateAt", "LastUpdateAt"));

            foreach (var f in fields)
            {
                sb.AppendLine(string.Join(",",
                    Csv(f.Id),
                    Csv(f.Name),
                    Csv(f.FieldArea),
                    Csv(f.RegionId),
                    Csv(f.ExternalId),
                    Csv(f.SoilTypeId),
                    Csv(f.AgroclimaticZoneId),
                    Csv(f.Comment),
                    Csv(f.AgricultureProducerId),
                    Csv(f.IrrigationTypeId),
                    Csv(f.IrrigationSourceId),
                    Csv(f.WaterSaving),
                    Csv(f.SoilIndex),
                    Csv(f.Control),
                    Csv(f.CreateAt),
                    Csv(f.LastUpdateAt)
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
                // CSV-escape: оборачиваем в кавычки, удваиваем внутренние
                var esc = s.Replace("\"", "\"\"");
                return $"\"{esc}\"";
            }
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
                // если нужно замыкать именно в исходную первую точку:
                // ring.Coordinates.Add(new KmlVector(first.Latitude, first.Longitude, first.Altitude ?? 0));
            }

            return ring.Coordinates.Count >= 4 ? ring : (KmlLinearRing?)null;
        }
        private static ExtendedData BuildFieldExtendedData(Masofa.Common.Models.CropMonitoring.Field f)
        {
            var ext = new ExtendedData();
            Add(ext, "id", f.Id);
            Add(ext, "name", f.Name);
            Add(ext, "externalId", f.ExternalId);
            Add(ext, "fieldArea", f.FieldArea);
            Add(ext, "regionId", f.RegionId);
            Add(ext, "soilTypeId", f.SoilTypeId);
            Add(ext, "agroclimZoneId", f.AgroclimaticZoneId);
            Add(ext, "producerId", f.AgricultureProducerId);
            Add(ext, "irrigationTypeId", f.IrrigationTypeId);
            Add(ext, "irrigationSourceId", f.IrrigationSourceId);
            Add(ext, "waterSaving", f.WaterSaving);
            Add(ext, "soilIndex", f.SoilIndex);
            Add(ext, "control", f.Control);
            Add(ext, "comment", f.Comment);
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
        private static string BuildFieldsCsv(IEnumerable<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var sb = new StringBuilder();
            sb.AppendLine("fieldId,fieldName,comment,regionId,externalId,fieldArea");

            foreach (var f in fields)
            {
                sb.AppendLine(string.Join(",",
                    Csv(f.Id),
                    Csv(f.Name),
                    Csv(f.Comment),
                    Csv(f.RegionId),
                    Csv(f.ExternalId),
                    Csv(f.FieldArea),
                    Csv(f.IrrigationSourceId),
                    Csv(f.IrrigationTypeId),
                    Csv(f.SoilTypeId),
                    Csv(f.SoilIndex),
                    Csv(f.AgricultureProducerId),
                    Csv(f.AgroclimaticZoneId),
                    Csv(f.Control),
                    Csv(f.CreateAt),
                    Csv(f.CreateUser)
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
        private static byte[] SerializeGeoJson(FeatureCollection fc)
        {
            var writer = new GeoJsonWriter();       // из NetTopologySuite.IO.GeoJSON
            var json = writer.Write(fc);            // строка с корректным GeoJSON
            return Encoding.UTF8.GetBytes(json);
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
        private static string ToDbfKey(string name)
        {
            var s = Regex.Replace(name ?? "F", "[^A-Za-z0-9_]", "");
            if (string.IsNullOrEmpty(s)) s = "F";
            if (s.Length > 10) s = s.Substring(0, 10);
            return s.ToUpperInvariant();
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

    public enum FieldExportType
    {
        GeoJson = 1,
        Kml = 2,
        CSV = 3,
        ShapeFile = 4
    }
}
