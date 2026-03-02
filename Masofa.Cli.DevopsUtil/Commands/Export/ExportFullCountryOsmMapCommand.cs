using Masofa.Common.Attributes;
using Masofa.Common.Models.SystemCrical;
using MaxRev.Gdal.Core;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace Masofa.Cli.DevopsUtil.Commands.Export
{
    public class ExportFullCountryOsmMapCommandParameters
    {
        [TaskParameter("Путь к папке сохранения", false, "")]
        public string DestinationDir { get; set; } = "";

        public double LatMin { get; set; } = 37.00;
        public double LatMax { get; set; } = 45.50;
        public double LonMin { get; set; } = 56.00;
        public double LonMax { get; set; } = 73.00;

        public int Zoom { get; set; } = 13;

        public string GeoServerUrl { get; set; } = "http://185.100.234.107:20060";
        public string Workspace { get; set; } = "osm";
        public string LayerName { get; set; } = "mosaics";

        public static ExportFullCountryOsmMapCommandParameters Parse(string[] args)
        {
            var parameters = new ExportFullCountryOsmMapCommandParameters();

            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                parameters.DestinationDir = args[0];
            }

            return parameters;
        }

        public static ExportFullCountryOsmMapCommandParameters GetFromUser()
        {
            Console.Write("Введите путь к папке сохранения (или нажмите Enter для ./): ");
            var destinationDir = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(destinationDir))
            {
                destinationDir = "";
            }

            return new ExportFullCountryOsmMapCommandParameters
            {
                DestinationDir = destinationDir
            };
        }
    }

    [BaseCommand("Full Map Export", "Экспорт полной карты OSM", typeof(ExportFullCountryOsmMapCommandParameters))]
    public class ExportFullCountryOsmMapCommand : IBaseCommand
    {
        private static readonly HttpClient Http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public void Dispose() { }

        public async Task Execute()
        {
            var parameters = ExportFullCountryOsmMapCommandParameters.GetFromUser();
            await ExecuteCore(parameters);
        }

        public async Task Execute(string[] args)
        {
            var parameters = ExportFullCountryOsmMapCommandParameters.Parse(args);
            await ExecuteCore(parameters);
        }

        private async Task ExecuteCore(ExportFullCountryOsmMapCommandParameters parameters)
        {
            Console.WriteLine("### Full country OSM map Export");
            GdalBase.ConfigureAll();

            var outTiff = Path.Combine(parameters.DestinationDir, "osmMap.tif");

            var (minX, minY) = LatLonToMeters(parameters.LatMin, parameters.LonMin);
            var (maxX, maxY) = LatLonToMeters(parameters.LatMax, parameters.LonMax);

            var (txMin, tyMin) = MetersToTile(minX, minY, parameters.Zoom);
            var (txMax, tyMax) = MetersToTile(maxX, maxY, parameters.Zoom);
            if (txMin > txMax) (txMin, txMax) = (txMax, txMin);
            if (tyMin > tyMax) (tyMin, tyMax) = (tyMax, tyMin);

            int tilesX = txMax - txMin + 1;
            int tilesY = tyMax - tyMin + 1;
            Console.WriteLine($"Tiles count: {tilesX}x{tilesY}={tilesX * tilesY}");

            var tileDir = Path.Combine(Path.GetTempPath(), "tiles");
            Directory.CreateDirectory(tileDir);
            await DownloadTiles(txMin, txMax, tyMin, tyMax, parameters.Zoom, tileDir, parameters);

            double pixelSize = (maxX - minX) / (tilesX * 256);

            MergeTilesToGeoTiff(txMin, tyMin, tilesX, tilesY, tileDir, outTiff, (minX, maxY), pixelSize);

            Console.WriteLine($"Done: {outTiff}");
            Console.Beep(200, 1000);
        }

        #region Web-Mercator math
        private const double OriginShift = 2 * Math.PI * 6378137 / 2.0;

        private static (double mx, double my) LatLonToMeters(double lat, double lon)
        {
            var mx = lon * OriginShift / 180.0;
            var my = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            my = my * OriginShift / 180.0;
            return (mx, my);
        }

        private static (int tx, int ty) MetersToTile(double mx, double my, int z)
        {
            var n = Math.Pow(2, z);
            var tx = (int)((mx + OriginShift) / (2 * OriginShift) * n);
            var ty = (int)((OriginShift - my) / (2 * OriginShift) * n);
            return (tx, ty);
        }
        #endregion

        #region Скачивание
        private static async Task DownloadTiles(int txMin, int txMax,
                                        int tyMin, int tyMax,
                                        int z, string folder, ExportFullCountryOsmMapCommandParameters parameters)
        {
            int total = (txMax - txMin + 1) * (tyMax - tyMin + 1);
            int done = 0;

            const int barWidth = 40;

            void DrawBar()
            {
                double fraction = (double)done / total;
                int whole = (int)(fraction * barWidth);
                var bar = new string('█', whole);
                Console.Write($"\r[{bar.PadRight(barWidth)}]  {done,6} / {total,6}  ({fraction,5:0.0 %})");
            }

            DrawBar();

            for (int y = tyMin; y <= tyMax; y++)
            {
                for (int x = txMin; x <= txMax; x++)
                {
                    var tileFile = Path.Combine(folder, $"{x}_{y}.png");
                    if (File.Exists(tileFile))
                    {
                        done++;
                        DrawBar();
                        continue;
                    }

                    var url = $"{parameters.GeoServerUrl}/geoserver/gwc/service/wmts/rest/" +
                              $"{parameters.Workspace}:{parameters.LayerName}/raster/EPSG:900913/EPSG:900913:{z}/{y}/{x}?format=image/png";

                    try
                    {
                        var bytes = await Http.GetByteArrayAsync(url);
                        await File.WriteAllBytesAsync(tileFile, bytes);

                        done++;
                        DrawBar();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError: {x},{y}: {ex.Message}");
                    }
                }
            }
            Console.WriteLine();
        }
        #endregion

        static void MergeTilesToGeoTiff(int txMin, int tyMin, int tilesX, int tilesY, string tileDir, string outTiff, (double minX, double maxY) topLeftM, double pixelSizeM)
        {
            const int tileSize = 256;
            int imgW = tilesX * tileSize;
            int imgH = tilesY * tileSize;

            var drv = Gdal.GetDriverByName("GTiff");
            using (var dstDs = drv.Create(outTiff, imgW, imgH, 3, DataType.GDT_Byte, new[] { "TILED=YES", "COMPRESS=LZW", "BIGTIFF=YES", "TFW=YES", "INTERLEAVE=PIXEL" }))
            {
                double[] gt = { topLeftM.minX, pixelSizeM, 0, topLeftM.maxY, 0, -pixelSizeM };
                dstDs.SetGeoTransform(gt);

                var srs = new SpatialReference("");
                srs.ImportFromEPSG(3857);
                srs.ExportToWkt(out var wkt, null);
                dstDs.SetProjection(wkt);

                for (int dy = 0; dy < tilesY; dy++)
                {
                    for (int dx = 0; dx < tilesX; dx++)
                    {
                        string tileFile = Path.Combine(tileDir, $"{txMin + dx}_{tyMin + dy}.png");
                        if (!File.Exists(tileFile)) continue;

                        using var srcDs = Gdal.Open(tileFile, Access.GA_ReadOnly);
                        int srcBands = srcDs.RasterCount;

                        int px = dx * tileSize;
                        int py = dy * tileSize;

                        for (int b = 0; b < 3; b++)
                        {
                            var band = srcDs.GetRasterBand(Math.Min(b + 1, srcBands));
                            byte[] buf = new byte[tileSize * tileSize];

                            band.ReadRaster(0, 0, tileSize, tileSize,
                                buf, tileSize, tileSize,
                                1, tileSize);

                            dstDs.GetRasterBand(b + 1)
                                .WriteRaster(px, py, tileSize, tileSize,
                                    buf, tileSize, tileSize,
                                    1, tileSize);
                        }
                    }
                    Console.Write($"\rDone {dy + 1}/{tilesY} lines");
                }
                Console.WriteLine();

                dstDs.FlushCache();
            }
        }
    }
}