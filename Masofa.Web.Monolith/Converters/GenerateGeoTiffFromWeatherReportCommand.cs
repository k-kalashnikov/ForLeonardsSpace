using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Era;
using Masofa.Web.Monolith.Services;
using MediatR;
using OSGeo.GDAL;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Reflection;

namespace Masofa.Web.Monolith.Converters
{
    public class GenerateGeoTiffFromWeatherReportCommand : IRequest<HashSet<string>>
    {
        [Required]
        public required NetTopologySuite.Geometries.Point Point { get; set; }

        [Required]
        public required BaseEra5WeatherReport Report { get; set; }

        public string? FormatedDate { get; set; }

        [Required]
        public required string OutputPath { get; set; }
    }

    public class GenerateGeoTiffFromWeatherReportCommandHandler : IRequestHandler<GenerateGeoTiffFromWeatherReportCommand, HashSet<string>>
    {
        private GdalInitializer GdalInitializer { get; set; }
        private GeoServerService GeoServerService { get; set; }
        private ILogger Logger { get; set; }
        private double lon0, lat0, pixel;

        public GenerateGeoTiffFromWeatherReportCommandHandler(ILogger<GenerateGeoTiffFromWeatherReportCommandHandler> logger, GeoServerService geoServerService, GdalInitializer gdalInitializer)
        {
            Logger = logger;
            GeoServerService = geoServerService;
            GdalInitializer = gdalInitializer;
        }

        public async Task<HashSet<string>> Handle(GenerateGeoTiffFromWeatherReportCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                HashSet<string> storeNames = [];
                var lonCenter = request.Point.X;
                var latCenter = request.Point.Y;
                pixel = 0.25;

                var width = 1;
                var height = 1;

                lon0 = lonCenter - pixel / 2.0;
                lat0 = latCenter + pixel / 2.0;

                double[] gt = { lon0, pixel, 0, lat0, 0, -pixel };

                var drv = GdalInitializer.GetDriverByName("GTiff");

                var options = new[] {
                    "COMPRESS=LZW",
                    "TILED=YES",
                    "ALPHA=YES"
                };

                var valueProperties = typeof(BaseEra5WeatherReport)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                    .ToList();

                var fileName = $"{request.FormatedDate}_lon={lonCenter}_lat={latCenter}.tif";
                foreach (var prop in valueProperties)
                {
                    var fieldName = prop.Name;
                    var fieldValue = (double)prop.GetValue(request.Report)!;


                    var currentFileName = $"{prop.Name}_{fileName}";
                    var dirPath = Path.Combine(request.OutputPath, fieldName, request.FormatedDate ?? string.Empty);
                    Directory.CreateDirectory(dirPath);
                    var storeName = $"{prop.Name}_{request.FormatedDate}";
                    await GeoServerService.CreateImageMosaicStoreAsync(storeName, Path.Combine(fieldName, request.FormatedDate ?? string.Empty));
                    storeNames.Add(storeName);
                    var filePath = Path.Combine(dirPath, currentFileName);

                    using Dataset ds = drv.Create(filePath, width, height, 4, OSGeo.GDAL.DataType.GDT_Byte, options);

                    ds.SetGeoTransform(gt);
                    ds.SetProjection("EPSG:4326");

                    var tiffTagDateTime = DateTime.ParseExact(request.FormatedDate, "yyyyMMdd", null).ToString("yyyy:MM:dd hh:mm:ss");

                    ds.SetMetadataItem("TIFFTAG_DATETIME", tiffTagDateTime, "");

                    var r = new byte[width * height];
                    var g = new byte[width * height];
                    var b = new byte[width * height];
                    var a = new byte[width * height];

                    Array.Clear(a, 0, a.Length);

                    int px = Px(lonCenter);
                    int py = Py(latCenter);

                    int idx = py * width + px;
                    var colorTable = WeatherReportColors.GetColorTable(prop.GetCustomAttribute<ReportValueAttribute>()?.ColorTable ?? string.Empty);
                    Color rgb = ValueToColor(fieldValue, colorTable);
                    r[idx] = rgb.R;
                    g[idx] = rgb.G;
                    b[idx] = rgb.B;
                    a[idx] = 255;

                    ds.GetRasterBand(1).WriteRaster(0, 0, width, height, r, width, height, 0, 0);
                    ds.GetRasterBand(2).WriteRaster(0, 0, width, height, g, width, height, 0, 0);
                    ds.GetRasterBand(3).WriteRaster(0, 0, width, height, b, width, height, 0, 0);
                    ds.GetRasterBand(4).WriteRaster(0, 0, width, height, a, width, height, 0, 0);

                    for (int i = 1; i <= 4; i++)
                    {
                        ds.GetRasterBand(i).ComputeStatistics(false, out _, out _, out _, out _, null, null);
                    }
                }

                return storeNames;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }

        private int Px(double lon) => (int)Math.Round((lon - lon0) / pixel);

        private int Py(double lat) => (int)Math.Round((lat0 - lat) / pixel);

        private static Color ValueToColor(double value, SortedDictionary<double, string>? colorMap)
        {
            if (colorMap is null) return Color.Transparent;

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

            var ratio = (double)((value - lower) / (upper - lower));
            Color c1 = HexToColor(colorMap[lower]);
            Color c2 = HexToColor(colorMap[upper]);

            var result = Color.FromArgb(
                (int)(c1.R + ratio * (c2.R - c1.R)),
                (int)(c1.G + ratio * (c2.G - c1.G)),
                (int)(c1.B + ratio * (c2.B - c1.B))
            );

            return result;
        }

        private static Color HexToColor(string hex) =>
            Color.FromArgb(int.Parse(hex, System.Globalization.NumberStyles.HexNumber));
    }
}
