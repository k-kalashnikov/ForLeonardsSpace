using DnsClient.Internal;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
//using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.Esri;
using SharpKml.Dom;
using SharpKml.Engine;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using static Masofa.BusinessLogic.CropMonitoring.Fields.FieldImportRequestHandler;

namespace Masofa.BusinessLogic.CropMonitoring.ImportedFields
{
    /// <summary>
    /// Запрос на импорт полей
    /// </summary>
    public class ImportFiledsRequest : IRequest<Dictionary<Guid, List<Guid>>>
    {
        /// <summary>
        /// Комментарий
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Импортируемые файлы
        /// </summary>
        public List<IFormFile> Files { get; set; } = [];

        /// <summary>
        /// Автор
        /// </summary>
        [Required]
        public required string Author { get; set; }
    }

    public class ImportFiledsRequestHandler : IRequestHandler<ImportFiledsRequest, Dictionary<Guid, List<Guid>>>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }
        private ILogger<ImportFiledsRequestHandler> Logger { get; set; }
        private IMediator Mediator { get; set; }

        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }

        private static readonly string _errorMSg = "Error reading attributes";
        private static readonly string _importedFieldsBucket = "imported-fields";
        private static readonly byte[] _zipSignature = { 0x50, 0x4B, 0x03, 0x04 };
        private readonly GeometryFactory _gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        public ImportFiledsRequestHandler(IFileStorageProvider fileStorageProvider, IMediator mediator, MasofaCropMonitoringDbContext cropMonitoringDbContext, ILogger<ImportFiledsRequestHandler> logger, IBusinessLogicLogger businessLogicLogger)
        {
            FileStorageProvider = fileStorageProvider;
            Mediator = mediator;
            CropMonitoringDbContext = cropMonitoringDbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<Dictionary<Guid, List<Guid>>> Handle(ImportFiledsRequest request, CancellationToken cancellationToken)
        {

            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            Dictionary<Guid, List<Guid>> result = [];
            foreach (var file in request.Files)
            {
                if (file is null || file.Length == 0) continue;

                try
                {
                    var importedFieldReport = new ImportedFieldReport()
                    {
                        Comment = request.Comment,
                    };

                    var importedFieldReportId = await Mediator.Send(new BaseCreateCommand<ImportedFieldReport, MasofaCropMonitoringDbContext>()
                    {
                        Model = importedFieldReport,
                        Author = request.Author
                    }, cancellationToken);

                    result[importedFieldReportId] = [];

                    await using var s = file.OpenReadStream();
                    using var memoryStream = new MemoryStream();
                    await s.CopyToAsync(memoryStream, cancellationToken);
                    var fileBytes = memoryStream.ToArray();

                    var isZip = IsZip(fileBytes);

                    var fileName = $"{importedFieldReportId}/{file.FileName}";
                    var fileStoragePath = await FileStorageProvider.PushFileAsync(fileBytes, fileName, _importedFieldsBucket);

                    var fileItem = new FileStorageItem()
                    {
                        FileContentType = isZip ? FileContentType.ArchiveZIP : FileContentType.Default,
                        FileStoragePath = fileStoragePath,
                        FileStorageBacket = _importedFieldsBucket,
                        OwnerTypeFullName = typeof(ImportedFieldReport).FullName ?? string.Empty,
                        OwnerId = importedFieldReport.Id,
                        Status = StatusType.Active,
                        CreateAt = importedFieldReport.LastUpdateAt,
                        CreateUser = importedFieldReport.LastUpdateUser,
                        LastUpdateAt = importedFieldReport.LastUpdateAt,
                        LastUpdateUser = importedFieldReport.LastUpdateUser,
                        FileLength = file.Length
                    };

                    var fileStorageItemId = await Mediator.Send(new BaseCreateCommand<FileStorageItem, MasofaCommonDbContext>()
                    {
                        Model = fileItem,
                        Author = request.Author,
                    }, cancellationToken);

                    List<(string, byte[])> filesToWork = [(file.FileName, fileBytes)];
                    if (isZip)
                    {
                        filesToWork = ExtractZip(fileBytes);
                    }

                    List<(NetTopologySuite.Geometries.Polygon polygon, string attributes)> polygons = [];
                    foreach (var (fName, fBytes) in filesToWork)
                    {
                        if (!fName.Contains('.')) continue;

                        var ext = GetExt(fName);

                        if (ext == "geojson")
                        {
                            polygons.AddRange(ReadPolygonFromGeoJsonBytes(fBytes));
                        }

                        if (ext == "kml")
                        {
                            polygons.AddRange(ReadPolygonFromKmlBytes(fBytes));
                        }

                        if (ext == "shp")
                        {
                            var dbfBytes = filesToWork.FirstOrDefault(f => f.Item1.Equals(fName.Replace(ext, "dbf"), StringComparison.CurrentCultureIgnoreCase)).Item2;
                            if (dbfBytes != null)
                            {
                                polygons.AddRange(ReadPolygonFromShpBytes(fBytes, dbfBytes));
                            }
                        }
                    }

                    if (polygons.Count > 0)
                    {
                        foreach (var (polygon, attributes) in polygons)
                        {
                            var newImportedField = new ImportedField()
                            {
                                FieldName = $"{request.Author}_{DateTime.UtcNow:yyyyMMdd}",
                                ImportedFieldReportId = importedFieldReportId,
                                Polygon = polygon,
                                DataJson = attributes
                            };

                            var newImportedFieldId = await Mediator.Send(new BaseCreateCommand<ImportedField, MasofaCropMonitoringDbContext>()
                            {
                                Model = newImportedField,
                                Author = request.Author,
                            }, cancellationToken);

                            result[importedFieldReportId].Add(newImportedFieldId);
                        }
                    }

                    importedFieldReport.Id = importedFieldReportId;
                    importedFieldReport.FileStorageItemId = fileStorageItemId;
                    importedFieldReport.FileSize = file.Length;
                    importedFieldReport.FieldsCount = result.GetValueOrDefault(importedFieldReportId, []).Count;

                    CropMonitoringDbContext.Update(importedFieldReport);
                    await CropMonitoringDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    await BusinessLogicLogger.LogErrorAsync(ex.Message, requestPath);
                    Logger.LogError(ex, ex.Message);
                }
            }

            return result;
        }

        private static string? GetExt(string fullName)
        {
            var parts = fullName.Split('.');
            if (parts.Length > 1)
            {
                return parts[^1].ToLower();
            }

            return null;
        }

        private static bool IsZip(byte[] data)
        {
            if (data.Length < 4) return false;

            for (int i = 0; i < 4; i++)
            {
                if (data[i] != _zipSignature[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static List<(string, byte[])> ExtractZip(byte[] zipData)
        {
            List<(string, byte[])> result = [];

            using var memoryStream = new MemoryStream(zipData);

            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                if (entry.Length == 0)
                {
                    continue;
                }

                if (entry.FullName.StartsWith('.') || entry.FullName.Contains('/') && Path.GetFileName(entry.FullName).StartsWith('.'))
                {
                    continue;
                }

                if (!IsValidEntryName(entry.FullName))
                {
                    continue;
                }

                using var entryStream = entry.Open();
                using var ms = new MemoryStream();
                entryStream.CopyTo(ms);
                var content = ms.ToArray();

                result.Add((entry.Name, content));
            }

            return result;
        }

        private static bool IsValidEntryName(string name)
        {
            if (name.Contains("../") || name.Contains("..\\"))
            {
                return false;
            }

            var path = Path.GetFullPath(Path.Combine("dummy", name));
            return path.StartsWith(Path.GetFullPath("dummy"));
        }

        private static List<(NetTopologySuite.Geometries.Polygon polygon, string attributes)> ReadPolygonFromGeoJsonBytes(byte[] geoJsonBytes)
        {
            List<(NetTopologySuite.Geometries.Polygon polygon, string attributes)> result = [];
            string geoJson = System.Text.Encoding.UTF8.GetString(geoJsonBytes);
            var reader = new GeoJsonReader();
            var featureCollection = reader.Read<NetTopologySuite.Features.FeatureCollection>(geoJson);
            if (featureCollection == null || featureCollection.Count == 0)
            {
                var geometry = reader.Read<NetTopologySuite.Geometries.Geometry>(geoJson);
                if (geometry is NetTopologySuite.Geometries.Polygon poly)
                {
                    result.Add((poly, _errorMSg));
                }
            }

            if (featureCollection != null)
            {
                foreach (var feature in featureCollection)
                {
                    Dictionary<string, object> attrDict = [];
                    var attributes = feature.Attributes;
                    if (attributes != null)
                    {
                        var attrNames = attributes.GetNames();
                        foreach (var attrName in attrNames)
                        {
                            attrDict[attrName] = attributes[attrName];
                        }
                    }

                    var attributesJson = Newtonsoft.Json.JsonConvert.SerializeObject(attrDict)
                        ?? _errorMSg;

                    if (feature.Geometry is NetTopologySuite.Geometries.Polygon polygon)
                    {
                        result.Add((polygon, attributesJson));
                    }
                    else if (feature.Geometry is MultiPolygon multiPolygon && multiPolygon.NumGeometries == 1)
                    {
                        var pols = multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToArray();
                        foreach (var p in pols)
                        {
                            result.Add((p, attributesJson));
                        }
                    }
                }
            }

            return result;
        }

        private List<(NetTopologySuite.Geometries.Polygon polygon, string attributes)> ReadPolygonFromKmlBytes(byte[] kmlBytes)
        {
            List<(NetTopologySuite.Geometries.Polygon polygon, string attributes)> result = [];
            using Stream kmlStream = new MemoryStream(kmlBytes);
            var placemarks = new List<KmlPlacemark>();

            var kmlFile = KmlFile.Load(kmlStream);

            foreach (var placemark in kmlFile.Root.Flatten().OfType<Placemark>())
            {
                var geom = ExtractPolygon(placemark.Geometry);
                var attrs = ExtractAttributes(placemark);
                placemarks.Add(new KmlPlacemark(geom, attrs));
            }

            foreach (var pm in placemarks)
            {
                var poly = ToPolygonOrNull(pm.Geometry);
                if (poly is null)
                {
                    continue;
                }

                if (poly.SRID == 0)
                {
                    poly.SRID = 4326;
                }

                var attributesJson = Newtonsoft.Json.JsonConvert.SerializeObject(pm.Attributes)
                    ?? _errorMSg;

                result.Add((poly, attributesJson));
            }

            return result;
        }

        private List<(NetTopologySuite.Geometries.Polygon polygon, string attributes)> ReadPolygonFromShpBytes(byte[] shpBytes, byte[] dbfBytes)
        {
            List<(NetTopologySuite.Geometries.Polygon polygon, string attributes)> result = [];

            using Stream shpStream = new MemoryStream(shpBytes);
            using Stream dbfStream = new MemoryStream(dbfBytes);
            IEnumerable<IFeature> feats = Shapefile.ReadAllFeatures(shpStream, dbfStream);

            foreach (var feat in feats)
            {
                Dictionary<string, object> attrDict = [];
                var attributes = feat.Attributes;
                if (attributes != null)
                {
                    var attrNames = attributes.GetNames();
                    foreach (var attrName in attrNames)
                    {
                        attrDict[attrName] = attributes[attrName];
                    }
                }

                var attributesJson = Newtonsoft.Json.JsonConvert.SerializeObject(attrDict)
                    ?? _errorMSg;

                var poly = ToPolygonOrNull(feat.Geometry);
                if (poly is null)
                {
                    continue;
                }

                if (poly.SRID == 0)
                {
                    poly.SRID = 4326;
                }

                result.Add((poly, attributesJson));
            }

            return result;
        }

        private static NetTopologySuite.Geometries.Polygon? ToPolygonOrNull(NetTopologySuite.Geometries.Geometry? g)
        {
            if (g is null)
            {
                return null;
            }

            return g switch
            {
                NetTopologySuite.Geometries.Polygon p => p,
                NetTopologySuite.Geometries.MultiPolygon mp when mp.NumGeometries > 0
                    => mp.Geometries.OfType<NetTopologySuite.Geometries.Polygon>().OrderByDescending(x => x.Area).FirstOrDefault(),
                NetTopologySuite.Geometries.GeometryCollection gc when gc.NumGeometries > 0
                    => gc.Geometries.OfType<NetTopologySuite.Geometries.Polygon>().OrderByDescending(x => x.Area).FirstOrDefault(),
                _ => null
            };
        }

        private NetTopologySuite.Geometries.Geometry? ExtractPolygon(SharpKml.Dom.Geometry? sharpGeom)
        {
            if (sharpGeom is SharpKml.Dom.Polygon p)
            {
                return ToNtsPolygon(p);
            }

            if (sharpGeom is MultipleGeometry mg)
            {
                var polys = mg.Geometry?.OfType<SharpKml.Dom.Polygon>()
                               .Select(ToNtsPolygon)
                               .Where(g => g != null)
                               .Cast<NetTopologySuite.Geometries.Polygon>()
                               .ToArray();

                return polys is { Length: > 0 } ? _gf.CreateMultiPolygon(polys) : null;
            }

            return null;
        }

        private static IReadOnlyDictionary<string, string> ExtractAttributes(Placemark pm)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(pm.Name))
            {
                map["name"] = pm.Name!;
            }

            var ext = pm.ExtendedData;
            if (ext != null)
            {
                foreach (var d in ext.Data)
                {
                    if (!string.IsNullOrWhiteSpace(d.Name))
                    {
                        map[d.Name!] = d.Value ?? string.Empty;
                    }
                }

                foreach (var sd in ext.SchemaData)
                {
                    foreach (var s in sd.SimpleData)
                    {
                        if (!string.IsNullOrWhiteSpace(s.Name))
                        {
                            map[s.Name!] = s.Text ?? string.Empty;
                        }
                    }
                }
            }

            return map;
        }


        private NetTopologySuite.Geometries.Polygon? ToNtsPolygon(SharpKml.Dom.Polygon kmlPoly)
        {
            var outer = kmlPoly.OuterBoundary?.LinearRing?.Coordinates;
            if (outer == null)
            {
                return null;
            }

            var outerRing = _gf.CreateLinearRing(ToNts(outer));

            var innerRings = new List<NetTopologySuite.Geometries.LinearRing>();
            if (kmlPoly.InnerBoundary != null)
            {
                foreach (var ib in kmlPoly.InnerBoundary)
                {
                    var coords = ib.LinearRing?.Coordinates;
                    if (coords == null)
                    {
                        continue;
                    }

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
                list.Add(new Coordinate(v.Longitude, v.Latitude));
            }

            if (list.Count > 0 && !list[0].Equals2D(list[^1]))
            {
                list.Add(new Coordinate(list[0].X, list[0].Y));
            }

            return list.ToArray();
        }
    }
}