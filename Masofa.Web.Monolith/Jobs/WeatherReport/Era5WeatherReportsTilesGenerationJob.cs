using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.WeatherReport;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using Quartz;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace Masofa.Web.Monolith.Jobs.WeatherReport
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "Era5WeatherReportsTilesGenerationJob", "Era")]
    public class Era5WeatherReportsTilesGenerationJob : BaseJob<Era5WeatherReportsTilesGenerationJobResult>, IJob
    {
        private GdalInitializer GdalInitializer { get; }
        private MasofaEraDbContext EraDbContext { get; set; }
        private GeoServerService GeoServerService { get; set; }

        private readonly string source = "Era";
        private readonly string type = "Weather";
        private readonly string _outputPath;
        private readonly string _workspace;
        private double lon0, lat0, lon1, lat1, eraPixel, finePixel;
        private int width, height, superFactor;
        private Geometry? enlargedPolygon = null;
        private MultiPolygon? countryMp = null;

        public Era5WeatherReportsTilesGenerationJob(IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<Era5WeatherReportsTilesGenerationJob> logger,
            MasofaEraDbContext eraDbContext,
            IConfiguration configuration,
            GeoServerService geoServerService,
            GdalInitializer gdalInitializer,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            EraDbContext = eraDbContext;
            Mediator = mediator;
            GeoServerService = geoServerService;
            _outputPath = configuration.GetValue<string>("Era5:TilesFolder") ?? string.Empty;
            _workspace = configuration.GetValue<string>("GeoServerOptions:Workspace") ?? string.Empty;
            GdalInitializer = gdalInitializer;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await GeoServerService.CreateWorkspaceAsync(_workspace);

                countryMp = await Mediator.Send(new GetCountryMultiPolygonCommand());
                if (countryMp != null)
                {
                    enlargedPolygon = countryMp.Buffer(100.0 / 111.32);
                }

                var dateToday = DateOnly.FromDateTime(DateTime.UtcNow);
                var reportsByDate = await EraDbContext.Era5DayWeatherReports
                     .Where(r => r.Date == dateToday)
                     .GroupBy(r => r.Date)
                     .ToDictionaryAsync(x => x.Key, x => x.ToList());

                var forecastsByDate = await EraDbContext.Era5DayWeatherForecasts
                    .Where(f => f.Date > DateOnly.FromDateTime(DateTime.UtcNow))
                    .GroupBy(r => r.Date)
                    .ToDictionaryAsync(x => x.Key, x => x.ToList());

                var reportStationIds = reportsByDate.Values
                    .SelectMany(list => list)
                    .Select(obj => obj.WeatherStation)
                    .Distinct()
                    .ToList();

                var forecastStationIds = forecastsByDate.Values
                    .SelectMany(list => list)
                    .Select(obj => obj.WeatherStation)
                    .Distinct()
                    .ToList();

                var stationIds = reportStationIds.Concat(forecastStationIds).Distinct().ToList();

                var stations = await EraDbContext.EraWeatherStations
                    .Where(s => stationIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id, s => s.Point);

                var pts = stations.Values.ToList();

                var minLon = pts.Min(p => p.X);
                var maxLon = pts.Max(p => p.X);
                var minLat = pts.Min(p => p.Y);
                var maxLat = pts.Max(p => p.Y);

                eraPixel = 0.25;
                finePixel = 0.0025;
                var margin = eraPixel;

                lon0 = Math.Floor((minLon - margin) / finePixel) * finePixel;
                lat0 = Math.Ceiling((maxLat + margin) / finePixel) * finePixel;
                lon1 = Math.Ceiling((maxLon + margin) / finePixel) * finePixel;
                lat1 = Math.Floor((minLat - margin) / finePixel) * finePixel;


                superFactor = (int)Math.Round(eraPixel / finePixel);

                width = (int)Math.Round((lon1 - lon0) / finePixel);
                height = (int)Math.Round((lat0 - lat1) / finePixel);

                double[] gt = [lon0, finePixel, 0, lat0, 0, -finePixel];

                var valueProperties = typeof(BaseEra5WeatherReport)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                        .ToList();

                foreach (var (reportDate, reports) in reportsByDate)
                {
                    foreach (var prop in valueProperties)
                    {
                        try
                        {
                            await CreateReportTiff(reportDate, prop, width, height, gt, [.. reports.Cast<BaseEra5WeatherReport>()], stations);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogCritical($"!!! CreateReportTiff ERROR: {ex.Message}");
                        }
                    }

                    try
                    {
                        await CreateFrostDangerTiff(reportDate, width, height, gt, reports, stations);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogCritical($"!!! CreateReportTiff ERROR: {ex.Message}");
                    }
                }

                foreach (var (forecastDate, forecasts) in forecastsByDate)
                {
                    foreach (var prop in valueProperties)
                    {
                        try
                        {
                            await CreateReportTiff(forecastDate, prop, width, height, gt, [.. forecasts.Cast<BaseEra5WeatherReport>()], stations);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogCritical(ex.Message);
                        }
                    }

                    try
                    {
                        await CreateFrostDangerTiff(forecastDate, width, height, gt, forecasts, stations);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogCritical(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
            }
        }

        private async Task CreateFrostDangerTiff<TReport>(DateOnly reportDate, int width, int height, double[] gt, List<TReport>? reports, Dictionary<Guid, NetTopologySuite.Geometries.Point>? stations)
            where TReport : BaseEra5WeatherReport, IIsFrostDanger
        {
            #region CreateEmptyTiff
            var propName = "FrostDanger";
            var options = new[] {
                    "COMPRESS=LZW",
                    "TILED=YES",
                    "ALPHA=YES"
                };
            var drv = GdalInitializer.GetDriverByName("GTiff");
            var formatedDate = $"{reportDate:yyyyMMdd}";
            var storeName = $"{source}{type}{propName}{formatedDate}";
            var currentFileName = $"{storeName}.tif";
            var path = Path.Combine($"{source}{type}{propName}", formatedDate ?? string.Empty);
            var dirPath = Path.Combine(_outputPath, path);
            Directory.CreateDirectory(dirPath);
            var filePath = Path.Combine(dirPath, currentFileName);
            using Dataset ds = drv.Create(filePath, width, height, 4, OSGeo.GDAL.DataType.GDT_Byte, options);
            ds.SetGeoTransform(gt);
            ds.SetProjection("EPSG:4326");
            var tiffTagDateTime = DateTime.ParseExact(formatedDate, "yyyyMMdd", null).ToString("yyyy:MM:dd hh:mm:ss");
            ds.SetMetadataItem("TIFFTAG_DATETIME", tiffTagDateTime, "");

            int len = width * height;

            var r = new byte[len];
            var g = new byte[len];
            var b = new byte[len];
            var a = new byte[len];

            Array.Clear(a, 0, a.Length);
            #endregion

            #region InitialFill
            foreach (var report in reports)
            {
                if (stations.TryGetValue(report.WeatherStation.Value, out var stationPoint))
                {
                    var fieldValue = report.IsFrostDanger;
                    var lon = stationPoint.X;
                    var lat = stationPoint.Y;

                    double cellW = lon - eraPixel / 2.0;   // left
                    double cellE = lon + eraPixel / 2.0;   // right
                    double cellS = lat - eraPixel / 2.0;   // bottom
                    double cellN = lat + eraPixel / 2.0;   // top

                    int pxLeft = (int)((cellW - lon0) / finePixel);
                    int pxRight = (int)((cellE - lon0) / finePixel);
                    int pyTop = (int)((lat0 - cellN) / finePixel);
                    int pyBot = (int)((lat0 - cellS) / finePixel);

                    for (int dy = pyTop; dy < pyBot; dy++)
                    {
                        for (int dx = pxLeft; dx < pxRight; dx++)
                        {
                            int idx = dy * width + dx;
                            var colorTable = WeatherReportColors.GetFrostDangerColors();
                            Color rgb = BoolValueToColor(fieldValue, colorTable);
                            r[idx] = rgb.R;
                            g[idx] = rgb.G;
                            b[idx] = rgb.B;
                            a[idx] = 255;
                        }
                    }
                }
            }
            #endregion

            #region ProcessEmpty
            if (enlargedPolygon != null)
            {
                ProcessEmpty(r, g, b, a, enlargedPolygon);
            }
            #endregion

            #region Gradient
            ApplyGradient(width, height, superFactor, 100, r, g, b, a);
            #endregion

            #region DrawContour
            //if (countryMp != null)
            //{
            //    DrawContour(r, g, b, a, countryMp);
            //}
            #endregion

            #region SaveAndPublish

            ds.GetRasterBand(1).WriteRaster(0, 0, width, height, r, width, height, 0, 0);
            ds.GetRasterBand(2).WriteRaster(0, 0, width, height, g, width, height, 0, 0);
            ds.GetRasterBand(3).WriteRaster(0, 0, width, height, b, width, height, 0, 0);
            ds.GetRasterBand(4).WriteRaster(0, 0, width, height, a, width, height, 0, 0);

            ds.SetMetadataItem("MIN_LON", lon0.ToString("F2"), "");
            ds.SetMetadataItem("MAX_LON", lon1.ToString("F2"), "");
            ds.SetMetadataItem("MIN_LAT", lat1.ToString("F2"), "");
            ds.SetMetadataItem("MAX_LAT", lat0.ToString("F2"), "");
            ds.SetMetadataItem("PIXEL_SIZE", eraPixel.ToString("F2"), "");
            ds.SetMetadataItem("POINT_COUNT", stations?.Values.ToList().Count.ToString(), "");

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
            #endregion
        }

        private async Task CreateReportTiff(DateOnly reportDate, PropertyInfo prop, int width, int height, double[] gt, List<BaseEra5WeatherReport>? reports, Dictionary<Guid, NetTopologySuite.Geometries.Point>? stations)
        {
            #region CreateEmptyTiff
            var options = new[] {
                    "COMPRESS=LZW",
                    "TILED=YES",
                    "ALPHA=YES"
                };
            var drv = GdalInitializer.GetDriverByName("GTiff");
            var formatedDate = $"{reportDate:yyyyMMdd}";
            var storeName = $"{source}{type}{prop.Name}{formatedDate}";
            var currentFileName = $"{storeName}.tif";
            var path = Path.Combine($"{source}{type}{prop.Name}", formatedDate ?? string.Empty);
            var dirPath = Path.Combine(_outputPath, path);
            Directory.CreateDirectory(dirPath);
            var filePath = Path.Combine(dirPath, currentFileName);
            using Dataset ds = drv.Create(filePath, width, height, 4, OSGeo.GDAL.DataType.GDT_Byte, options);
            ds.SetGeoTransform(gt);
            ds.SetProjection("EPSG:4326");
            var tiffTagDateTime = DateTime.ParseExact(formatedDate, "yyyyMMdd", null).ToString("yyyy:MM:dd hh:mm:ss");
            ds.SetMetadataItem("TIFFTAG_DATETIME", tiffTagDateTime, "");

            int len = width * height;

            var r = new byte[len];
            var g = new byte[len];
            var b = new byte[len];
            var a = new byte[len];

            Array.Clear(a, 0, a.Length);
            #endregion

            #region InitialFill
            foreach (var report in reports)
            {
                if (stations.TryGetValue(report.WeatherStation.Value, out var stationPoint))
                {
                    var fieldValue = (double)prop.GetValue(report)!;
                    var lon = stationPoint.X;
                    var lat = stationPoint.Y;

                    double cellW = lon - eraPixel / 2.0;   // left
                    double cellE = lon + eraPixel / 2.0;   // right
                    double cellS = lat - eraPixel / 2.0;   // bottom
                    double cellN = lat + eraPixel / 2.0;   // top

                    int pxLeft = (int)((cellW - lon0) / finePixel);
                    int pxRight = (int)((cellE - lon0) / finePixel);
                    int pyTop = (int)((lat0 - cellN) / finePixel);
                    int pyBot = (int)((lat0 - cellS) / finePixel);

                    for (int dy = pyTop; dy < pyBot; dy++)
                    {
                        for (int dx = pxLeft; dx < pxRight; dx++)
                        {
                            int idx = dy * width + dx;
                            var colorTable = WeatherReportColors.GetColorTable(prop.GetCustomAttribute<ReportValueAttribute>()?.ColorTable ?? string.Empty);
                            Color rgb = ValueToColor(fieldValue, colorTable);
                            r[idx] = rgb.R;
                            g[idx] = rgb.G;
                            b[idx] = rgb.B;
                            a[idx] = 255;
                        }
                    }
                }
            }
            #endregion

            #region ProcessEmpty
            if (enlargedPolygon != null)
            {
                ProcessEmpty(r, g, b, a, enlargedPolygon);
            }
            #endregion

            #region Gradient
            ApplyGradient(width, height, superFactor, 100, r, g, b, a);
            #endregion

            #region DrawContour
            //if (countryMp != null)
            //{
            //    DrawContour(r, g, b, a, countryMp);
            //}
            #endregion

            #region SaveAndPublish

            ds.GetRasterBand(1).WriteRaster(0, 0, width, height, r, width, height, 0, 0);
            ds.GetRasterBand(2).WriteRaster(0, 0, width, height, g, width, height, 0, 0);
            ds.GetRasterBand(3).WriteRaster(0, 0, width, height, b, width, height, 0, 0);
            ds.GetRasterBand(4).WriteRaster(0, 0, width, height, a, width, height, 0, 0);

            ds.SetMetadataItem("MIN_LON", lon0.ToString("F2"), "");
            ds.SetMetadataItem("MAX_LON", lon1.ToString("F2"), "");
            ds.SetMetadataItem("MIN_LAT", lat1.ToString("F2"), "");
            ds.SetMetadataItem("MAX_LAT", lat0.ToString("F2"), "");
            ds.SetMetadataItem("PIXEL_SIZE", eraPixel.ToString("F2"), "");
            ds.SetMetadataItem("POINT_COUNT", stations?.Values.ToList().Count.ToString(), "");

            for (int i = 1; i <= 4; i++)
            {
                ds.GetRasterBand(i).ComputeStatistics(false, out _, out _, out _, out _, null, null);
            }

            await Mediator.Send(new TileLayerCreateCommand()
            {
                Indicator = prop.Name,
                LayerName = storeName,
                RelativePath = path
            });

            var storeRes = await GeoServerService.RecreateImageMosaicStoreAsync(storeName, path);
            var layerRes = await GeoServerService.PublishCoverageAsync(storeName, storeName);
            #endregion
        }

        //void DrawContour(byte[] r, byte[] g, byte[] b, byte[] a, MultiPolygon mp)
        //{
        //    var pts = new List<Coordinate>();
        //    foreach (Polygon poly in mp.Geometries)
        //    {
        //        pts.AddRange(poly.ExteriorRing.CoordinateSequence.ToCoordinateArray());

        //        foreach (var hole in poly.InteriorRings)
        //        {
        //            pts.AddRange(hole.CoordinateSequence.ToCoordinateArray());
        //        }
        //    }

        //    DrawLineByCoords(pts, r, g, b, a, lon0, lat0, finePixel, 0, 0, 0);
        //}

        //void DrawLineByCoords(List<Coordinate> coords, byte[] dstR, byte[] dstB, byte[] dstG, byte[] alpha, double lon0, double lat0, double finePixel, byte lineR, byte lineG, byte lineB)
        //{
        //    for (int i = 0; i < coords.Count - 1; i++)
        //    {
        //        var (lon1, lat1) = (coords[i].X, coords[i].Y);
        //        var (lon2, lat2) = (coords[i + 1].X, coords[i + 1].Y);
        //        DrawLineBresenham(lon1, lat1, lon2, lat2, dstR, dstG, dstB, alpha, lon0, lat0, finePixel, lineR, lineG, lineB);
        //    }
        //}

        //void DrawLineBresenham(double lon1, double lat1, double lon2, double lat2, byte[] dstR, byte[] dstG, byte[] dstB, byte[] alpha, double lon0, double lat0, double finePixel, byte r, byte g, byte b)
        //{
        //    int x1 = (int)Math.Round((lon1 - lon0) / finePixel);
        //    int y1 = (int)Math.Round((lat0 - lat1) / finePixel);
        //    int x2 = (int)Math.Round((lon2 - lon0) / finePixel);
        //    int y2 = (int)Math.Round((lat0 - lat2) / finePixel);

        //    int dx = Math.Abs(x2 - x1);
        //    int dy = Math.Abs(y2 - y1);
        //    int sx = x1 < x2 ? 1 : -1;
        //    int sy = y1 < y2 ? 1 : -1;
        //    int err = dx - dy;

        //    while (true)
        //    {
        //        if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < dstB.Length / width)
        //        {
        //            int idx = y1 * width + x1;
        //            dstR[idx] = r;
        //            dstG[idx] = g;
        //            dstB[idx] = b;
        //            alpha[idx] = 255;
        //        }

        //        if (x1 == x2 && y1 == y2) break;
        //        int e2 = 2 * err;
        //        if (e2 > -dy) { err -= dy; x1 += sx; }
        //        if (e2 < dx) { err += dx; y1 += sy; }
        //    }
        //}

        //public void ApplyGradient0(int width, int height, int superFactor, int blendPercent, byte[] r, byte[] g, byte[] b, byte[] a)
        //{
        //    if (blendPercent <= 0)
        //    {
        //        return;
        //    }

        //    int len = width * height;
        //    var outR = new byte[len];
        //    var outG = new byte[len];
        //    var outB = new byte[len];
        //    var outA = new byte[len];

        //    int blendSize = (int)(superFactor * (blendPercent / 100.0));

        //    for (var y = 0; y < height; y += superFactor)
        //    {
        //        int row = y * width;
        //        for (int x = 0; x < width; x += superFactor)
        //        {
        //            int idx = row + x;
        //        }
        //    }
        //}

        void ProcessEmpty(byte[] r, byte[] g, byte[] b, byte[] a, Geometry mp)
        {
            bool isOk;
            do
            {
                isOk = true;
                for (var y = 0; y < height; y += superFactor)
                {
                    int row = y * width;
                    for (int x = 0; x < width; x += superFactor)
                    {
                        int idx = row + x;
                        if (a[idx] == 0)
                        {
                            var (lon, lat) = PxToGeo(x, y);
                            var inMp = mp.Contains(new NetTopologySuite.Geometries.Point(lon, lat));
                            if (inMp)
                            {
                                isOk = false;
                                var northIdx = ((y - superFactor) * width) + x;
                                var southIdx = ((y + superFactor) * width) + x;
                                var westIdx = (y * width) + (x - superFactor);
                                var eastIdx = (y * width) + (x + superFactor);

                                var north = TryGetColor(r, g, b, northIdx);
                                var south = TryGetColor(r, g, b, southIdx);
                                var west = TryGetColor(r, g, b, westIdx);
                                var east = TryGetColor(r, g, b, eastIdx);

                                if ((north.HasColor()) && (south.HasColor()))
                                {
                                    var avg = AverageColor(north.R, north.G, north.B, south.R, south.G, south.B);
                                    FillCell(r, g, b, a, x, y, avg);
                                }
                                else if (north.HasColor())
                                {
                                    FillCell(r, g, b, a, x, y, (north.R, north.G, north.B));
                                }
                                else if (south.HasColor())
                                {
                                    FillCell(r, g, b, a, x, y, (south.R, south.G, south.B));
                                }
                                else if ((west.HasColor()) && (east.HasColor()))
                                {
                                    var avg = AverageColor(west.R, west.G, west.B, east.R, east.G, east.B);
                                    FillCell(r, g, b, a, x, y, avg);
                                }
                                else if (west.HasColor())
                                {
                                    FillCell(r, g, b, a, x, y, (west.R, west.G, west.B));
                                }
                                else if (east.HasColor())
                                {
                                    FillCell(r, g, b, a, x, y, (east.R, east.G, east.B));
                                }
                            }
                        }
                    }
                }
            } while (!isOk);
        }

        private IndexColor TryGetColor(byte[] r, byte[] g, byte[] b, int idx)
        {
            var result = new IndexColor();
            try
            {
                result.R = r[idx];
                result.G = g[idx];
                result.B = b[idx];
            }
            catch { }
            return result;
        }

        private void FillCell(byte[] r, byte[] g, byte[] b, byte[] a, int x, int y, (byte r, byte g, byte b) color)
        {
            var pyTop = y - (superFactor / 2);
            if (pyTop < 0) pyTop = 0;
            var pyBot = y + (superFactor / 2);
            if (pyBot > height) pyBot = height;

            var pxLeft = x - (superFactor / 2);
            if (pxLeft < 0) pxLeft = 0;
            var pxRight = x + (superFactor / 2);
            if (pxRight > width) pxRight = width;
            for (int dy = pyTop; dy < pyBot; dy++)
            {
                for (int dx = pxLeft; dx < pxRight; dx++)
                {
                    int idx1 = dy * width + dx;
                    r[idx1] = color.r;
                    g[idx1] = color.g;
                    b[idx1] = color.b;
                    a[idx1] = 255;
                }
            }
        }

        public void ApplyGradient(int width, int height, int superFactor, int blendPercent, byte[] r, byte[] g, byte[] b, byte[] a)
        {

            if (blendPercent <= 0)
            {
                return;
            }

            int len = width * height;
            var outR = new byte[len];
            var outG = new byte[len];
            var outB = new byte[len];
            var outA = new byte[len];

            int blendSize = (int)(superFactor * (blendPercent / 100.0));
            int cellsX = (width + superFactor - 1) / superFactor;
            int cellsY = (height + superFactor - 1) / superFactor;

            for (int y = superFactor / 2; y < height; y++)
            {
                int cellY = (y - superFactor / 2) / superFactor;
                int localY = (y - superFactor / 2) % superFactor;

                for (int x = superFactor / 2; x < width; x++)
                {
                    int cellX = (x - superFactor / 2) / superFactor;
                    int localX = (x - superFactor / 2) % superFactor;
                    int idx = y * width + x;

                    int cx = cellX * superFactor + superFactor / 2;
                    int cy = cellY * superFactor + superFactor / 2;
                    int centerIdx = cy * width + cx;

                    var baseColor = GetColor(centerIdx, r, g, b, a);

                    bool nearLeft = localX < blendSize;
                    bool nearRight = localX >= superFactor - blendSize;
                    bool nearTop = localY < blendSize;
                    bool nearBottom = localY >= superFactor - blendSize;

                    if (!nearLeft && !nearRight && !nearTop && !nearBottom)
                    {
                        outR[idx] = baseColor.r;
                        outG[idx] = baseColor.g;
                        outB[idx] = baseColor.b;
                        outA[idx] = baseColor.a;
                        continue;
                    }

                    double weightSelf = 1.0;
                    double totalWeight = weightSelf;
                    double rr = baseColor.r * weightSelf;
                    double gg = baseColor.g * weightSelf;
                    double bb = baseColor.b * weightSelf;
                    double aa = baseColor.a * weightSelf;

                    void BlendNeighbor(int nCellX, int nCellY, double distFrac)
                    {
                        if (nCellX < 0 || nCellY < 0 || nCellX >= cellsX || nCellY >= cellsY)
                        {
                            return;
                        }

                        int nCx = nCellX * superFactor + superFactor / 2;
                        int nCy = nCellY * superFactor + superFactor / 2;
                        if (nCx >= width || nCy >= height)
                        {
                            return;
                        }

                        int nIdx = nCy * width + nCx;
                        var nColor = GetColor(nIdx, r, g, b, a);

                        double w = 1.0 - distFrac;
                        w = distFrac;
                        rr += nColor.r * w;
                        gg += nColor.g * w;
                        bb += nColor.b * w;
                        aa += nColor.a * w;
                        totalWeight += w;
                    }

                    if (nearLeft)
                    {
                        double frac = (blendSize - localX) / (double)blendSize;
                        BlendNeighbor(cellX - 1, cellY, frac);
                    }

                    if (nearRight)
                    {
                        double frac = (localX - (superFactor - blendSize - 1)) / (double)blendSize;
                        BlendNeighbor(cellX + 1, cellY, frac);
                    }

                    if (nearTop)
                    {
                        double frac = (blendSize - localY) / (double)blendSize;
                        BlendNeighbor(cellX, cellY - 1, frac);
                    }

                    if (nearBottom)
                    {
                        double frac = (localY - (superFactor - blendSize - 1)) / (double)blendSize;
                        BlendNeighbor(cellX, cellY + 1, frac);
                    }

                    if (nearLeft && nearTop)
                    {
                        double fx = (blendSize - localX) / (double)blendSize;
                        double fy = (blendSize - localY) / (double)blendSize;
                        BlendNeighbor(cellX - 1, cellY - 1, (fx + fy) / 2);
                    }

                    if (nearRight && nearTop)
                    {
                        double fx = (localX - (superFactor - blendSize - 1)) / (double)blendSize;
                        double fy = (blendSize - localY) / (double)blendSize;
                        BlendNeighbor(cellX + 1, cellY - 1, (fx + fy) / 2);
                    }

                    if (nearLeft && nearBottom)
                    {
                        double fx = (blendSize - localX) / (double)blendSize;
                        double fy = (localY - (superFactor - blendSize - 1)) / (double)blendSize;
                        BlendNeighbor(cellX - 1, cellY + 1, (fx + fy) / 2);
                    }

                    if (nearRight && nearBottom)
                    {
                        double fx = (localX - (superFactor - blendSize - 1)) / (double)blendSize;
                        double fy = (localY - (superFactor - blendSize - 1)) / (double)blendSize;
                        BlendNeighbor(cellX + 1, cellY + 1, (fx + fy) / 2);
                    }

                    byte R = (byte)Math.Clamp(rr / totalWeight, 0, 255);
                    byte G = (byte)Math.Clamp(gg / totalWeight, 0, 255);
                    byte B = (byte)Math.Clamp(bb / totalWeight, 0, 255);
                    byte A = (byte)Math.Clamp(aa / totalWeight, 0, 255);

                    outR[idx] = R;
                    outG[idx] = G;
                    outB[idx] = B;
                    outA[idx] = A;
                }
            }

            Array.Copy(outR, r, len);
            Array.Copy(outG, g, len);
            Array.Copy(outB, b, len);
            Array.Copy(outA, a, len);

        }

        private static (byte r, byte g, byte b, byte a) GetColor(int idx, byte[] r, byte[] g, byte[] b, byte[] a)
        {
            if (idx > a.Length - 1)
            {
                return (255, 255, 255, 255);
            }

            if (a[idx] == 0)
            {
                return (255, 255, 255, 255);
            }

            return (r[idx], g[idx], b[idx], a[idx]);
        }

        static (byte r, byte g, byte b) AverageColor(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
        {
            return ((byte)Math.Round((r1 + r2) / 2.0), (byte)Math.Round((g1 + g2) / 2.0), (byte)Math.Round((b1 + b2) / 2.0));
        }

        (double lon, double lat) PxToGeo(int x, int y) => (lon0 + x * finePixel, lat0 - y * finePixel);

        private static Color BoolValueToColor(bool value, SortedDictionary<bool, string>? colorMap)
        {
            if (colorMap is null)
            {
                return Color.Transparent;
            }

            Color calCol = HexToColor(colorMap[value]);
            return Color.FromArgb(calCol.R, calCol.G, calCol.B);
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

    public class Era5WeatherReportsTilesGenerationJobResult : BaseJobResult
    {
        public Dictionary<string, List<string>> ReportsCreated { get; set; } = new Dictionary<string, List<string>>();
    }

    class IndexColor
    {
        public byte R { get; set; } = 0;
        public byte G { get; set; } = 0;
        public byte B { get; set; } = 0;

        public bool HasColor()
        {
            return R != 0 || G != 0 || B != 0;
        }
    }
}