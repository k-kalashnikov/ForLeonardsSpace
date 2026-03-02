using MediatR;
using OSGeo.OGR;
using System.IO.Compression;

namespace Masofa.BusinessLogic.WeatherReport
{
    public class WeatherReportVectorExportCommand : IRequest<byte[]>
    {
        public required List<(double lon, double lat, double value)> Points { get; set; }
        public required string FileName { get; set; }
        public required string[] Ext { get; set; }
        public required string Driver { get; set; }
        public required string FieldName { get; set; }
    }

    public class WeatherReportVectorExportCommandHandler : IRequestHandler<WeatherReportVectorExportCommand, byte[]>
    {
        public async Task<byte[]> Handle(WeatherReportVectorExportCommand request, CancellationToken cancellationToken)
        {
            var result = await ExportToZipAsync(request.Points, request.FileName, request.Ext, request.Driver, request.FieldName, cancellationToken);
            return result;
        }

        private async Task<byte[]> ExportToZipAsync(IEnumerable<(double lon, double lat, double val)> data,
                                           string fileName,
                                           string[] fileExt,
                                           string driver,
                                           string fieldName = "value",
                                           CancellationToken ct = default)
        {
            using var mem = Ogr.GetDriverByName("Memory").CreateDataSource(":memory:", null);
            using var layer = mem.CreateLayer("data", null, wkbGeometryType.wkbPoint, null);
            using var field = new FieldDefn(fieldName, FieldType.OFTReal);
            layer.CreateField(field, 1);

            foreach (var (lon, lat, val) in data)
            {
                using var feat = new Feature(layer.GetLayerDefn());
                feat.SetField(fieldName, val);
                using var geom = new Geometry(wkbGeometryType.wkbPoint);
                geom.SetPoint_2D(0, lon, lat);
                feat.SetGeometry(geom);
                layer.CreateFeature(feat);
            }

            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                string filePath = Path.Combine(tempDir, $"{fileName}{fileExt[0]}");

                using var outDrv = Ogr.GetDriverByName(driver);
                using var outDs = outDrv.CreateDataSource(filePath, null);
                outDs.CopyLayer(layer, "data", null);
                outDs.FlushCache();
                outDs.Dispose();

                using var zipStream = new MemoryStream();
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var extensions = driver == "ESRI Shapefile" ? [".shp", ".shx", ".dbf", ".prj"] : fileExt;

                    foreach (var ext in extensions)
                    {
                        var src = Path.Combine(tempDir, $"{fileName}{ext}");
                        if (File.Exists(src))
                        {
                            var entry = archive.CreateEntry($"{fileName}{ext}", CompressionLevel.Optimal);
                            using var fs = File.OpenRead(src);
                            using var es = entry.Open();
                            fs.CopyTo(es);
                        }
                    }
                }

                zipStream.Position = 0;
                return zipStream.ToArray();
            }
            finally
            {
                foreach (var ext in new[] { "", ".shp", ".shx", ".dbf", ".prj" })
                {
                    File.Delete(Path.Combine(tempDir, "export" + ext));
                }
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }
}
