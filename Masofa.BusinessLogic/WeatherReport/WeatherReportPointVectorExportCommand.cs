using MediatR;
using OSGeo.OGR;
using System.IO.Compression;

namespace Masofa.BusinessLogic.WeatherReport
{
    public class WeatherReportPointVectorExportCommand : IRequest<byte[]>
    {
        public NetTopologySuite.Geometries.Point Point { get; set; }
        public List<(string fieldName, double value)> Values { get; set; }
        public required string FileName { get; set; }
        public required string[] Ext { get; set; }
        public required string Driver { get; set; }
    }

    public class WeatherReportPointVectorExportCommandHandler : IRequestHandler<WeatherReportPointVectorExportCommand, byte[]>
    {
        public async Task<byte[]> Handle(WeatherReportPointVectorExportCommand request, CancellationToken cancellationToken)
        {
            var result = await ExportToZipAsync(request, cancellationToken);
            return result;
        }

        private async Task<byte[]> ExportToZipAsync(WeatherReportPointVectorExportCommand request, CancellationToken ct = default)
        {
            using var mem = Ogr.GetDriverByName("Memory").CreateDataSource(":memory:", null);
            using var layer = mem.CreateLayer("data", null, wkbGeometryType.wkbPoint, null);

            foreach (var (fieldName, value) in request.Values)
            {
                using var field = new FieldDefn(fieldName, FieldType.OFTReal);
                layer.CreateField(field, 1);
                using var feat = new Feature(layer.GetLayerDefn());
                feat.SetField(fieldName, value);
                using var geom = new Geometry(wkbGeometryType.wkbPoint);
                geom.SetPoint_2D(0, request.Point.X, request.Point.Y);
                feat.SetGeometry(geom);
                layer.CreateFeature(feat);
            }

            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                string filePath = Path.Combine(tempDir, $"{request.FileName}{request.Ext[0]}");

                using var outDrv = Ogr.GetDriverByName(request.Driver);
                using var outDs = outDrv.CreateDataSource(filePath, null);
                outDs.CopyLayer(layer, "data", null);
                outDs.FlushCache();
                outDs.Dispose();

                using var zipStream = new MemoryStream();
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var extensions = request.Driver == "ESRI Shapefile" ? [".shp", ".shx", ".dbf", ".prj"] : request.Ext;

                    foreach (var ext in extensions)
                    {
                        var src = Path.Combine(tempDir, $"{request.FileName}{ext}");
                        if (File.Exists(src))
                        {
                            var entry = archive.CreateEntry($"{request.FileName}{ext}", CompressionLevel.Optimal);
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
