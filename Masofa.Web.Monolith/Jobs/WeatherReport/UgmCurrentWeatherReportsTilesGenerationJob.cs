using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.WeatherReport;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Ugm;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using Quartz;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace Masofa.Web.Monolith.Jobs.WeatherReport
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "UgmCurrentWeatherReportsTilesGenerationJob", "Ugm")]
    public class UgmCurrentWeatherReportsTilesGenerationJob : BaseJob<UgmCurrentWeatherReportsTilesGenerationJobResult>, IJob
    {
        private MasofaUgmDbContext UgmDbContext { get; }
        private GeoServerService GeoServerService { get; }

        private readonly string source = "Ugm";
        private readonly string type = "Weather";
        private readonly string _outputPath;
        private readonly string _workspace;
        private double lon0, lat0, lon1, lat1, eraPixel, finePixel;
        private int width, height;
        private MultiPolygon? countryMp = null;
        private readonly double minLon, maxLon, minLat, maxLat;

        public UgmCurrentWeatherReportsTilesGenerationJob(
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<UgmCurrentWeatherReportsTilesGenerationJob> logger,
            IConfiguration configuration,
            GeoServerService geoServerService,
            MasofaUgmDbContext ugmDbContext,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            _outputPath = configuration.GetValue<string>("Era5:TilesFolder") ?? string.Empty;
            _workspace = configuration.GetValue<string>("GeoServerOptions:Workspace") ?? string.Empty;
            GeoServerService = geoServerService;
            UgmDbContext = ugmDbContext;

            minLat = configuration.GetValue<double>("CountryBoundaries:LatMin");
            maxLat = configuration.GetValue<double>("CountryBoundaries:LatMax");
            minLon = configuration.GetValue<double>("CountryBoundaries:LonMin");
            maxLon = configuration.GetValue<double>("CountryBoundaries:LonMax");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await GeoServerService.CreateWorkspaceAsync(_workspace);

                countryMp = await Mediator.Send(new GetCountryMultiPolygonCommand());

                var dateToday = DateOnly.FromDateTime(DateTime.UtcNow);
                var data = await UgmDbContext.UgmWeatherData
                    .Where(d => d.Date == dateToday && d.DayPart == DayPart.Day)
                    .ToDictionaryAsync(d => d.RegionId, d => d);

                var stations = await UgmDbContext.UgmWeatherStations
                    .Where(s => data.Keys.Contains(s.UgmRegionId))
                    .ToListAsync();

                if (minLon == 0 && maxLon == 0 && minLat == 0 && maxLat == 0) return;

                eraPixel = 0.25;
                finePixel = 0.025;
                var margin = eraPixel;

                lon0 = Math.Floor((minLon - margin) / finePixel) * finePixel;
                lat0 = Math.Ceiling((maxLat + margin) / finePixel) * finePixel;
                lon1 = Math.Ceiling((maxLon + margin) / finePixel) * finePixel;
                lat1 = Math.Floor((minLat - margin) / finePixel) * finePixel;

                width = (int)Math.Round((lon1 - lon0) / finePixel);
                height = (int)Math.Round((lat0 - lat1) / finePixel);

                double[] gt = [lon0, finePixel, 0, lat0, 0, -finePixel];

                var propName = "TemperatureAverage";

                var options = new[] {
                    "COMPRESS=LZW",
                    "TILED=YES",
                    "ALPHA=YES"
                };
                var drv = Gdal.GetDriverByName("GTiff");
                var formatedDate = $"{dateToday:yyyyMMdd}";
                var storeName = $"{source}{type}{propName}{formatedDate}";
                var currentFileName = $"{storeName}.tif";
                var path = Path.Combine($"{source}{type}{propName}", formatedDate ?? string.Empty);
                var dirPath = Path.Combine(_outputPath, path);
                Directory.CreateDirectory(dirPath);
                var filePath = Path.Combine(dirPath, currentFileName);
                using Dataset ds = drv.Create(filePath, width, height, 4, OSGeo.GDAL.DataType.GDT_Byte, options);
                ds.SetGeoTransform(gt);
                ds.SetProjection("EPSG:4326");
                if (formatedDate != null)
                {
                    var tiffTagDateTime = DateTime.ParseExact(formatedDate, "yyyyMMdd", null).ToString("yyyy:MM:dd hh:mm:ss");
                    ds.SetMetadataItem("TIFFTAG_DATETIME", tiffTagDateTime, "");
                }

                int len = width * height;

                var r = new byte[len];
                var g = new byte[len];
                var b = new byte[len];
                var a = new byte[len];

                List<(double x, double y, double temperature)> peaks = [];

                foreach (var station in stations)
                {
                    var fieldValue = data[station.UgmRegionId].AirTAverage;
                    var lon = station.Longitude;
                    var lat = station.Latitude;

                    if (lon == null || lat == null || fieldValue == null)
                    {
                        continue;
                    }

                    peaks.Add((lon.Value, lat.Value, fieldValue.Value));
                }

                FillTemperatureMap(width, height, peaks, r, g, b, a, 1);

                //foreach (var station in stations)
                //{
                //    var fieldValue = data[station.UgmRegionId].AirTAverage;
                //    var lon = station.Longitude;
                //    var lat = station.Latitude;

                //    if (lon == null || lat == null || fieldValue == null)
                //    {
                //        continue;
                //    }
                //    var colorTable = WeatherReportColors.GetColorTable("temperature");
                //    Color rgb = ValueToColor(fieldValue.Value, colorTable);
                //    DrawCircle(r, g, b, a, lon.Value, lat.Value, 10.0, rgb.R, rgb.G, rgb.B, lon0, lat0, finePixel, width, height, true);
                //}

                //if (countryMp != null)
                //{
                //    DrawContour(r, g, b, a, countryMp);
                //}

                ds.GetRasterBand(1).WriteRaster(0, 0, width, height, r, width, height, 0, 0);
                ds.GetRasterBand(2).WriteRaster(0, 0, width, height, g, width, height, 0, 0);
                ds.GetRasterBand(3).WriteRaster(0, 0, width, height, b, width, height, 0, 0);
                ds.GetRasterBand(4).WriteRaster(0, 0, width, height, a, width, height, 0, 0);

                ds.SetMetadataItem("MIN_LON", lon0.ToString("F2"), "");
                ds.SetMetadataItem("MAX_LON", lon1.ToString("F2"), "");
                ds.SetMetadataItem("MIN_LAT", lat1.ToString("F2"), "");
                ds.SetMetadataItem("MAX_LAT", lat0.ToString("F2"), "");
                ds.SetMetadataItem("PIXEL_SIZE", eraPixel.ToString("F2"), "");
                ds.SetMetadataItem("POINT_COUNT", stations.Count.ToString(), "");

                for (int i = 1; i <= 4; i++)
                {
                    ds.GetRasterBand(i).ComputeStatistics(false, out _, out _, out _, out _, null, null);
                }

                await Mediator.Send(new TileLayerCreateCommand()
                {
                    Indicator = propName,
                    LayerName = storeName,
                    RelativePath = path
                });

                var storeRes = await GeoServerService.RecreateImageMosaicStoreAsync(storeName, path);
                var layerRes = await GeoServerService.PublishCoverageAsync(storeName, storeName);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
            }
        }

        public void FillTemperatureMap(int width, int height, List<(double lon, double lat, double temperature)> geoPeaks, byte[] r, byte[] g, byte[] b, byte[] a, double power = 2.0)
        {
            var pixelPeaks = geoPeaks.Select(p => (
                x: (int)((p.lon - minLon) / (maxLon - minLon) * (width - 1)),
                y: (int)((maxLat - p.lat) / (maxLat - minLat) * (height - 1)),
                temperature: p.temperature
            )).ToList();

            var tempToColorHex = WeatherReportColors.GetColorTable("temperature");
            var sortedKeys = tempToColorHex.Keys.OrderBy(t => t).ToArray();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int done = 0;
            var toCheckMp = countryMp != null;
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    if (toCheckMp)
                    {
                        var (lon, lat) = PxToGeo(x, y);
                        var point = new NetTopologySuite.Geometries.Point(lon, lat);

                        bool mightBeInside = countryMp.EnvelopeInternal.Intersects(point.Coordinate);
                        if (!mightBeInside) continue;

                        var inMp = countryMp.Covers(point);

                        if (!inMp)
                        {
                            continue;
                        }
                    }

                    int idx = y * width + x;

                    double numerator = 0;
                    double denominator = 0;
                    foreach (var (px, py, temp) in pixelPeaks)
                    {
                        double dx = x - px;
                        double dy = y - py;
                        double distSq = dx * dx + dy * dy;
                        if (distSq == 0)
                        {
                            numerator = temp;
                            denominator = 1;
                            break;
                        }
                        double weight = 1.0 / Math.Pow(distSq, power / 2);
                        numerator += weight * temp;
                        denominator += weight;
                    }

                    double interpolatedTemp = numerator / denominator;
                    Color color = ValueToColor(interpolatedTemp, tempToColorHex);

                    r[idx] = color.R;
                    g[idx] = color.G;
                    b[idx] = color.B;
                    a[idx] = 255;
                }

                int current = Interlocked.Increment(ref done);
                if (current % 100 == 0 || current == height)
                {
                    double percent = (double)current / height * 100.0;
                    Console.Write($"\rProgress: {current}/{height} ({percent:F1}%)   ");
                }
            });

            stopwatch.Stop();
            Console.WriteLine("\nFill done!");
            Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds:F2} seconds or {stopwatch.Elapsed.TotalMinutes:F2} minutes");
        }

        (double lon, double lat) PxToGeo(int x, int y) => (lon0 + x * finePixel, lat0 - y * finePixel);

        private void DrawCircle(byte[] dstR, byte[] dstG, byte[] dstB, byte[] alpha, double lonC, double latC, double radiusKm, byte r, byte g, byte b, double lon0, double lat0, double finePixel, int width, int height, bool fill = false)
        {
            double radiusDeg = radiusKm / 111.32;

            if (fill)
            {
                double minLon = lonC - radiusDeg;
                double maxLon = lonC + radiusDeg;
                double minLat = latC - radiusDeg;
                double maxLat = latC + radiusDeg;

                int pxMin = (int)Math.Round((minLon - lon0) / finePixel);
                int pxMax = (int)Math.Round((maxLon - lon0) / finePixel);
                int pyMin = (int)Math.Round((lat0 - maxLat) / finePixel);
                int pyMax = (int)Math.Round((lat0 - minLat) / finePixel);

                pxMin = Math.Max(0, pxMin);
                pxMax = Math.Min(width - 1, pxMax);
                pyMin = Math.Max(0, pyMin);
                pyMax = Math.Min(height - 1, pyMax);

                double r2 = radiusDeg * radiusDeg;

                for (int y = pyMin; y <= pyMax; y++)
                {
                    double lat = lat0 - y * finePixel;
                    for (int x = pxMin; x <= pxMax; x++)
                    {
                        double lon = lon0 + x * finePixel;
                        double dx = lon - lonC;
                        double dy = lat - latC;

                        if (dx * dx + dy * dy <= r2)
                        {
                            int idx = y * width + x;
                            dstR[idx] = r;
                            dstG[idx] = g;
                            dstB[idx] = b;
                            alpha[idx] = 255;
                        }
                    }
                }
            }

            int steps = (int)Math.Max(64, radiusDeg / finePixel * 4);
            var circlePts = new List<Coordinate>();
            for (int i = 0; i < steps; i++)
            {
                double angle = 2.0 * Math.PI * i / steps;
                double lon = lonC + radiusDeg * Math.Cos(angle);
                double lat = latC + radiusDeg * Math.Sin(angle);
                circlePts.Add(new Coordinate(lon, lat));
            }

            for (int i = 0; i < circlePts.Count; i++)
            {
                var (lon1, lat1) = (circlePts[i].X, circlePts[i].Y);
                var (lon2, lat2) = (circlePts[(i + 1) % circlePts.Count].X, circlePts[(i + 1) % circlePts.Count].Y);

                DrawCircleLineBresenham(lon1, lat1, lon2, lat2, dstR, dstG, dstB, alpha, lon0, lat0, finePixel, r, g, b, width, height);
            }
        }

        private void DrawCircleLineBresenham(double lon1, double lat1, double lon2, double lat2, byte[] dstR, byte[] dstG, byte[] dstB, byte[] alpha, double lon0, double lat0, double finePixel, byte r, byte g, byte b, int width, int height)
        {
            (int x1, int y1) = GeoToPx(lon1, lat1);
            (int x2, int y2) = GeoToPx(lon2, lat2);

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                {
                    int idx = y1 * width + x1;
                    dstR[idx] = r;
                    dstG[idx] = g;
                    dstB[idx] = b;
                    alpha[idx] = 255;
                }

                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x1 += sx; }
                if (e2 < dx) { err += dx; y1 += sy; }
            }
        }

        (int x, int y) GeoToPx(double lon, double lat) => ((int)Math.Round((lon - lon0) / finePixel), (int)Math.Round((lat0 - lat) / finePixel));
        private void DrawContour(byte[] dstR, byte[] dstG, byte[] dstB, byte[] alpha, MultiPolygon mp, byte lineR = 0, byte lineG = 0, byte lineB = 0)
        {
            foreach (Polygon poly in mp.Geometries)
            {
                DrawPolygonRing(poly.ExteriorRing, dstR, dstG, dstB, alpha, lon0, lat0, finePixel, lineR, lineG, lineB, width, height);

                foreach (var hole in poly.InteriorRings)
                {
                    DrawPolygonRing(hole, dstR, dstG, dstB, alpha, lon0, lat0, finePixel, lineR, lineG, lineB, width, height);
                }
            }
        }

        private void DrawPolygonRing(LineString ring,
                                     byte[] dstR, byte[] dstG, byte[] dstB, byte[] alpha,
                                     double lon0, double lat0, double finePixel,
                                     byte r, byte g, byte b,
                                     int width, int height)
        {
            var pts = ring.CoordinateSequence.ToCoordinateArray();

            for (int i = 0; i < pts.Length - 1; i++)
            {
                var (lon1, lat1) = (pts[i].X, pts[i].Y);
                var (lon2, lat2) = (pts[i + 1].X, pts[i + 1].Y);

                DrawLineBresenham(lon1, lat1, lon2, lat2, dstR, dstG, dstB, alpha, lon0, lat0, finePixel, r, g, b, width, height);
            }

            if (pts.Length > 2)
            {
                var (lonLast, latLast) = (pts[^1].X, pts[^1].Y);
                var (lonFirst, latFirst) = (pts[0].X, pts[0].Y);

                DrawLineBresenham(lonLast, latLast, lonFirst, latFirst, dstR, dstG, dstB, alpha, lon0, lat0, finePixel, r, g, b, width, height);
            }
        }

        private void DrawLineBresenham(double lon1, double lat1, double lon2, double lat2, byte[] dstR, byte[] dstG, byte[] dstB, byte[] alpha, double lon0, double lat0, double finePixel, byte r, byte g, byte b, int width, int height)
        {
            (int x1, int y1) = GeoToPx(lon1, lat1);
            (int x2, int y2) = GeoToPx(lon2, lat2);

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                {
                    int idx = y1 * width + x1;
                    dstR[idx] = r;
                    dstG[idx] = g;
                    dstB[idx] = b;
                    alpha[idx] = 255;
                }

                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x1 += sx; }
                if (e2 < dx) { err += dx; y1 += sy; }
            }
        }

        private static Color ValueToColor(double value, SortedDictionary<double, string>? colorMap)
        {
            if (colorMap is null)
            {
                return Color.Transparent;
            }

            if (value <= colorMap.First().Key)
            {
                return HexToColor(colorMap.First().Value);
            }

            if (value >= colorMap.Last().Key)
            {
                return HexToColor(colorMap.Last().Value);
            }

            var lower = double.MinValue;
            var upper = double.MaxValue;
            foreach (var kv in colorMap)
            {
                if (kv.Key <= value)
                {
                    lower = kv.Key;
                }

                if (kv.Key >= value)
                {
                    upper = kv.Key; break;
                }
            }

            double ratio = (double)((value - lower) / (upper - lower));

            if (double.IsNaN(ratio))
            {
                ratio = 1.0;
            }

            Color c1 = HexToColor(colorMap[lower]);
            Color c2 = HexToColor(colorMap[upper]);

            var result = Color.FromArgb(
                (int)(c1.R + ratio * (c2.R - c1.R)),
                (int)(c1.G + ratio * (c2.G - c1.G)),
                (int)(c1.B + ratio * (c2.B - c1.B))
            );

            return result;
        }

        private static Color HexToColor(string hex) => Color.FromArgb(int.Parse(hex, NumberStyles.HexNumber));
    }

    public class UgmCurrentWeatherReportsTilesGenerationJobResult : BaseJobResult
    {
        public Dictionary<string, List<string>> ReportsCreated { get; set; } = [];
    }
}
