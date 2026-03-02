using Masofa.Common.Resources;
using Masofa.BusinessLogic.FieldSatellite.Commands;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using Newtonsoft.Json;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Globalization;
using Masofa.Common.Helper;
using Microsoft.Extensions.Configuration;
using GeoJsonSerializer = NetTopologySuite.IO.GeoJsonSerializer;
using NtsAttributesTable = NetTopologySuite.Features.AttributesTable;
using NtsFeature = NetTopologySuite.Features.Feature;
using NtsFeatureCollection = NetTopologySuite.Features.FeatureCollection;
using NtsGeometry = NetTopologySuite.Geometries.Geometry;
using Polygon = NetTopologySuite.Geometries.Polygon;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    public class FieldImportRequest : IRequest<List<Masofa.Common.Models.CropMonitoring.Field>>
    {
        public byte[] Bytes { get; set; }
        public FieldExportType ExportType { get; set; }
    }

    public class FieldImportRequestHandler : IRequestHandler<FieldImportRequest, List<Masofa.Common.Models.CropMonitoring.Field>>
    {
        private Dictionary<FieldExportType, Func<Stream, CancellationToken, Task<List<Masofa.Common.Models.CropMonitoring.Field>>>> _importAction;

        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private GeometryFactory GeometryFactory { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IMediator Mediator { get; set; }
        private IConfiguration Configuration { get; set; }
        
        private readonly GeometryFactory _gf = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        public sealed record KmlPlacemark(NetTopologySuite.Geometries.Geometry? Geometry, IReadOnlyDictionary<string, string> Attributes);


        public FieldImportRequestHandler(MasofaDictionariesDbContext masofaDictionariesDbContext, GeometryFactory geometryFactory, IBusinessLogicLogger businessLogicLogger, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, IMediator mediator, IConfiguration configuration)
        {
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            GeometryFactory = geometryFactory;
            BusinessLogicLogger = businessLogicLogger;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            Mediator = mediator;
            Configuration = configuration;
            
            _importAction = new Dictionary<FieldExportType, Func<Stream, CancellationToken, Task<List<Masofa.Common.Models.CropMonitoring.Field>>>>()
            {
                { FieldExportType.GeoJson, (stream, ct) => GeoJsonHandle(stream, ct) },
                { FieldExportType.Kml, (stream, ct) => KmlHandle(stream, ct) },
                { FieldExportType.CSV, (stream, ct) => CsvHandle(stream, ct) },
                { FieldExportType.ShapeFile, (stream, ct) => ShapeFileHandle(stream, ct) }
            };
        }

        public async Task<List<Masofa.Common.Models.CropMonitoring.Field>> Handle(FieldImportRequest command, CancellationToken cancellationToken)
        {
            var fields = new List<Masofa.Common.Models.CropMonitoring.Field>(default(int));
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                using (MemoryStream stream = new MemoryStream(command.Bytes))
                {
                    fields = await (_importAction[command.ExportType](stream, cancellationToken));
                    await MasofaCropMonitoringDbContext.Fields.AddRangeAsync(fields, cancellationToken);
                }
                await MasofaCropMonitoringDbContext.SaveChangesAsync();

                // Пересчитываем маппинг для всех импортированных полей
                if (fields.Any())
                {
                    await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted($"{nameof(FieldImportRequestHandler)}=>{nameof(Handle)}"), $"{nameof(FieldImportRequestHandler)}=>{nameof(Handle)}");
                    
                    // Пересчитываем глобальные границы
                    await Mediator.Send(new RecalculateGlobalBoundariesCommand(cancellationToken), cancellationToken);
                    
                    // Пересчитываем маппинг для каждого импортированного поля
                    foreach (var field in fields.Where(f => f.Polygon != null))
                    {
                        await Mediator.Send(new RecalculateFieldProductsCommand(field.Id, cancellationToken), cancellationToken);
                    }
                    
                    await BusinessLogicLogger.LogInformationAsync(LogMessageResource.FieldRecalculationCompleted(fields.Count), $"{nameof(FieldImportRequestHandler)}=>{nameof(Handle)}");
                }

                return fields;
            }
            catch (Exception ex)
            {
                await BusinessLogicLogger.LogCriticalAsync(LogMessageResource.GenericError(requestPath, ex.Message), $"{nameof(FieldImportRequestHandler)}=>{nameof(Handle)}");
                return fields;
            }
        }

        private async Task<List<Masofa.Common.Models.CropMonitoring.Field>> GeoJsonHandle(Stream stream, CancellationToken cancellationToken)
        {
            var features = await ReadFeatureCollectionAsync(stream, cancellationToken);
            var list = new List<Masofa.Common.Models.CropMonitoring.Field>(features.Count);

            foreach (var f in features)
            {
                var poly = ToPolygonOrNull(f.Geometry);
                if (poly is null) continue;

                if (poly.SRID == 0)
                {
                    poly.SRID = 4326;
                }

                var a = (AttributesTable?)f.Attributes;

                var field = new Masofa.Common.Models.CropMonitoring.Field
                {
                    Name = Get(a, "fieldName") ?? Get(a, "name"),
                    Comment = Get(a, "comment"),
                    ExternalId = Get(a, "externalId"),
                    Polygon = poly,
                    CreateAt = DateTime.UtcNow,
                    LastUpdateAt = DateTime.UtcNow
                };

                if (TryGuid(a, "regionId", out var regionId)) field.RegionId = regionId;
                if (TryGuid(a, "soilTypeId", out var soilType)) field.SoilTypeId = soilType;
                if (TryGuid(a, "agroclimaticZoneId", out var agro)) field.AgroclimaticZoneId = agro;
                if (TryGuid(a, "agricultureProducerId", out var prod)) field.AgricultureProducerId = prod;
                if (TryGuid(a, "irrigationTypeId", out var irrt)) field.IrrigationTypeId = irrt;
                if (TryGuid(a, "irrigationSourceId", out var irrs)) field.IrrigationSourceId = irrs;

                if (TryDouble(a, "soilIndex", out var soilIndex)) field.SoilIndex = soilIndex;
                if (TryDouble(a, "fieldArea", out var area)) field.FieldArea = area;
                if (TryBool(a, "waterSaving", out var ws)) field.WaterSaving = ws;
                if (TryBool(a, "control", out var ctrl)) field.Control = ctrl;

                field.FieldArea = ComputeAreaHa(poly);
                field.RegionId = await ResolveRegionIdAsync(poly, regionId, cancellationToken);

                list.Add(field);
            }

            return list;

        }

        private async Task<List<Masofa.Common.Models.CropMonitoring.Field>> KmlHandle(Stream stream, CancellationToken cancellationToken)
        {
            var placemarks = await ReadPlacemarksAsync(stream, cancellationToken);
            var result = new List<Masofa.Common.Models.CropMonitoring.Field>();

            foreach (var pm in placemarks)
            {
                var poly = ToPolygonOrNull(pm.Geometry);
                if (poly is null) continue;

                if (poly.SRID == 0)
                {
                    poly.SRID = 4326;
                }

                var field = new Masofa.Common.Models.CropMonitoring.Field
                {
                    Name = Get(pm.Attributes, "fieldName") ?? Get(pm.Attributes, "name"),
                    Polygon = poly,
                    CreateAt = DateTime.UtcNow,
                    Comment = Get(pm.Attributes, "comment"),
                    LastUpdateAt = DateTime.UtcNow,
                    RegionId = Guid.Parse(Get(pm.Attributes, "regionId")),
                };

                // безопасные маппинги GUID'ов (если есть)
                if (TryGuid(pm.Attributes, "regionId", out var regionId)) field.RegionId = regionId;
                if (TryGuid(pm.Attributes, "soilTypeId", out var soilTypeId)) field.SoilTypeId = soilTypeId;
                if (TryGuid(pm.Attributes, "agroclimaticZoneId", out var agroZoneId)) field.AgroclimaticZoneId = agroZoneId;
                if (TryGuid(pm.Attributes, "agricultureProducerId", out var prodId)) field.AgricultureProducerId = prodId;
                if (TryGuid(pm.Attributes, "irrigationTypeId", out var irrTypeId)) field.IrrigationTypeId = irrTypeId;
                if (TryGuid(pm.Attributes, "irrigationSourceId", out var irrSourceId)) field.IrrigationSourceId = irrSourceId;

                // числовые/булевые если прислали
                if (TryDouble(pm.Attributes, "soilIndex", out var soilIndex)) field.SoilIndex = soilIndex;
                if (TryDouble(pm.Attributes, "fieldArea", out var area)) field.FieldArea = area;
                if (TryBool(pm.Attributes, "waterSaving", out var ws)) field.WaterSaving = ws;
                if (TryBool(pm.Attributes, "control", out var control)) field.Control = control;

                field.FieldArea = ComputeAreaHa(poly);
                field.RegionId = await ResolveRegionIdAsync(poly, regionId, cancellationToken);

                result.Add(field);
            }

            return result;
        }

        private async Task<List<Masofa.Common.Models.CropMonitoring.Field>> CsvHandle(Stream stream, CancellationToken cancellationToken)
        {
            var fields = new List<Masofa.Common.Models.CropMonitoring.Field>();
            
            using var reader = new StreamReader(stream, leaveOpen: true);
            var csvContent = await reader.ReadToEndAsync();
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            if (lines.Length < 2) return fields;
            
            var headers = lines[0].Split(',').Select(h => h.Trim('"')).ToArray();
            
            for (int i = 1; i < lines.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var values = ParseCsvLine(lines[i]);
                if (values.Length != headers.Length) continue;
                
                var fieldData = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length; j++)
                {
                    fieldData[headers[j]] = values[j].Trim('"');
                }
                
                var field = new Masofa.Common.Models.CropMonitoring.Field
                {
                    Id = TryParseGuid(fieldData.GetValueOrDefault("id")) ?? Guid.NewGuid(),
                    Name = fieldData.GetValueOrDefault("name") ?? $"Field_{i}",
                    Comment = fieldData.GetValueOrDefault("comment"),
                    ExternalId = fieldData.GetValueOrDefault("external_id"),
                    CreateAt = DateTime.UtcNow,
                    LastUpdateAt = DateTime.UtcNow,
                    Status = Masofa.Common.Models.StatusType.Active
                };
                
                if (TryParseGuid(fieldData.GetValueOrDefault("region_id"), out var regionId))
                    field.RegionId = regionId;
                
                if (TryParseGuid(fieldData.GetValueOrDefault("soil_type_id"), out var soilTypeId))
                    field.SoilTypeId = soilTypeId;
                
                if (TryParseGuid(fieldData.GetValueOrDefault("agroclimatic_zone_id"), out var agroZoneId))
                    field.AgroclimaticZoneId = agroZoneId;
                
                if (TryParseGuid(fieldData.GetValueOrDefault("agriculture_producer_id"), out var producerId))
                    field.AgricultureProducerId = producerId;
                
                if (TryParseGuid(fieldData.GetValueOrDefault("irrigation_type_id"), out var irrigationTypeId))
                    field.IrrigationTypeId = irrigationTypeId;
                
                if (TryParseGuid(fieldData.GetValueOrDefault("irrigation_source_id"), out var irrigationSourceId))
                    field.IrrigationSourceId = irrigationSourceId;
                
                if (TryParseDouble(fieldData.GetValueOrDefault("field_area"), out var area))
                    field.FieldArea = area;
                
                if (TryParseDouble(fieldData.GetValueOrDefault("soil_index"), out var soilIndex))
                    field.SoilIndex = soilIndex;
                
                if (TryParseBool(fieldData.GetValueOrDefault("water_saving"), out var waterSaving))
                    field.WaterSaving = waterSaving;
                
                if (TryParseBool(fieldData.GetValueOrDefault("control"), out var control))
                    field.Control = control;
                
                if (TryParseDateTime(fieldData.GetValueOrDefault("create_date"), out var createDate))
                    field.CreateAt = createDate;
                
                if (TryParseDateTime(fieldData.GetValueOrDefault("modify_date"), out var modifyDate))
                    field.LastUpdateAt = modifyDate;
                
                if (TryParseGuid(fieldData.GetValueOrDefault("create_user"), out var createUser))
                    field.CreateUser = createUser;
                else
                    field.CreateUser = Guid.Empty; 

                if (TryParseGuid(fieldData.GetValueOrDefault("modify_user"), out var modifyUser))
                    field.LastUpdateUser = modifyUser;
                else
                    field.LastUpdateUser = field.CreateUser; 

                var wktGeom = fieldData.GetValueOrDefault("polygon");
                if (!string.IsNullOrEmpty(wktGeom))
                {
                    var polygon = ParseWktGeometry(wktGeom);
                    if (polygon != null)
                    {
                        if (polygon.SRID == 0 || polygon.SRID == -1)
                        {
                            var firstCoord = polygon.Coordinates[0];
                            
                            if (Math.Abs(firstCoord.X) > 1000000 || Math.Abs(firstCoord.Y) > 1000000)
                            {
                                polygon.SRID = 32641;
                                
                                polygon = TransformFromUtmToWgs84(polygon);
                            }
                            else
                            {
                                polygon.SRID = 4326;
                            }
                        }
                        
                        field.Polygon = polygon;
                        
                        // Проверяем, входит ли поле в границы страны
                        var isWithinCountryBoundaries = GeometryCalculationHelper.IsFieldWithinCountryBoundaries(field, Configuration);
                        if (!isWithinCountryBoundaries)
                        {
                            field.Status = Masofa.Common.Models.StatusType.Hiden;
                        }
                        
                        if (!field.FieldArea.HasValue)
                        {
                            field.FieldArea = ComputeAreaHa(polygon);
                        }
                        
                        if (!field.RegionId.HasValue)
                        {
                            field.RegionId = await ResolveRegionIdAsync(polygon, null, cancellationToken);
                        }
                    }
                }
                
                fields.Add(field);
            }
            
            return fields;
        }
        
        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;
            
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            
            result.Add(current);
            return result.ToArray();
        }
        
        private static Polygon? ParseWktGeometry(string geometryData)
        {
            if (string.IsNullOrWhiteSpace(geometryData))
                return null;
                
            try
            {
                if (IsHexString(geometryData))
                {
                    return ParseWkbGeometry(geometryData);
                }
                else
                {
                    return ParseWktGeometryText(geometryData);
                }
            }
            catch
            {
                return null;
            }
        }
        
        private static bool IsHexString(string input)
        {
           if (string.IsNullOrWhiteSpace(input) || input.Length % 2 != 0)
                return false;
                
            return input.All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
        }
        
        private static Polygon? ParseWkbGeometry(string hexString)
        {
            try
            {
                var bytes = Convert.FromHexString(hexString);
                
                var reader = new NetTopologySuite.IO.WKBReader();
                var geometry = reader.Read(bytes);
                
                return geometry switch
                {
                    Polygon p => p,
                    MultiPolygon mp when mp.NumGeometries > 0
                        => mp.Geometries.OfType<Polygon>().OrderByDescending(x => x.Area).FirstOrDefault(),
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }
        
        private static Polygon? ParseWktGeometryText(string wkt)
        {
            try
            {
                var reader = new NetTopologySuite.IO.WKTReader();
                var geometry = reader.Read(wkt);
                
                return geometry switch
                {
                    Polygon p => p,
                    MultiPolygon mp when mp.NumGeometries > 0
                        => mp.Geometries.OfType<Polygon>().OrderByDescending(x => x.Area).FirstOrDefault(),
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }
        
        private static Polygon TransformFromUtmToWgs84(Polygon utmPolygon)
        {
            try
            {
                var utm = ProjectedCoordinateSystem.WGS84_UTM(41, true); // Zone 41N
                var wgs84 = GeographicCoordinateSystem.WGS84;
                
                var transform = new CoordinateTransformationFactory()
                    .CreateFromCoordinateSystems(utm, wgs84)
                    .MathTransform;
                
                Coordinate[] TransformCoords(Coordinate[] src)
                {
                    var dst = new Coordinate[src.Length];
                    for (int i = 0; i < src.Length; i++)
                    {
                        var p = transform.Transform(new[] { src[i].X, src[i].Y });
                        dst[i] = new Coordinate(p[0], p[1]);
                    }
                    return dst;
                }
                
                var gf = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var shell = gf.CreateLinearRing(TransformCoords(utmPolygon.ExteriorRing.Coordinates));
                
                var holes = utmPolygon.NumInteriorRings == 0
                    ? Array.Empty<NetTopologySuite.Geometries.LinearRing>()
                    : Enumerable.Range(0, utmPolygon.NumInteriorRings)
                        .Select(i => gf.CreateLinearRing(TransformCoords(utmPolygon.GetInteriorRingN(i).Coordinates)))
                        .ToArray();
                
                return gf.CreatePolygon(shell, holes);
            }
            catch
            {
                return utmPolygon;
            }
        }

        private async Task<List<Masofa.Common.Models.CropMonitoring.Field>> ShapeFileHandle(Stream stream, CancellationToken cancellationToken)
        {
            var tmp = await ExtractZipToTempAsync(stream, "shape_fields", cancellationToken);
            try
            {
                var shp = Directory.GetFiles(tmp, "*.shp", SearchOption.AllDirectories).FirstOrDefault()
                          ?? throw new InvalidOperationException(".shp not found in zip");

                var result = new List<Masofa.Common.Models.CropMonitoring.Field>();

                IEnumerable<IFeature> feats = Shapefile.ReadAllFeatures(shp);

                foreach (var feat in feats)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var poly = ToPolygonOrNull(feat.Geometry);
                    if (poly is null) continue;

                    if (poly.SRID == 0)
                    {
                        poly.SRID = 4326;
                    }

                    var a = (feat.Attributes as AttributesTable) ?? new AttributesTable();

                    var field = new Masofa.Common.Models.CropMonitoring.Field
                    {
                        Name = Get(a, "fieldName") ?? Get(a, "name"),
                        Comment = Get(a, "comment"),
                        ExternalId = Get(a, "externalId"),
                        RegionId = TryGuid(a, "regionId"),
                        FieldArea = TryDouble(a, "fieldArea"),
                        Polygon = poly,
                        CreateAt = DateTime.UtcNow,
                        LastUpdateAt = DateTime.UtcNow
                    };

                    field.FieldArea = ComputeAreaHa(poly);
                    field.RegionId = await ResolveRegionIdAsync(poly, field.RegionId, cancellationToken);

                    result.Add(field);
                }

                return result;
            }
            finally
            {
                Directory.Delete(tmp, true);
            }
        }

        #region Helpers
        private async Task<Guid?> ResolveRegionIdAsync(Polygon? poly, Guid? regionIdHint, CancellationToken ct)
        {
            if (regionIdHint.HasValue && regionIdHint.Value != Guid.Empty)
                return regionIdHint;

            if (poly is null) return null;

            var p = poly.Centroid;
            if (p.SRID == 0) p.SRID = 4326;

            var regionMapId = await MasofaDictionariesDbContext.RegionMaps.AsNoTracking()
                .Where(rm => rm.Polygon != null && rm.Polygon.Contains(p))
                .Select(rm => (Guid?)rm.Id)
                .FirstOrDefaultAsync(ct);

            if (regionMapId is null)
            {
                return null;
            }

            return await MasofaDictionariesDbContext.Regions.AsNoTracking()
            .Where(r => r.RegionMapId == regionMapId.Value)
            .Select(r => (Guid?)r.Id)
            .FirstOrDefaultAsync(ct);
        }

        private static double? ComputeAreaHa(Polygon? poly)
        {
            if (poly is null) return null;

            var c = poly.Centroid;
            var lon = c.X; var lat = c.Y;

            int zone = (int)Math.Floor((lon + 180d) / 6d) + 1;
            bool north = lat >= 0;

            var csFactory = new CoordinateSystemFactory();
            var wgs84 = GeographicCoordinateSystem.WGS84;
            var utm = ProjectedCoordinateSystem.WGS84_UTM(zone, north);

            var ctFactory = new CoordinateTransformationFactory();
            var trans = ctFactory.CreateFromCoordinateSystems(wgs84, utm);

            var transform = new CoordinateTransformationFactory()
                .CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84,
                                             ProjectedCoordinateSystem.WGS84_UTM(zone, north))
                .MathTransform;

            var polyUtm = TransformToUtm(poly, transform, poly.Factory);
            return polyUtm.Area / 10_000d; // м² -> гектары
        }

        private static Polygon TransformToUtm(Polygon poly, ProjNet.CoordinateSystems.Transformations.MathTransform mt, GeometryFactory gf)
        {
            Coordinate[] Tx(Coordinate[] src)
            {
                var dst = new Coordinate[src.Length];
                for (int i = 0; i < src.Length; i++)
                {
                    var p = mt.Transform(new[] { src[i].X, src[i].Y });
                    dst[i] = new Coordinate(p[0], p[1]);
                }
                return dst;
            }

            var shell = gf.CreateLinearRing(Tx(poly.ExteriorRing.Coordinates));
            var holes = poly.NumInteriorRings == 0
                ? Array.Empty<NetTopologySuite.Geometries.LinearRing>()
                : Enumerable.Range(0, poly.NumInteriorRings)
                    .Select(i => gf.CreateLinearRing(Tx(poly.GetInteriorRingN(i).Coordinates)))
                    .ToArray();

            return gf.CreatePolygon(shell, holes);
        }

        private static async Task<string> ExtractZipToTempAsync(Stream zip, string prefix, CancellationToken ct)
        {
            var root = Path.Combine(Path.GetTempPath(), $"{prefix}_{Guid.NewGuid():N}");
            Directory.CreateDirectory(root);
            using var archive = new System.IO.Compression.ZipArchive(zip, System.IO.Compression.ZipArchiveMode.Read, leaveOpen: true);
            foreach (var e in archive.Entries)
            {
                ct.ThrowIfCancellationRequested();
                var full = Path.Combine(root, e.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                using var es = e.Open();
                await using var fs = File.Create(full);
                await es.CopyToAsync(fs, ct);
            }
            return root;
        }

        private async Task<NtsFeatureCollection> ReadFeatureCollectionAsync(Stream s, CancellationToken ct)
        {
            using var sr = new StreamReader(s, leaveOpen: true);
            var text = await sr.ReadToEndAsync();
            using var jr = new JsonTextReader(new StringReader(text));

            var serializer = GeoJsonSerializer.Create(this.GeometryFactory);

            var fc = serializer.Deserialize<NtsFeatureCollection>(jr);
            if (fc != null)
                return fc;

            // допустим «голая» геометрия
            jr.Close();
            using var jr2 = new JsonTextReader(new StringReader(text));
            NtsGeometry? geom = serializer.Deserialize<NtsGeometry>(jr2);

            var coll = new NtsFeatureCollection();
            if (geom != null)
                coll.Add(new NtsFeature(geom, new NtsAttributesTable()));
            return coll;
        }

        private static Polygon? ToPolygonOrNull(NtsGeometry? g)
        {
            if (g is null) return null;
            return g switch
            {
                Polygon p => p,
                MultiPolygon mp when mp.NumGeometries > 0
                    => mp.Geometries.OfType<Polygon>().OrderByDescending(x => x.Area).FirstOrDefault(),
                GeometryCollection gc when gc.NumGeometries > 0
                    => gc.Geometries.OfType<Polygon>().OrderByDescending(x => x.Area).FirstOrDefault(),
                _ => null
            };
        }

        private static string? Get(IAttributesTable? a, string key)
        {
            return a != null && a.Exists(key)
            ? a[key]?.ToString()
            : null;
        }
        private static string? Get(IReadOnlyDictionary<string, string> map, string key)
        {
            return map.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v) ? v : null;
        }

        private static bool TryGuid(IAttributesTable? a, string key, out Guid value)
        {
            value = default;
            var s = Get(a, key);
            return s is not null && Guid.TryParse(s, out value);
        }

        private static bool TryGuid(IReadOnlyDictionary<string, string> map, string key, out Guid value)
        {
            value = default;

            return map.TryGetValue(key, out var s) && Guid.TryParse(s, out value);
        }

        private static Guid? TryGuid(AttributesTable a, string key)
        {
            return Guid.TryParse(Get(a, key), out var g) ? g : null;
        }

        private static double? TryDouble(AttributesTable a, string key)
        {
            return double.TryParse(Get(a, key), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;
        }

        private static bool TryDouble(AttributesTable? a, string key, out double val)
        {
            val = default;
            var s = Get(a, key);
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out val)
                || double.TryParse(s, out val);
        }

        private static bool TryBool(AttributesTable? a, string key, out bool val)
        {
            val = default;
            var s = Get(a, key)?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(s)) return false;
            if (bool.TryParse(s, out val)) return true;
            if (s is "1" or "yes" or "y" or "on") { val = true; return true; }
            if (s is "0" or "no" or "n" or "off") { val = false; return true; }
            return false;
        }

        private static bool TryDouble(IReadOnlyDictionary<string, string> map, string key, out double value)
        {
            value = default;
            return map.TryGetValue(key, out var s) && double.TryParse(
                s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryBool(IReadOnlyDictionary<string, string> map, string key, out bool value)
        {
            value = default;
            if (!map.TryGetValue(key, out var s) || string.IsNullOrWhiteSpace(s))
                return false;

            s = s.Trim();
            if (bool.TryParse(s, out value)) return true;
            if (s is "1" or "yes" or "y" or "on") { value = true; return true; }
            if (s is "0" or "no" or "n" or "off") { value = false; return true; }
            return false;
        }

        private static bool TryDate(IAttributesTable? a, string key, out DateTime value)
        {
            value = default;
            var s = Get(a, key);
            return s is not null && DateTime.TryParse(s, out value);
        }
        
        private static Guid? TryParseGuid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return Guid.TryParse(value, out var guid) ? guid : null;
        }
        
        private static bool TryParseGuid(string? value, out Guid guid)
        {
            guid = default;
            if (string.IsNullOrWhiteSpace(value)) return false;
            return Guid.TryParse(value, out guid);
        }
        
        private static bool TryParseDouble(string? value, out double result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(value)) return false;
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }
        
        private static bool TryParseBool(string? value, out bool result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(value)) return false;
            
            value = value.Trim().ToLowerInvariant();
            if (bool.TryParse(value, out result)) return true;
            if (value is "1" or "yes" or "y" or "on" or "t" or "true") { result = true; return true; }
            if (value is "0" or "no" or "n" or "off" or "f" or "false") { result = false; return true; }
            return false;
        }
        
        private static bool TryParseDateTime(string? value, out DateTime result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(value)) return false;
            
            if (DateTime.TryParse(value, out var dateTime))
            {
                // Конвертируем в UTC для PostgreSQL
                result = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                return true;
            }
            
            return false;
        }

        public Task<List<KmlPlacemark>> ReadPlacemarksAsync(Stream kmlStream, CancellationToken ct = default)
        {
            var list = new List<KmlPlacemark>();

            var kmlFile = KmlFile.Load(kmlStream);
            if (kmlFile?.Root == null) return Task.FromResult(list);

            foreach (var placemark in kmlFile.Root.Flatten().OfType<Placemark>())
            {
                var geom = ExtractPolygon(placemark.Geometry);
                var attrs = ExtractAttributes(placemark);
                list.Add(new KmlPlacemark(geom, attrs));
            }

            return Task.FromResult(list);
        }

        private NetTopologySuite.Geometries.Geometry? ExtractPolygon(SharpKml.Dom.Geometry? sharpGeom)
        {
            // SharpKml Geometry -> может быть Polygon / MultiGeometry / и т.п.
            if (sharpGeom is SharpKml.Dom.Polygon p)
            {
                return ToNtsPolygon(p);
            }

            if (sharpGeom is MultipleGeometry mg)
            {
                // берём все Polygon внутри
                var polys = mg.Geometry?.OfType<SharpKml.Dom.Polygon>()
                               .Select(ToNtsPolygon)
                               .Where(g => g != null)
                               .Cast<NetTopologySuite.Geometries.Polygon>()
                               .ToArray();

                return polys is { Length: > 0 } ? _gf.CreateMultiPolygon(polys) : null;
            }

            return null; // линии/точки пропускаем
        }

        private NetTopologySuite.Geometries.Polygon? ToNtsPolygon(SharpKml.Dom.Polygon kmlPoly)
        {
            var outer = kmlPoly.OuterBoundary?.LinearRing?.Coordinates;
            if (outer == null) return null;

            var outerRing = _gf.CreateLinearRing(ToNts(outer));

            var innerRings = new List<NetTopologySuite.Geometries.LinearRing>();
            if (kmlPoly.InnerBoundary != null)
            {
                foreach (var ib in kmlPoly.InnerBoundary)
                {
                    var coords = ib.LinearRing?.Coordinates;
                    if (coords == null) continue;
                    innerRings.Add(_gf.CreateLinearRing(ToNts(coords)));
                }
            }

            return _gf.CreatePolygon(outerRing, innerRings.ToArray());
        }

        private static Coordinate[] ToNts(IEnumerable<SharpKml.Base.Vector> coords)
        {
            var list = new List<Coordinate>();
            foreach (var v in coords)
            {
                // В KML порядок: longitude, latitude (, altitude)
                list.Add(new Coordinate(v.Longitude, v.Latitude));
            }
            // KML может быть не "замкнут": NTS сам замкнёт, но добавим аккуратно
            if (list.Count > 0 && !list[0].Equals2D(list[^1]))
                list.Add(new Coordinate(list[0].X, list[0].Y));

            return list.ToArray();
        }

        private static IReadOnlyDictionary<string, string> ExtractAttributes(Placemark pm)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(pm.Name))
                map["name"] = pm.Name!;

            var ext = pm.ExtendedData;
            if (ext != null)
            {
                // <ExtendedData><Data name=""><value>...</value></Data>...</ExtendedData>
                foreach (var d in ext.Data)
                {
                    if (!string.IsNullOrWhiteSpace(d.Name))
                    {
                        map[d.Name!] = d.Value ?? string.Empty;
                    }
                }

                // <ExtendedData><SchemaData><SimpleData name="">text</SimpleData>...</SchemaData></ExtendedData>
                foreach (var sd in ext.SchemaData)
                {
                    foreach (var s in sd.SimpleData)
                    {
                        if (!string.IsNullOrWhiteSpace(s.Name))
                            map[s.Name!] = s.Text ?? string.Empty;
                    }
                }
            }

            return map;
        }

        #endregion
    }
}
