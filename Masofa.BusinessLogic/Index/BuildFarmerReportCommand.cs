using DocumentFormat.OpenXml.Spreadsheet;
using Masofa.BusinessLogic.Extentions;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common;
using Masofa.BusinessLogic.WeatherReport;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Era;
using Masofa.Common.Models.MasofaAnaliticReport;
using Masofa.Common.Models.Reports;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Npgsql;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using QRCoder;
using RazorLight;
using SkiaSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using MaxRev.Gdal.Core;
using OSGeo.GDAL;
using OSGeo.OGR;
using GdalDataType = OSGeo.GDAL.DataType;
using ImageSharpPngEncoder = SixLabors.ImageSharp.Formats.Png.PngEncoder;
using LineStyle = OxyPlot.LineStyle;
using OSGeo.OSR;
using Field = Masofa.Common.Models.CropMonitoring.Field;

namespace Masofa.BusinessLogic.Index
{
    public class BuildFarmerReportCommand : IRequest<BuildFarmerReportResult>
    {
        public Guid ReportId { get; set; }
        public DateOnly ReportDate { get; set; }
        public string Locale { get; set; } = "ru-RU";
        public bool AlsoPdf { get; set; } = true;
    }


    public class BuildFarmerReportResult
    {
        public string HtmlObjectKey { get; set; } = string.Empty;
        public string? PdfObjectKey { get; set; }
        public string? DebugLocalPath { get; set; }
    }

    public class BuildFarmerReportHandler : IRequestHandler<BuildFarmerReportCommand, BuildFarmerReportResult>
    {
        private IFileStorageProvider FileStorageProvider { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private MasofaWeatherDbContext MasofaWeatherDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaEraDbContext MasofaWeatherReportDbContext { get; set; }
        private MasofaAnaliticReportDbContext MasofaAnaliticReportDbContext { get; set; }
        private MasofaTileDbContext MasofaTileDbContext { get; set; }
        private MasofaSentinelDbContext MasofaSentinelDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private ILogger<BuildFarmerReportHandler> Logger { get; set; }
        private RazorLightEngine RazorLightEngine { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        protected IMediator Mediator { get; set; }
        private GeoServerOptions GeoServerOptions { get; set; }
        private HttpClient HttpClient { get; set; }
        private const double EarthRadiusOnPi = 20037508.34;

        private static bool _gdalInitialized = false;
        private static readonly object _gdalLock = new();

        public BuildFarmerReportHandler(IFileStorageProvider fileStorageProvider, MasofaIndicesDbContext masofaIndicesDbContext, MasofaWeatherDbContext masofaWeatherDbContext, ILogger<BuildFarmerReportHandler> logger, RazorLightEngine razorLightEngine, MasofaCropMonitoringDbContext masofaCropMonitorDbContext, MasofaDictionariesDbContext masofaDictionariesDbContext, IBusinessLogicLogger businessLogicLogger, MasofaEraDbContext masofaWeatherReportDbContext, IMediator mediator, MasofaAnaliticReportDbContext masofaAnaliticReportDbContext, MasofaTileDbContext masofaTileDbContext, MasofaCommonDbContext masofaCommonDbContext, MasofaSentinelDbContext masofaSentinelDbContext, IConfiguration configuration)
        {
            FileStorageProvider = fileStorageProvider;
            MasofaIndicesDbContext = masofaIndicesDbContext;
            MasofaWeatherDbContext = masofaWeatherDbContext;
            Logger = logger;
            RazorLightEngine = razorLightEngine;
            MasofaCropMonitoringDbContext = masofaCropMonitorDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            BusinessLogicLogger = businessLogicLogger;
            MasofaWeatherReportDbContext = masofaWeatherReportDbContext;
            Mediator = mediator;
            MasofaAnaliticReportDbContext = masofaAnaliticReportDbContext;
            MasofaTileDbContext = masofaTileDbContext;
            MasofaCommonDbContext = masofaCommonDbContext;
            MasofaSentinelDbContext = masofaSentinelDbContext;
            GeoServerOptions = configuration.GetSection("GeoServerOptions").Get<GeoServerOptions>();
            HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GeoServer-Tile-Client/1.0");
            
            // Инициализируем GDAL
            InitializeGdal();
        }

        private void InitializeGdal()
        {
            lock (_gdalLock)
            {
                if (_gdalInitialized) return;

                try
                {
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    var rid = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win-x64"
                            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux-x64"
                            : throw new PlatformNotSupportedException();
                    var runtimes = Path.Combine(baseDir, "runtimes", rid, "native");
                    var gdalData = Path.Combine(runtimes, "gdal-data");
                    var projLib = Path.Combine(runtimes, "maxrev.gdal.core.libshared");

                    Environment.SetEnvironmentVariable("GDAL_DATA", gdalData);
                    Environment.SetEnvironmentVariable("PROJ_LIB", projLib);

                    Gdal.AllRegister();

                    _gdalInitialized = true;
                    Logger.LogInformation($"GDAL initialized. GDAL_DATA={gdalData}, PROJ_LIB={projLib}");
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to initialize GDAL, some operations may fail");
                }
            }
        }
        public async Task<BuildFarmerReportResult> Handle(BuildFarmerReportCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var reportEntity = await MasofaAnaliticReportDbContext.FarmerRecomendationReports
                    .FirstAsync(r => r.Id == request.ReportId, cancellationToken);

                var field = await MasofaCropMonitoringDbContext.Fields
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == reportEntity.FieldId, cancellationToken);
                var region = await MasofaDictionariesDbContext.Regions
                    .AsNoTracking()
                    .Where(x => x.Id == field.RegionId)
                    .FirstAsync(cancellationToken);
                var season = await MasofaCropMonitoringDbContext.Seasons
                    .AsNoTracking()
                    .FirstAsync(s => s.Id == reportEntity.SeasonId, cancellationToken);

                var fieldId = reportEntity.FieldId;

                var centroid = field.Polygon?.Centroid;
                var lat = centroid?.Y ?? field.CenterY ?? 0;
                var lng = centroid?.X ?? field.CenterX ?? 0;
                var vm = new CoordinatesAndDateViewModel()
                {
                    Latitude = lat,
                    Longitude = lng,
                    InputDate = DateTime.UtcNow
                };

                var sums = await GetByCoordinatesAndDate(vm);
                var sumVm = new ClimaticSummVm()
                {
                    Date = sums.Date,
                    Temp7 = (double)sums.SumOfActiveTemperaturesBase7,
                    Temp10 = (double)sums.SumOfActiveTemperaturesBase10,
                    Temp12 = (double)sums.SumOfActiveTemperaturesBase12,
                    Temp15 = (double)sums.SumOfActiveTemperaturesBase15,
                    Fallout = (double)sums.SumOfFallout,
                    Radiation = (double)sums.SumOfSolarRadiation,
                };

                var bidResults = await GetBidResults(field.Id, request, season.CropId);

                var bucket = "farmer-reports";

                if (season.CropId == null)
                {
                    throw new InvalidOperationException($"Season {season.Id} has null CropId");
                }

                var crop = await MasofaDictionariesDbContext.Crops
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == season.CropId.Value, cancellationToken);

                var cropName = crop?.Names[request.Locale];


                var localizationFile = reportEntity.LocalizationFile;

                var locale = request.Locale;

                if (!LocalizationFileStorageItem.SupportedLanguages.Contains(locale, StringComparer.OrdinalIgnoreCase))
                {
                    var msg = LogMessageResource.LocaleNotSupported(locale, season.Id.ToString());
                    await BusinessLogicLogger.LogWarningAsync(msg, requestPath);
                }

                var localeForTemplate = locale.Split('-')[0];
                var templateName = $"FarmerReport.{localeForTemplate}.cshtml";

                var header = await LoadHeaderAsync(request, season, fieldId, cancellationToken);
                var soil = await LoadSoilAsync(request, season, fieldId, cancellationToken);
                var cal = await LoadCalendarAsync(request, season, fieldId, cancellationToken);
                var irr = await LoadIrrigationAsync(request, season, cancellationToken);
                var weather = await LoadWeatherAsync(request, season, fieldId, cancellationToken);
                var mon = await LoadMonitoringAsync(request, season, cancellationToken);
                var fert = await LoadFertilizationAsync(request, season, cancellationToken);
                var growth = await LoadGrowthStagesAsync(request, season, cancellationToken);
                //var indices = await LoadIndicesAsync(request, fieldId, season, cancellationToken);
                var growthStageSvg = await LoadGrowthStageSvgAsync(request, season, cancellationToken);
                var ministryLogo = await LoadMinistryLogoSvgAsync("ministrylogo");

                var cropRotation = await LoadCropRotationAsync(request, fieldId, cancellationToken);
                var fertPest = await LoadFertPestAsync(request, season, fieldId, cancellationToken);
                var indices = await LoadIndicesAsync(request, fieldId, season, cancellationToken);

                var tempSvg = MakeLineSvg(weather.TempFact, weather.TempNorm, "Температура (°C)", OxyColors.Orange);
                var rainSvg = MakeLineSvg(weather.RainFact, weather.RainNorm, "Осадки (мм/день)", OxyColors.Orange);
                var solarSvg = MakeLineSvg(weather.SolarFact, weather.SolarNorm, "Солнечная радиация (кДж/см²)", OxyColors.Orange);

                reportEntity.Header = header;
                reportEntity.Soil = soil;
                reportEntity.Calendar = cal;
                reportEntity.Irrigation = irr;
                reportEntity.Weather = weather;
                reportEntity.Monitoring = mon;
                reportEntity.Fertilization = fert;
                reportEntity.GrowthStages = growth;
                reportEntity.Indices = indices;
                reportEntity.CropRotation = cropRotation;
                reportEntity.TempChartSvg = tempSvg;
                reportEntity.RainChartSvg = rainSvg;
                reportEntity.SolarChartSvg = solarSvg;
                reportEntity.QrSvg = MakeQrSvg(header.DeepLink);
                reportEntity.LogoSvgBase64 = GetLogoBase64();
                reportEntity.GrowthStageSvg = Convert.ToBase64String(Encoding.UTF8.GetBytes(growthStageSvg));
                reportEntity.MinistrySvg = Convert.ToBase64String(Encoding.UTF8.GetBytes(ministryLogo));
                reportEntity.Fields = field;
                reportEntity.Seasons = season;
                reportEntity.FieldId = field.Id;
                reportEntity.SeasonId = season.Id;
                reportEntity.ClimaticSumm = sumVm;
                reportEntity.BidResults = bidResults;
                reportEntity.LocalizationFile = localizationFile;
                reportEntity.LastUpdateAt = DateTime.UtcNow;
                reportEntity.FertPest = fertPest;

                reportEntity.AnomalyTable = new AnomalyPhotoTableVm();

                // === Ищем bid с архивом
                var bid = await MasofaCropMonitoringDbContext.Bids
                    .AsNoTracking()
                    .Where(b => b.FieldId == reportEntity.FieldId)
                    .Where(b => b.CropId == season.CropId)
                    .Where(b => b.FileResultId != null)
                    .OrderByDescending(b => b.LastUpdateAt)
                    .FirstOrDefaultAsync(cancellationToken);

                Guid? bidFileId = bid?.FileResultId;

                if (bidFileId == null)
                {
                    Logger.LogWarning("Bid archive not found. FieldId={FieldId}, CropId={CropId}",
                        reportEntity.FieldId, season.CropId);
                }

                FileStorageItem? bidFile = null;
                if (bidFileId != null)
                {
                    bidFile = await MasofaCommonDbContext.FileStorageItems
                        .AsNoTracking()
                        .FirstOrDefaultAsync(f => f.Id == bidFileId.Value, cancellationToken);

                    if (bidFile == null)
                    {
                        Logger.LogWarning("FileStorageItem not found. FileId={FileId}", bidFileId);
                    }
                }

                Stream? zipStream = null;
                if (bidFile != null)
                {
                    zipStream = await FileStorageProvider.GetFileStreamAsync(bidFile);
                    if (zipStream == null)
                    {
                        Logger.LogWarning("Failed to open zip stream from storage. FileId={FileId}", bidFile.Id);
                    }
                }

                // ВАЖНО: zipStream может быть null — BuildAnomalyTableRowsAsync должен это пережить
                await using (zipStream)
                {
                    reportEntity.AnomalyTable.Rows = await BuildAnomalyTableRowsAsync(
                        reportEntity, zipStream, request.Locale, cropName, cancellationToken);
                }

                //// === Сбор anomaly-таблицы
                //reportEntity.AnomalyTable.Rows = await BuildAnomalyTableRowsAsync(reportEntity, zipStream, request.Locale, cropName, cancellationToken);

                string html;
                try
                {
                    html = await RazorLightEngine.CompileRenderAsync(templateName, reportEntity);
                    await BusinessLogicLogger.LogInformationAsync(LogMessageResource.ReportGenerated(locale, templateName), requestPath);
                }
                catch (Exception templateEx)
                {
                    var msg = $"Failed to compile or render template '{templateName}' for locale '{locale}' and season '{season.Id}'. Error: {templateEx}";
                    await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                    throw;
                }

                var objectKey = $"reports/{request.ReportId}/{season.CropId}/{request.ReportDate:yyyyMMdd}/{locale}/report.html";
                var bytes = Encoding.UTF8.GetBytes(html);
                var fileStorageItemId = await FileStorageProvider.PushFileAsync(bytes, objectKey, bucket);

                var fileItem = new FileStorageItem()
                {
                    FileContentType = FileContentType.HtmlFile,
                    FileStoragePath = objectKey,
                    FileStorageBacket = bucket,
                    OwnerTypeFullName = typeof(FarmerRecomendationReport).FullName,
                    Status = StatusType.Active,
                    CreateAt = DateTime.UtcNow,
                    LastUpdateAt = DateTime.UtcNow,
                    OwnerId = reportEntity.Id,
                    FileLength = bytes.Length
                };

                var fileItemEntity = await MasofaCommonDbContext.FileStorageItems.AddAsync(fileItem);
                await MasofaCommonDbContext.SaveChangesAsync();
                var entity = fileItemEntity.Entity;

                localizationFile[locale] = entity.Id;
                reportEntity.LocalizationFile = localizationFile;
                reportEntity.FileStorageItemId = entity.Id;

                MasofaAnaliticReportDbContext.Entry(reportEntity).Property(x => x.LocalizationFile).IsModified = true;
                MasofaAnaliticReportDbContext.Entry(reportEntity).Property(x => x.FileStorageItemId).IsModified = true;

                await MasofaAnaliticReportDbContext.SaveChangesAsync(cancellationToken);

                return new BuildFarmerReportResult
                {
                    HtmlObjectKey = $"{bucket}/{objectKey}",
                    PdfObjectKey = null,
                    DebugLocalPath = null
                };
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }

        }

        private byte[] GeneratePdfFromHtml(string html)
        {
            throw new NotImplementedException();
        }

        #region Helpers
        private string GetLogoBase64()
        {
            //var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "Masofa.Common.Templates.FarmerReportTemplates.masofalogo.svg";

            //using var stream = assembly.GetManifestResourceStream(resourceName);
            //if (stream == null)
            //    throw new InvalidOperationException($"Resource '{resourceName}' not found.");

            //using var reader = new StreamReader(stream);
            //var svgContent = reader.ReadToEnd();
            var svg = "<svg width=\"24\" height=\"24\" viewBox=\"0 0 24 24\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\">\r\n<mask id=\"mask0_74_151915\" style=\"mask-type:alpha\" maskUnits=\"userSpaceOnUse\" x=\"0\" y=\"0\" width=\"24\" height=\"24\">\r\n<circle cx=\"12\" cy=\"12\" r=\"12\" fill=\"#E7E7E7\"/>\r\n</mask>\r\n<g mask=\"url(#mask0_74_151915)\">\r\n<circle cx=\"24\" cy=\"36\" r=\"24\" fill=\"#00C955\"/>\r\n<circle cx=\"24.5\" cy=\"36.5\" r=\"19.5\" fill=\"#92E018\"/>\r\n<circle cy=\"36\" r=\"24\" fill=\"#92E018\"/>\r\n<circle cx=\"0.5\" cy=\"36.5\" r=\"19.5\" fill=\"#00C955\"/>\r\n</g>\r\n<path d=\"M19.2227 2.26855L17.4375 6.61035L21.748 4.81152L24 10.2871L19.6895 12.0859L17.9512 12.7461C17.9815 12.5015 18 12.2528 18 12C18 8.68629 15.3137 6 12 6C8.68629 6 6 8.68629 6 12C6 12.251 6.01694 12.4983 6.04688 12.7412L4.31055 12.0859L0 10.2871L2.25195 4.81152L6.56348 6.61035L4.77734 2.26855L10.2148 0L12 4.3418L13.7861 0L19.2227 2.26855Z\" fill=\"#FDC018\"/>\r\n<path d=\"M14.1213 13.151C15.2929 12.0191 15.2929 10.1838 14.1213 9.05188L11.9998 7L9.87868 9.05188C8.70711 10.1838 8.70711 12.0191 9.87868 13.151C11.0503 14.283 12.9497 14.283 14.1213 13.151Z\" fill=\"#007AFF\" fill-opacity=\"0.5\"/>\r\n</svg>\r\n";

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(svg));
        }

        private async Task<CropRotationVm> LoadCropRotationAsync(BuildFarmerReportCommand req, Guid fieldId, CancellationToken ct)
        {
            var seasons = await MasofaCropMonitoringDbContext.Seasons
                .Where(s => s.FieldId == fieldId)
                .ToListAsync(ct);

            var seasonCropIds = seasons.Select(s => s.CropId).ToList();

            var crops = await MasofaDictionariesDbContext.Crops
                .Where(c => seasonCropIds.Contains(c.Id))
                .ToListAsync();

            var cropRotationVm = new CropRotationVm(); 

            foreach (var crop in crops)
            {
                var season = seasons.FirstOrDefault(s => s.CropId == crop.Id);

                if(season == null)
                {
                    continue;
                }

                var cropRotationRow = new CropRotationRow
                {
                    CropName = crop.Names[$"{req.Locale}"] ?? "Неизвестно",
                    PlannedSowingDate = season.PlantingDatePlan,
                    ActualSowingDate = season.PlantingDate,
                    AreaHa = season.FieldArea ?? 0,
                    PlannedHarvestDate = season.HarvestingDatePlan,
                    ActualHarvestDate = season.HarvestingDate,
                    YieldPerHectare = season.YieldHaFact ?? 0
                };

                cropRotationVm.Rows.Add(cropRotationRow);
            }

            return cropRotationVm;
        }

        private async Task<FertilizersPesticidesVm> LoadFertPestAsync(BuildFarmerReportCommand req, Season season, Guid fieldId, CancellationToken ct)
        {
            var crop = await MasofaDictionariesDbContext.Crops
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == season.CropId, ct);

            var fieldAgroOperations = await MasofaCropMonitoringDbContext.FieldAgroOperations
                .AsNoTracking()
                .Where(f => f.FieldId == fieldId)
                .OrderBy(f => f.OriginalDate)
                .ToListAsync(ct);

            var fertilizers = await MasofaDictionariesDbContext.Fertilizers
                .AsNoTracking()
                .ToListAsync();

            var fertPestVm = new FertilizersPesticidesVm();

            if (fieldAgroOperations is null)
            {
                return fertPestVm;
            }

            foreach (var fieldAgroOperation in fieldAgroOperations)
            {
                var agroOperationParam = ParseAgroParams(fieldAgroOperation.AgroOperationParamsJson);

                if (agroOperationParam is null)
                    continue;

                var fert = fertilizers.FirstOrDefault(f => f.Id == fieldAgroOperation.OperationId);

                var fertPestRow = new FertilizersPesticidesRows();

                if (fert is null)
                {
                    fertPestRow.Crop = crop.Names[$"{req.Locale}"] ?? "-";
                    fertPestRow.Date = DateOnly.FromDateTime(fieldAgroOperation.OriginalDate).AddDays(1);
                    fertPestRow.Type = "-";
                    fertPestRow.Name = agroOperationParam.Name ?? "-";
                    fertPestRow.Quantity = agroOperationParam.Quantity;
                    fertPestRow.UnitOfMeasurement = agroOperationParam.Unit;
                }
                else
                {
                    fertPestRow.Crop = crop.Names[$"{req.Locale}"] ?? "-";
                    fertPestRow.Date = DateOnly.FromDateTime(fieldAgroOperation.OriginalDate).AddDays(1);
                    fertPestRow.Type = fert.Names[$"{req.Locale}"] ?? "-";
                    fertPestRow.Name = agroOperationParam.Name ?? "-";
                    fertPestRow.Quantity = agroOperationParam.Quantity;
                    fertPestRow.UnitOfMeasurement = agroOperationParam.Unit;
                }

                fertPestVm.Rows.Add(fertPestRow);
            }

            return fertPestVm;
        }

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static AgroOperationParamsDto? ParseAgroParams(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<AgroOperationParamsDto>(json, _jsonOpts);
            }
            catch
            {
                return null;
            }
        }

        public sealed class AgroOperationParamsDto
        {
            [JsonPropertyName("quantity")]
            public string? QuantityRaw { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            [System.Text.Json.Serialization.JsonIgnore]
            public decimal? Quantity =>
                decimal.TryParse(QuantityRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;

            [JsonPropertyName("unit")]
            public string? Unit { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("cropId")]
            public Guid? CropId { get; set; }
        }

        private async Task<string> LoadGrowthStageSvgAsync(BuildFarmerReportCommand req, Season season, CancellationToken ct)
        {
            var crop = await MasofaDictionariesDbContext.Crops
                .Where(c => c.Id == season.CropId)
                .FirstOrDefaultAsync(ct);

            if (crop == null)
                throw new InvalidOperationException($"Crop with ID {season.CropId} not found.");

            var cropNameRu = crop.Names["ru-RU"]?.ToLowerInvariant() ?? "unknown";

            var assembly = Assembly.GetExecutingAssembly();
            var svgPath = "Masofa.Common.Templates.FarmerReportTemplates.";

            return cropNameRu switch
            {
                "хлопчатник" or "хлопок" => GetSvg("хлопчатник", assembly, svgPath),
                "кукуруза" => GetSvg("кукуруза", assembly, svgPath),
                "пшеница" => GetSvg("пшеница", assembly, svgPath),
                "подсолнух" => GetSvg("подсолнух", assembly, svgPath),
                "подсолнечник" => GetSvg("подсолнух", assembly, svgPath),
                "рис" => GetSvg("рис", assembly, svgPath),
                "арбуз" => GetSvg("арбуз", assembly, svgPath),
                "виноград" => GetSvg("виноград", assembly, svgPath),
                "гранат" => GetSvg("гранат", assembly, svgPath),
                "капуста" => GetSvg("капуста", assembly, svgPath),
                "картофель" => GetSvg("картофель", assembly, svgPath),
                "огурцы" => GetSvg("огурцы", assembly, svgPath),
                "лук" => GetSvg("лук", assembly, svgPath),
                "маш" => GetSvg("маш", assembly, svgPath),
                "нут" => GetSvg("нут", assembly, svgPath),
                "морковь" => GetSvg("морковь", assembly, svgPath),
                "орех" => GetSvg("орех", assembly, svgPath),
                "перец острый" => GetSvg("перец острый", assembly, svgPath),
                "соя" => GetSvg("соя", assembly, svgPath),
                "томат" => GetSvg("томат", assembly, svgPath),
                "яблоня" => GetSvg("яблоня", assembly, svgPath),
                _ => GetSvg("default_growth_stages", assembly, svgPath)
            };
        }

        static string GetSvg(string name, Assembly? assembly, string? svgPath)
        {
            var resourceName = Path.Combine(AppContext.BaseDirectory, "Templates\\FarmerReportTemplates", $"{name}.svg");
            if (!File.Exists(resourceName))
            {
                return string.Empty;
            }
            return File.ReadAllText(resourceName);
        }

        private async Task<string> LoadMinistryLogoSvgAsync(string name)
        {
            var resourceName = Path.Combine(AppContext.BaseDirectory, "Templates\\FarmerReportTemplates", $"{name}.svg");
            if (!File.Exists(resourceName))
            {
                return string.Empty;
            }
            return File.ReadAllText(resourceName);
        }

        private async Task<HeaderVm> LoadHeaderAsync(BuildFarmerReportCommand req, Season season, Guid FieldId, CancellationToken ct)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var field = await MasofaCropMonitoringDbContext.Fields
                .AsNoTracking()
                .Where(x => x.Id == FieldId)
                .FirstAsync(ct);

                var region = await MasofaDictionariesDbContext.Regions
                    .AsNoTracking()
                    .Where(x => x.Id == field.RegionId)
                    .Select(x => new { x.Names })
                    .FirstAsync(ct);

                var seasons = await MasofaCropMonitoringDbContext.Seasons
                    .AsNoTracking()
                    .Where(s => s.FieldId == FieldId)
                    .ToListAsync();

                var soilType = await MasofaDictionariesDbContext.SoilTypes.FirstOrDefaultAsync(s => s.Id == field.SoilTypeId);

                var cropId = new List<Guid>();
                var crop = new Crop();
                string cropName = "null";


                if (season.CropId != null)
                {
                    cropId.Add((Guid)season.CropId);

                    crop = await MasofaDictionariesDbContext.Crops
                    .AsNoTracking()
                    .Where(c => c.Id == cropId.First())
                    .FirstAsync(ct);

                    cropName = crop.Names["ru-RU"];
                }


                var regionName = region.Names["ru-RU"];
                if (string.IsNullOrEmpty(regionName))
                {
                    regionName = "null";
                }

                var regionRay = await MasofaDictionariesDbContext.Regions
                    .Where(r => r.ParentId == field.RegionId)
                    .Where(l => l.Level == 2)
                    .FirstAsync();

                var regionRayName = regionRay?.Names[$"{req.Locale}"];
                if (string.IsNullOrEmpty(regionRayName))
                {
                    regionRayName = "TEST";
                }

                var soilTypeName = soilType?.Names[$"{req.Locale}"] ?? "-";

                return new HeaderVm
                {
                    FieldId = FieldId,
                    SoilType = soilTypeName,
                    Region = regionName,
                    District = regionRayName,
                    CropName = cropName,
                    CadastralNumber = field.ExternalId,
                    FieldIdText = field.Name,
                    Wgs84 = (field.CenterX, field.CenterY),
                    AreaHa = field.FieldArea,
                    ReportNumber = "№1-025",
                    ReportDate = req.ReportDate
                };
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }

        private async Task<SoilVm> LoadSoilAsync(BuildFarmerReportCommand req, Season season, Guid FieldId, CancellationToken ct)
        {
            var field = await MasofaCropMonitoringDbContext.Fields
                .AsNoTracking()
                .Where(x => x.Id == FieldId)
                .Select(x => new { x.Id, x.SoilTypeId, x.CenterX, x.CenterY })
                .FirstAsync(ct);

            var pt = new NetTopologySuite.Geometries.Point((double)field.CenterX, (double)field.CenterY) { SRID = 4326 };

            var soilType = new SoilType();
            if (field.SoilTypeId != null)
            {
                soilType = await MasofaDictionariesDbContext.SoilTypes
                .AsNoTracking()
                .Where(s => s.Id == field.SoilTypeId)
                .FirstAsync(ct);
            }

            // 3. Вычисляем целевую ячейку
            const double step = 0.25;
            double Snap(double v) => Math.Round(Math.Floor(v / step) * step, 2);
            double targetLon = Snap(pt.X);
            double targetLat = Snap(pt.Y);
            string targetTileKey = $"x_{targetLon}_y_{targetLat}";

            // 4. Генерируем кандидатов: окрестность 3x3, 5x5 или 7x7
            var candidateTileKeys = new List<string>();
            const int radiusSteps = 2; // ±2 шага → 5x5 = 25 ячеек

            for (int dx = -radiusSteps; dx <= radiusSteps; dx++)
            {
                for (int dy = -radiusSteps; dy <= radiusSteps; dy++)
                {
                    double lon = targetLon + dx * step;
                    double lat = targetLat + dy * step;
                    string key = $"x_{lon}_y_{lat}";
                    candidateTileKeys.Add(key);
                }
            }

            // Убираем дубликаты (на всякий случай)
            candidateTileKeys = candidateTileKeys.Distinct().ToList();

            // 5. Находим, какие из этих ячеек содержат данные
            var tileKeysWithData = await MasofaCropMonitoringDbContext.SoilDatas
                .AsNoTracking()
                .Where(s => candidateTileKeys.Contains(s.TileKey))
                .Select(s => s.TileKey)
                .Distinct()
                .ToListAsync(ct);

            if (!tileKeysWithData.Any())
                throw new InvalidOperationException("No soil data found in the vicinity of the field.");

            // 6. Выбираем ближайшую ячейку из тех, где есть данные
            string selectedTileKey = tileKeysWithData
                .Select(tk =>
                {
                    // Парсим TileKey: x_68.5_y_41.5
                    if (tk.StartsWith("x_") && tk.Contains("_y_"))
                    {
                        var parts = tk.Substring(2).Split(new[] { "_y_" }, StringSplitOptions.None);
                        if (parts.Length == 2 &&
                            double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon) &&
                            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat))
                        {
                            double dist = Math.Sqrt(Math.Pow(lon - targetLon, 2) + Math.Pow(lat - targetLat, 2));
                            return new { TileKey = tk, Distance = dist };
                        }
                    }
                    return null;
                })
                .Where(x => x != null)
                .OrderBy(x => x.Distance)
                .First()
                .TileKey;

            // 7. Загружаем данные из выбранной ячейки
            var nearby = await MasofaCropMonitoringDbContext.SoilDatas
                .AsNoTracking()
                .Where(n => n.TileKey == selectedTileKey)
                .OrderBy(s => s.Point.Distance(pt))
                .Take(300)
                .Select(s => new { s.Parameter, s.DepthRange, s.Value, s.Unit, s.Point })
                .ToListAsync(ct);

            var groups = new Dictionary<string, List<(double val, double dist, string? depth)>>(StringComparer.OrdinalIgnoreCase);

            foreach (var r in nearby)
            {
                if (!r.Value.HasValue) continue;
                var rawKey = r.Parameter ?? "";
                if (rawKey.Length == 0) continue;

                var canon = ParamAliases.TryGetValue(rawKey, out var mapped) ? mapped : CanonicalizeFreeFormKey(rawKey);
                var v = NormalizeToTargetUnit(canon, r.Value.Value, r.Unit) ?? r.Value.Value;

                if (!groups.TryGetValue(canon, out var list))
                    groups[canon] = list = new();

                list.Add((v, r.Point.Distance(pt), r.DepthRange));
            }

            double? Resolve(string canonKey)
            {
                if (!groups.TryGetValue(canonKey, out var list) || list.Count == 0) return null;

                foreach (var d in PreferredDepths)
                {
                    var layer = list.Where(x => string.Equals(x.depth, d, StringComparison.OrdinalIgnoreCase))
                                    .Select(x => (x.val, x.dist))
                                    .ToList();
                    var v = Idw(layer);
                    if (v.HasValue) return v;
                }

                return Idw(list.Select(x => (x.val, x.dist)).ToList());
            }

            double? Round1(double? value) => value.HasValue ? Math.Round(value.Value, 1) : null;

            var row = new SoilRow
            {
                AnalysisDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Ph = Round1(Resolve(SoilCanon.Ph)),
                HumusPercent = Round1(Resolve(SoilCanon.Humus)),
                N = Round1(Resolve(SoilCanon.N)),
                P = Round1(Resolve(SoilCanon.P)),
                SandPercent = Round1(Resolve(SoilCanon.Sand)),
                SiltPercent = Round1(Resolve(SoilCanon.Silt)),
                ClayPercent = Round1(Resolve(SoilCanon.Clay)),
                Density = Round1(Resolve(SoilCanon.Density)),
                CEC = Round1(Resolve(SoilCanon.CEC)),
                SOCPercent = Round1(Resolve(SoilCanon.SOC)),
                Salinity = Round1(Resolve(SoilCanon.Salinity)),
                CoarseFragmentsVolPercent = Round1(Resolve(SoilCanon.CFVO))
            };

            if (!row.HumusPercent.HasValue && row.SOCPercent.HasValue)
                row.HumusPercent = Math.Round(row.SOCPercent.Value * 1.724, 3);

            var soilTypeName = soilType?.Names["ru-RU"];

            if (string.IsNullOrEmpty(soilTypeName))
            {
                soilTypeName = "undefined";
            }

            var vm = new SoilVm { SoilType = soilTypeName };
            vm.Rows.Add(row);

            foreach (var k in groups.Keys)
            {
                if (k.Equals(SoilCanon.Sand, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.Silt, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.Clay, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.Ph, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.CEC, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.SOC, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.Density, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.CFVO, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.N, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.Humus, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.P, StringComparison.OrdinalIgnoreCase) ||
                    k.Equals(SoilCanon.Salinity, StringComparison.OrdinalIgnoreCase))
                    continue;

                vm.Extras[k] = Resolve(k);
            }

            return vm;
        }

        private async Task<CalendarVm> LoadCalendarAsync(BuildFarmerReportCommand req, Season season, Guid FieldId, CancellationToken ct)
        {
            if (season.CropId == null)
            {
                return new CalendarVm();
            }

            var field = await MasofaCropMonitoringDbContext.Fields
                .AsNoTracking()
                .Select(f => new { f.Id, f.RegionId })
                .FirstOrDefaultAsync(f => f.Id == FieldId, ct);

            if (field == null)
            {
                return new CalendarVm();
            }

            var measures = await MasofaDictionariesDbContext.AgrotechnicalMeasures
                .AsNoTracking()
                .Where(m => m.CropId == season.CropId)
                .Where(m => m.RegionId == field.RegionId || m.RegionId == null)
                .OrderBy(m => m.DayStart)
                .ToListAsync(ct);

            var completedOperations = await MasofaCropMonitoringDbContext.FieldAgroOperations
                .AsNoTracking()
                .Where(op => op.FieldId == FieldId && op.Status != 0)
                .Select(op => new { op.OperationId, op.OriginalDate })
                .ToListAsync(ct);

            var rows = new List<CalendarRow>();

            foreach (var item in measures)
            {
                var activity = item.Names[req.Locale];
                var notes = item.Descriptions[req.Locale];
                var soilRec = item.SoilRecommendations[req.Locale];

                if (string.IsNullOrWhiteSpace(activity))
                {
                    activity = item.Names["ru-RU"];
                }
                string dayStr = item.DayStart == item.DayEnd
                    ? item.DayStart.ToString()
                    : $"{item.DayStart}-{item.DayEnd}";

                var matchedOps = completedOperations
                    .Where(op => op.OperationId == item.Id)
                    .OrderBy(op => op.OriginalDate)
                    .ToList();

                string actualDateStr = "—";

                if (matchedOps.Any())
                {
                    actualDateStr = string.Join(", ", matchedOps.Select(op => op.OriginalDate.ToString("dd.MM.yyyy")));
                }

                rows.Add(new CalendarRow(
                    dayOffset: dayStr,
                    activity: activity ?? "—",
                    notes: notes ?? "—",
                    rec: soilRec ?? "—",
                    actualDate: actualDateStr
                ));
            }

            var vm = new CalendarVm
            {
                Rows = rows
            };

            return vm;
        }

        private async Task<IrrigationVm> LoadIrrigationAsync(BuildFarmerReportCommand req, Season season, CancellationToken ct)
        {
            var crop = await MasofaDictionariesDbContext.Crops.Where(c => c.Id == season.CropId).FirstAsync(ct);

            var cropName = crop.Names[$"{req.Locale}"];
            if (cropName == null)
            {
                cropName = "Хлопчатник";
            }

            var xlopok = new IrrigationVm
            {
                CropName = "Хлопчатник",
                SprinklingM3Ha = "0",
                DripM3Ha = "2440-4250",
                FurrowM3Ha = "4000-4600"
            };

            var pshenica = new IrrigationVm
            {
                CropName = "Пшеница",
                SprinklingM3Ha = "1150-1400",
                DripM3Ha = "0",
                FurrowM3Ha = "3350-3650"
            };

            var rise = new IrrigationVm
            {
                CropName = "рис",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "10000-25000 (затопление)"
            };
            var podsolnux = new IrrigationVm
            {
                CropName = "подсолнух",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "3450-3700"
            };
            var soya = new IrrigationVm
            {
                CropName = "соя",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "3000-3200"
            };
            var nut = new IrrigationVm
            {
                CropName = "нут",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "1300-1350"
            };
            var mash = new IrrigationVm
            {
                CropName = "маш",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "2350-2450"
            };
            var kapusta = new IrrigationVm
            {
                CropName = "капуста",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "2600-2800 (ранний), \n3250-4200 (поздний)"
            };
            var pomidor = new IrrigationVm
            {
                CropName = "помидор",
                SprinklingM3Ha = "0",
                DripM3Ha = "2500-4500",
                FurrowM3Ha = "5000-7000"
            };
            var ostriyperec = new IrrigationVm
            {
                CropName = "острый перец",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "4000-6000"
            };
            var arbuz = new IrrigationVm
            {
                CropName = "арбуз",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "3600-4200"
            };
            var ogurci = new IrrigationVm
            {
                CropName = "огурцы",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "3600-3900"
            };
            var kartofel = new IrrigationVm
            {
                CropName = "картофель",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "4550-4900 (ранний), \n7150-7700 (поздний)"
            };
            var kukuruza = new IrrigationVm
            {
                CropName = "кукуруза",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "2000–5000"
            };
            var luk = new IrrigationVm
            {
                CropName = "лук",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "4800-5700, \n6300-7400 (туксонбости)"
            };
            var morkov = new IrrigationVm
            {
                CropName = "морковь",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "5050-5800 (ранний), \n3950-4600 (поздний), \n4450-4900 (туксонбости) "
            };
            var vinograd = new IrrigationVm
            {
                CropName = "интен.виноград",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "1400-4500"
            };
            var yablona = new IrrigationVm
            {
                CropName = "яблоня",
                SprinklingM3Ha = "0",
                DripM3Ha = "1200-2400",
                FurrowM3Ha = "2500-5000"
            };
            var granat = new IrrigationVm
            {
                CropName = "гранат",
                SprinklingM3Ha = "0",
                DripM3Ha = "220-275",
                FurrowM3Ha = "1400-4500"
            };
            var orex = new IrrigationVm
            {
                CropName = "орех",
                SprinklingM3Ha = "0",
                DripM3Ha = "0",
                FurrowM3Ha = "3600-5000"
            };

            var normalizedName = cropName.ToLower().Trim();

            var result = normalizedName switch
            {
                "хлопчатник" or "хлопок" => xlopok,
                "пшеница" => pshenica,
                "рис" => rise,
                "подсолнечник" => podsolnux,
                "соя" => soya,
                "нут" => nut,
                "маш" => mash,
                "капуста" => kapusta,
                "помидор" or "томат" => pomidor,
                "острый перец" or "перец" => ostriyperec,
                "арбуз" => arbuz,
                "огурцы" or "огурец" => ogurci,
                "картофель" => kartofel,
                "кукуруза" => kukuruza,
                "лук" => luk,
                "морковь" => morkov,
                "интен.виноград" or "виноград" => vinograd,
                "яблоня" => yablona,
                "гранат" => granat,
                "орех" or "грецкий орех" => orex,
                _ => new IrrigationVm
                {
                    CropName = cropName,
                    SprinklingM3Ha = "0",
                    DripM3Ha = "0",
                    FurrowM3Ha = "3600-5000"
                }
            };

            return result;
        }

        private async Task<WeatherBlockVm> LoadWeatherAsync(BuildFarmerReportCommand req, Season season, Guid FieldId, CancellationToken ct)
        {
            var field = await MasofaCropMonitoringDbContext.Fields
                .Where(f => f.Id == FieldId)
                .FirstOrDefaultAsync(ct);

            if (field == null)
                throw new InvalidOperationException($"Field with ID {FieldId} not found.");

            var region = await MasofaDictionariesDbContext.Regions
                .Where(r => r.Id == field.RegionId)
                .FirstOrDefaultAsync(ct);

            var station = field.Polygon != null && !field.Polygon.IsEmpty
                ? await MasofaWeatherReportDbContext.EraWeatherStations
                    .FromSqlRaw(@"
                        SELECT *
                        FROM ""EraWeatherStations""
                        WHERE ST_Contains(
                            ST_SetSRID(ST_GeomFromText(@polygonWkt), 4326),
                            ST_SetSRID(""Point""::geometry, 4326)
                        )
                        LIMIT 1",
                        new NpgsqlParameter("@polygonWkt", field.Polygon.AsText()))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ct)
                : null;

            if (station == null)
            {
                var centerPoint = new NetTopologySuite.Geometries.Point((double)field.CenterX, (double)field.CenterY)
                {
                    SRID = 4326
                };

                station = await MasofaWeatherReportDbContext.EraWeatherStations
                    .OrderBy(s => s.Point.Distance(centerPoint))
                    .FirstOrDefaultAsync(ct);

                if (station == null)
                    throw new InvalidOperationException("No weather station found, even nearby.");
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // 1. Исторические данные за 4 года
            var startData = today.AddYears(-4);
            var historicalData = await MasofaWeatherReportDbContext.Era5DayWeatherReports
                .AsNoTracking()
                .Where(x => x.WeatherStation == station.Id && x.Date >= startData)
                .OrderBy(x => x.Date)
                .ToListAsync(ct);

            // 2. Нормы
            var norms = await MasofaWeatherReportDbContext.Era5DayNormalizedWeather
                .AsNoTracking()
                .Where(x => x.WeatherStation == station.Id)
                .ToListAsync(ct);

            var normLookup = norms
                .GroupBy(x => new { x.Month, x.Day })
                .ToDictionary(g => g.Key, g => g.First());

            // 3. Прогноз на 7 дней
            var rawForecast = await MasofaWeatherReportDbContext.Era5DayWeatherForecasts
                .Where(x => x.WeatherStation == station.Id && x.Date >= today && x.Date <= today.AddDays(6))
                .OrderBy(x => x.Date)
                .ToListAsync(ct);

            // Заполняем до 7 дней, если данных не хватает
            var fullForecast = new List<Era5DayWeatherForecast>();
            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(i);
                var existing = rawForecast.FirstOrDefault(f => f.Date == date);
                if (existing != null)
                {
                    fullForecast.Add(existing);
                }
            }

            var current = fullForecast.FirstOrDefault() ?? new Era5DayWeatherForecast();

            // 4. Группировка по неделям и расчет данных для графиков (Факт и Норма)
            var weeklyGroups = historicalData
                .GroupBy(x => GetStartOfWeek(x.Date.ToDateTime(TimeOnly.MinValue)))
                .OrderBy(g => g.Key)
                .ToList();

            var tempFact = new List<(DateTime x, double y)>();
            var tempNorm = new List<(DateTime x, double y)>();

            var rainFact = new List<(DateTime x, double y)>();
            var rainNorm = new List<(DateTime x, double y)>();

            var solarFact = new List<(DateTime x, double y)>();
            var solarNorm = new List<(DateTime x, double y)>();

            var tempSeriesDeviation = new List<(DateTime x, double y)>();

            foreach (var weekGroup in weeklyGroups)
            {
                var date = weekGroup.Key;

                var avgTemp = weekGroup.Average(x => x.TemperatureAverage);
                var avgRain = weekGroup.Average(x => x.Fallout);
                var avgSolar = weekGroup.Average(x => x.SolarRadiationInfluence);

                tempFact.Add((date, avgTemp));
                rainFact.Add((date, avgRain));
                solarFact.Add((date, avgSolar));

                var normsInWeek = new List<Era5DayNormalizedWeather>();
                foreach (var dayReport in weekGroup)
                {
                    if (normLookup.TryGetValue(new { dayReport.Date.Month, dayReport.Date.Day }, out var n))
                    {
                        normsInWeek.Add(n);
                    }
                }

                if (normsInWeek.Any())
                {
                    var avgNormTemp = normsInWeek.Average(x => x.TemperatureAverage);

                    tempNorm.Add((date, avgNormTemp));
                    rainNorm.Add((date, normsInWeek.Average(x => x.Fallout)));
                    solarNorm.Add((date, normsInWeek.Average(x => x.SolarRadiationInfluence / 1000.0)));

                    var diff = avgTemp - avgNormTemp;

                    if (Math.Abs(diff) >= 3.0)
                    {
                        tempSeriesDeviation.Add((date, diff));
                    }
                }
            }

            // 5. Преобразуем прогноз в VM
            var next7Days = fullForecast.Select((f, i) => new ForecastDay
            {
                Date = f.Date,
                Icon = f.Fallout > 0.5 ? "rain" : "sun",
                DayTempC = (int)Math.Round(f.TemperatureAverage)
            }).ToList();

            var regionName = region?.Names[$"{req.Locale}"] ?? "Неизвестно";

            return new WeatherBlockVm
            {
                Region = regionName,
                AirTemp = Math.Round(current.TemperatureAverage, 1),
                AirHumidity = Math.Round(current.Humidity, 1),
                PrecipMm = Math.Round(current.Fallout, 1),
                SolarJPerCm2 = Math.Round(current.SolarRadiationInfluence, 1),
                WindSpeed = Math.Round(current.WindSpeed, 1),
                WindDir = ConvertWindDirection(current.WindDerection),
                Next7Days = next7Days,
                TempFact = tempFact,
                TempNorm = tempNorm,
                TempSeriesDeviation = tempSeriesDeviation,
                RainFact = rainFact,
                RainNorm = rainNorm,
                SolarFact = solarFact,
                SolarNorm = solarNorm
            };
        }

        private static string ConvertWindDirection(double degrees)
        {
            var directions = new[] { "С", "СВ", "В", "ЮВ", "Ю", "ЮЗ", "З", "СЗ" };
            return directions[(int)Math.Round(degrees % 360 / 45) % 8];
        }

        private async Task<MonitoringVm> LoadMonitoringAsync(BuildFarmerReportCommand req, Season season, CancellationToken ct)
        {
            // TODO: вычислить отклонения/alerts из твоих данных
            return new MonitoringVm
            {
                WarningText = "В пределах климат нормы или отклоняются (при отклонении выше чем климат норма повышать норму полива)"
            };
        }

        private async Task<FertilizationVm> LoadFertilizationAsync(BuildFarmerReportCommand req, Season season, CancellationToken ct)
        {
            var crop = await MasofaDictionariesDbContext.Crops
             .Where(c => c.Id == season.CropId)
             .FirstOrDefaultAsync(ct);

            if (crop == null)
                throw new InvalidOperationException($"Crop with ID {season.CropId} not found.");

            var cropNameRu = crop.Names[$"{req.Locale}"] ?? "Неизвестно";

            static double ParseNumber(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return 0;

                var parts = value.Split('-');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0].Trim(), out double min) &&
                    double.TryParse(parts[1].Trim(), out double max))
                {
                    return (min + max) / 2;
                }
                else if (double.TryParse(value.Trim(), out double number))
                {
                    return number;
                }

                return 0;
            }

            // 3. Свитч по культуре — возвращаем нормы
            return cropNameRu switch
            {
                "Хлопчатник" or "хлопчатник" => new FertilizationVm
                {
                    Title = "Нормы внесения (переработки) минеральных и органических удобрений",
                    Table = new FertilizationTable
                    {
                        Crop = "Хлопчатник",
                        Variety = "—",
                        Manure = "Навоз",
                        ManureKgHa = 3040,
                        NKgHa = 240,
                        PKgHa = 170,
                        KKgHa = 125,
                        Spring = "100%",
                        BeforeSowing = "—",
                        WithSowing = "—",
                        Topdress1 = "30%",
                        Topdress2 = "30%",
                        Topdress3 = "15–20%"
                    }
                },
                "Кукуруза" or "кукуруза" => new FertilizationVm
                {
                    Title = "Нормы внесения (переработки) минеральных и органических удобрений",
                    Table = new FertilizationTable
                    {
                        Crop = "Кукуруза",
                        Variety = "—",
                        Manure = "Навоз",
                        ManureKgHa = 3000,
                        NKgHa = 150,
                        PKgHa = 80,
                        KKgHa = 100,
                        Spring = "50%",
                        BeforeSowing = "50%",
                        WithSowing = "—",
                        Topdress1 = "20%",
                        Topdress2 = "30%",
                        Topdress3 = "—"
                    }
                },
                "Пшеница" or "пшеница" => new FertilizationVm
                {
                    Title = "Нормы внесения (переработки) минеральных и органических удобрений",
                    Table = new FertilizationTable
                    {
                        Crop = "Пшеница",
                        Variety = "—",
                        Manure = "Навоз",
                        ManureKgHa = 2500,
                        NKgHa = 100,
                        PKgHa = 60,
                        KKgHa = 80,
                        Spring = "100%",
                        BeforeSowing = "—",
                        WithSowing = "—",
                        Topdress1 = "30%",
                        Topdress2 = "20%",
                        Topdress3 = "—"
                    }
                },
                "Соя" or "соя" => new FertilizationVm
                {
                    Title = "Нормы внесения (переработки) минеральных и органических удобрений",
                    Table = new FertilizationTable
                    {
                        Crop = "Соя",
                        Variety = "—",
                        Manure = "Навоз",
                        ManureKgHa = 2500,
                        NKgHa = 100,
                        PKgHa = 60,
                        KKgHa = 80,
                        Spring = "100%",
                        BeforeSowing = "—",
                        WithSowing = "—",
                        Topdress1 = "30%",
                        Topdress2 = "20%",
                        Topdress3 = "—"
                    }
                },
                _ => new FertilizationVm
                {
                    Title = "Нормы внесения (переработки) минеральных и органических удобрений",
                    Table = new FertilizationTable
                    {
                        Crop = cropNameRu,
                        Variety = "—",
                        Manure = "—",
                        ManureKgHa = 0,
                        NKgHa = 0,
                        PKgHa = 0,
                        KKgHa = 0,
                        Spring = "—",
                        BeforeSowing = "—",
                        WithSowing = "—",
                        Topdress1 = "—",
                        Topdress2 = "—",
                        Topdress3 = "—"
                    }
                }
            };
        }

        private async Task<List<AgroOperationRow>> LoadFertilizationOperationsAsync(BuildFarmerReportCommand req, Season season, Guid FieldId, CancellationToken ct)
        {
            var operations = await MasofaCropMonitoringDbContext.FieldAgroOperations
                .Where(o => o.FieldId == FieldId)
                .Where(o => o.AgroOperationParamsType == typeof(Fertilizer))
                .ToListAsync(ct);

            var rows = new List<AgroOperationRow>();

            foreach (var op in operations)
            {
                var param = op.AgroOperationParamsJson != null
                    ? JsonConvert.DeserializeObject<FertilizerOperationParams>(op.AgroOperationParamsJson)
                    : null;

                rows.Add(new AgroOperationRow
                {
                    Date = op.OriginalDate,
                    OperationName = op.OperationName[$"{req.Locale}"] ?? "Неизвестно",
                    FertilizerType = param?.FertilizerType ?? "—",
                    AmountKgHa = param?.AmountKgHa ?? 0,
                    Timing = param?.Timing ?? "—"
                });
            }

            return rows;
        }

        // Пример структуры параметров операции (подстрой под свою модель)
        public class FertilizerOperationParams
        {
            public string? FertilizerType { get; set; }
            public double? AmountKgHa { get; set; }
            public string? Timing { get; set; }
        }

        private async Task<GrowthStagesVm> LoadGrowthStagesAsync(BuildFarmerReportCommand req, Season season, CancellationToken ct)
        {
            var cropPeriod = await MasofaDictionariesDbContext.CropPeriods
                .Where(c => c.CropId == season.CropId)
                .OrderBy(c => c.DayStart)
                .ToListAsync(ct);
            var growthStages = new GrowthStagesVm();
            foreach (var stage in cropPeriod)
            {
                if (stage == null) continue;
                var stageName = stage.Names[$"{req.Locale}"] ?? "Неизвестно";
                string stageDays;
                if (stage.DayStart == stage.DayEnd)
                {
                    stageDays = $"{stage.DayStart}+";
                }
                else
                {
                    stageDays = $"{stage.DayStart}-{stage.DayEnd}";
                }
                var activeTempRange = $"{stage.ActiveTemperatureSumStart} - {stage.ActiveTemperatureSumEnd}";
                var raw = new GrowthStageRow(stageName, stageDays, activeTempRange);
                growthStages.Rows.Add(raw);
            }

            return growthStages;
        }

        private async Task<IndicesVm> LoadIndicesAsync(BuildFarmerReportCommand req, Guid fieldId, Season season, CancellationToken ct)
        {
            var field = await MasofaCropMonitoringDbContext.Fields
                .AsNoTracking()
                .Where(x => x.Id == fieldId)
                .FirstAsync(ct);

            var reportDate = req.ReportDate;
            var reportDateUtc = DateTime.SpecifyKind(reportDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

            var anomalies = await MasofaIndicesDbContext.AnomalyPolygons
                .AsNoTracking()
                .Where(ap => ap.FieldId == fieldId &&
                             ap.SeasonId == season.Id &&
                             ap.OriginalDate.Date == reportDateUtc.Date)
                .ToListAsync(ct);

            var fieldTitle = await GenerateFieldTitleAsync(field, season, req.Locale, ct);
            var (imageDate, satellite) = await GetImageDateAndSatelliteAsync(field, anomalies, reportDate, ct);

            var bbox = field.Polygon.EnvelopeInternal;
            var (minX, minY) = LonLatToWebMercator(bbox.MinX, bbox.MinY);
            var (maxX, maxY) = LonLatToWebMercator(bbox.MaxX, bbox.MaxY);
            int zoom = 14;

            // Все доступные индексы
            var availableIndices = new[] { "NDVI", "EVI", "GNDVI", "MNDWI", "NDWI", "NDMI", "ORVI", "OSAVI", "ARVI" };

            var indexImages = new List<IndexImageVm>();

            foreach (var indexType in availableIndices)
            {
                try
                {
                    // Получаем TIFF байты
                    var tiffBytes = await GetIndexTiffBytesAsync(field, indexType, reportDate, ct);
                    if (tiffBytes == null || tiffBytes.Length == 0)
                    {
                        Logger.LogWarning($"No TIFF data for index {indexType}, field {field.Id}");
                        continue;
                    }

                    // Обрезаем TIFF по полигону поля и конвертируем в PNG
                    var (croppedImageBytes, geoTransform) = ConvertTiffToImage(tiffBytes, field);
                    if (croppedImageBytes == null)
                    {
                        Logger.LogWarning($"Failed to crop TIFF for index {indexType}, field {field.Id}");
                        continue;
                    }

                    // Накладываем аномалии (красные точки-маркеры) - рисуем центром аномалии
                    var imageWithAnomalies = OverlayAnomaliesOnCroppedImage(croppedImageBytes, anomalies, field, geoTransform);

                    // Создаем превью (уменьшаем размер и сжимаем)
                    var previewBytes = CreatePreviewImage(imageWithAnomalies, maxSize: 150, quality: 75);

                    // Сохраняем превью в MinIO
                    var objectKey = $"reports/{fieldId}/{season.Id}/{reportDate:yyyyMMdd}/{indexType.ToLowerInvariant()}-preview.png";
                    var bucket = "farmer-reports";
                    var fileStoragePath = await FileStorageProvider.PushFileAsync(previewBytes, objectKey, bucket);

                    var fileStorageItem = new FileStorageItem
                    {
                        Id = Guid.NewGuid(),
                        FileContentType = FileContentType.ImagePNG,
                        FileStoragePath = fileStoragePath,
                        FileStorageBacket = bucket,
                        OwnerTypeFullName = typeof(IndexImageVm).FullName,
                        Status = StatusType.Active,
                        CreateAt = DateTime.UtcNow,
                        LastUpdateAt = DateTime.UtcNow,
                        OwnerId = Guid.NewGuid(),
                        FileLength = previewBytes.Length
                    };

                    await MasofaCommonDbContext.FileStorageItems.AddAsync(fileStorageItem, ct);

                    // Извлекаем координаты аномалий
                    var anomalyCoordinates = ExtractAnomalyCoordinates(anomalies);

                    var imageBase64 = Convert.ToBase64String(previewBytes);

                    indexImages.Add(new IndexImageVm
                    {
                        IndexType = indexType,
                        IndexName = indexType,
                        ImageFileStorageItemId = fileStorageItem.Id,
                        ImageBase64 = imageBase64,
                        AnomalyCoordinates = anomalyCoordinates
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error processing index {indexType} for field {field.Id}, season {season.Id}. Skipping this index.");
                }
            }

            await MasofaCommonDbContext.SaveChangesAsync(ct);

            var firstImage = indexImages.FirstOrDefault();
            return new IndicesVm
            {
                Title = "Карты с индексами (актуальные, за период)",
                FieldTitle = fieldTitle,
                ImageDate = imageDate,
                Satellite = satellite,
                IndexImages = indexImages
            };
        }

        private static readonly string[] PreferredDepths = { "0-30cm", "0-20cm", "0-10cm", "0-5cm" };


        private static readonly Dictionary<string, string> ParamAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["sand"] = SoilCanon.Sand,
            ["silt"] = SoilCanon.Silt,
            ["clay"] = SoilCanon.Clay,

            ["ph"] = SoilCanon.Ph,
            ["phh2o"] = SoilCanon.Ph,
            ["pH_H2O"] = SoilCanon.Ph,
            ["ph_water"] = SoilCanon.Ph,

            ["cec"] = SoilCanon.CEC,
            ["soc"] = SoilCanon.SOC,
            ["oc"] = SoilCanon.SOC,
            ["organic_carbon"] = SoilCanon.SOC,

            ["bdod"] = SoilCanon.Density,
            ["bulk_density"] = SoilCanon.Density,
            ["density"] = SoilCanon.Density,

            ["cfvo"] = SoilCanon.CFVO,
            ["coarse_fragments"] = SoilCanon.CFVO,

            ["nitrogen"] = SoilCanon.N,
            ["n"] = SoilCanon.N,
            ["total_n"] = SoilCanon.N,
            ["humus"] = SoilCanon.Humus,
            ["om"] = SoilCanon.Humus,
            ["organic_matter"] = SoilCanon.Humus,
            ["phosphorus"] = SoilCanon.P,
            ["p"] = SoilCanon.P,
            ["available_p"] = SoilCanon.P,

            ["salinity"] = SoilCanon.Salinity,
            ["ec"] = SoilCanon.Salinity
        };

        private static double? NormalizeToTargetUnit(string canon, double val, string? unit)
        {
            unit = unit?.Trim().ToLowerInvariant();

            switch (canon)
            {
                case var s when s == SoilCanon.Sand || s == SoilCanon.Silt || s == SoilCanon.Clay
                             || s == SoilCanon.SOC || s == SoilCanon.Humus || s == SoilCanon.CFVO:
                    if (unit is "g/kg" or "gkg") return val / 10.0;
                    if (unit is "%" or "percent" or null) return val;
                    return val;

                case var s when s == SoilCanon.Density:
                    if (unit is "kg/m³" or "kg/m3") return val / 1000.0;
                    if (unit is "g/cm³" or "g/cm3" or null) return val;
                    return val;

                case var s when s == SoilCanon.N || s == SoilCanon.P:
                    if (unit is "mg/kg" or "mgkg" or "ppm" or null) return val;
                    return val;

                case var s when s == SoilCanon.Ph || s == SoilCanon.CEC || s == SoilCanon.Salinity:
                    return val;

                default:
                    return val;
            }
        }

        private static string CanonicalizeFreeFormKey(string raw)
        {
            var s = raw.Trim().ToLowerInvariant();
            s = s.Replace(" ", "_").Replace("-", "_").Replace(".", "_");
            while (s.Contains("__")) s = s.Replace("__", "_");
            return s;
        }

        private static DateTime GetStartOfWeek(DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private static string MakeLineSvg(List<(DateTime x, double y)> factSeries, List<(DateTime x, double y)> normSeries, string title, OxyColor mainColor)
        {
            var model = new PlotModel
            {
                Title = title,
                DefaultFont = "Arial",
                PlotAreaBorderColor = OxyColors.Transparent
            };

            model.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.TopCenter,
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendSymbolMargin = 20,
                LegendItemSpacing = 150,
                LegendPadding = 20,
                LegendBorder = OxyColors.Transparent,
                LegendBackground = OxyColors.Transparent,
                FontSize = 14
            });

            model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd.MM.yyyy",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "Дата (начало недели)"
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            var normLine = new LineSeries
            {
                Title = "Норма (среднее)",
                Color = OxyColors.Red,
                LineStyle = LineStyle.Dash,
                StrokeThickness = 2,
                Decimator = null
            };
            foreach (var (x, y) in normSeries)
                normLine.Points.Add(new DataPoint(DateTimeAxis.ToDouble(x), y));

            model.Series.Add(normLine);

            var factLine = new LineSeries
            {
                Title = "Факт (среднее)",
                Color = mainColor,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 2,
                Decimator = null
            };
            foreach (var (x, y) in factSeries)
                factLine.Points.Add(new DataPoint(DateTimeAxis.ToDouble(x), y));

            model.Series.Add(factLine);

            using var stream = new MemoryStream();
            var exporter = new SvgExporter { Width = 1100, Height = 400, IsDocument = false };
            exporter.Export(model, stream);

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static string MakeQrSvg(string text)
        {
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            return new SvgQRCode(data).GetGraphic(4);
        }

        private static double? Idw(IReadOnlyList<(double val, double dist)> pts)
        {
            const double eps = 1e-9;
            const double p = 2.0;

            if (pts.Count == 0) return null;
            var zero = pts.FirstOrDefault(t => t.dist <= eps);
            if (!zero.Equals(default)) return zero.val;

            double num = 0, den = 0;
            foreach (var (v, d) in pts)
            {
                var w = 1.0 / Math.Pow(d + eps, p);
                num += w * v; den += w;
            }
            return den > 0 ? num / den : (double?)null;
        }

        private async Task<byte[]?> GetIndexTiffBytesAsync(Field field, string indexType, DateOnly date, CancellationToken ct)
        {
            try
            {
                var reportDateUtc = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

                var mapping = await MasofaCropMonitoringDbContext.FieldProductMappings
                    .AsNoTracking()
                    .Where(fpm => fpm.FieldId == field.Id
                        && fpm.Status == StatusType.Active)
                    .OrderByDescending(fpm => fpm.CreateAt)
                    .FirstOrDefaultAsync(ct);

                if (mapping == null)
                {
                    Logger.LogWarning($"No FieldProductMapping found for field {field.Id} and date {date}.");
                    return null;
                }

                var satelliteProduct = await MasofaCommonDbContext.SatelliteProducts
                    .AsNoTracking()
                    .Where(sp => sp.Id == Guid.Parse(mapping.ProductId) && sp.ProductSourceType == mapping.SatelliteType)
                    .FirstOrDefaultAsync(ct);

                if (satelliteProduct == null)
                {
                    Logger.LogWarning($"No SatelliteProduct found for ProductId {mapping.ProductId} and SatelliteType {mapping.SatelliteType}.");
                    return null;
                }

                var bucketName = GetIndexBucketName(mapping.SatelliteType, indexType);
                if (string.IsNullOrEmpty(bucketName))
                {
                    Logger.LogWarning($"Could not determine bucket name for SatelliteType {mapping.SatelliteType} and IndexType {indexType}.");
                    return null;
                }

                var fileStorageItem = await MasofaCommonDbContext.FileStorageItems
                    .AsNoTracking()
                    .Where(fsi => fsi.OwnerId == satelliteProduct.Id
                        && fsi.FileStorageBacket == bucketName
                        && fsi.Status == StatusType.Active
                        && fsi.FileStoragePath.Contains(indexType.ToUpperInvariant()))
                    .FirstOrDefaultAsync(ct);

                if (fileStorageItem == null)
                {
                    Logger.LogWarning($"No FileStorageItem found for SatelliteProduct {satelliteProduct.Id}, bucket {bucketName}, and index {indexType}.");
                    return null;
                }

                byte[]? tiffBytes = null;
                try
                {
                    tiffBytes = await FileStorageProvider.GetFileBytesAsync(fileStorageItem);
                }
                catch (Minio.Exceptions.ObjectNotFoundException ex)
                {
                    Logger.LogWarning(ex, $"MinIO object not found: {fileStorageItem.FileStorageBacket}/{fileStorageItem.FileStoragePath}");
                    return null;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error getting file from MinIO: {fileStorageItem.FileStorageBacket}/{fileStorageItem.FileStoragePath}");
                    return null;
                }

                if (tiffBytes == null || tiffBytes.Length == 0)
                {
                    Logger.LogWarning($"Received empty TIFF bytes for SatelliteProduct {satelliteProduct.Id}, bucket {bucketName}, and index {indexType}.");
                    return null;
                }

                return tiffBytes;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error in GetIndexTiffBytesAsync for field {field.Id}, index {indexType}, date {date}.");
                return null;
            }
        }

        private string GetIndexBucketName(ProductSourceType satelliteType, string indexType)
        {
            var indexLower = indexType.ToLowerInvariant();
            var prefix = satelliteType switch
            {
                ProductSourceType.Sentinel2 => "sentinel",
                ProductSourceType.Landsat => "landsat",
                _ => null
            };

            if (prefix == null)
                return null;

            return $"{prefix}{indexLower}colore";
        }

        private (byte[]? imageBytes, double[]? geoTransform) ConvertTiffToImage(byte[] tiffBytes, Field field)
        {
            try
            {
                // Сначала обрезаем TIFF по полигону поля с помощью GDAL
                var (croppedTiffBytes, geoTransform) = CropTiffByFieldPolygon(tiffBytes, field);
                if (croppedTiffBytes == null)
                {
                    Logger.LogWarning($"Failed to crop TIFF by polygon for field {field.Id}, using original TIFF");
                    croppedTiffBytes = tiffBytes; // Используем оригинал при ошибке
                    // Получаем геотрансформацию из оригинального TIFF
                    string? tempTiffPath = null;
                    try
                    {
                        tempTiffPath = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}.tif");
                        File.WriteAllBytes(tempTiffPath, tiffBytes);
                        using var srcDataset = Gdal.Open(tempTiffPath, Access.GA_ReadOnly);
                        if (srcDataset != null)
                        {
                            geoTransform = new double[6];
                            srcDataset.GetGeoTransform(geoTransform);
                        }
                    }
                    catch
                    {
                        // Игнорируем ошибку
                    }
                    finally
                    {
                        if (tempTiffPath != null && File.Exists(tempTiffPath))
                            File.Delete(tempTiffPath);
                    }
                }

                // Конвертируем обрезанный TIFF в PNG с обработкой NoData значений
                string? tempTiffPath2 = null;
                try
                {
                    tempTiffPath2 = Path.Combine(Path.GetTempPath(), $"tiff_{Guid.NewGuid()}.tif");
                    File.WriteAllBytes(tempTiffPath2, croppedTiffBytes);
                    
                    using var dataset = Gdal.Open(tempTiffPath2, Access.GA_ReadOnly);
                    if (dataset == null)
                    {
                        Logger.LogWarning("Failed to open cropped TIFF for PNG conversion");
                        using var tiffStream = new MemoryStream(croppedTiffBytes);
                        using var fallbackImage = Image.Load<Rgba32>(tiffStream);
                        using var fallbackPngStream = new MemoryStream();
                        fallbackImage.SaveAsPng(fallbackPngStream);
                        fallbackPngStream.Position = 0;
                        return (fallbackPngStream.ToArray(), geoTransform);
                    }

                    int width = dataset.RasterXSize;
                    int height = dataset.RasterYSize;
                    int bandCount = dataset.RasterCount;
                    
                    // Получаем NoData значение
                    var firstBand = dataset.GetRasterBand(1);
                    double noDataValue;
                    int hasNoData;
                    firstBand.GetNoDataValue(out noDataValue, out hasNoData);
                    bool hasNoDataFlag = hasNoData != 0;
                    
                    // Если NoData не установлен, используем 255 (максимальное значение для byte)
                    // чтобы избежать конфликта с цветами палитры (обычно палитра использует 0-254)
                    // Но также проверяем значение 0, так как оно может быть NoData в исходном TIFF
                    double noDataValueToCheck = hasNoDataFlag ? noDataValue : 255.0;

                    // Создаем RGBA изображение
                    using var rgbaImage = new Image<Rgba32>(width, height);
                    
                    // Читаем данные из каждого канала
                    if (bandCount == 1)
                    {
                        // Проверяем, есть ли палитра цветов (color table)
                        var colorTable = firstBand.GetColorTable();
                        bool hasColorTable = colorTable != null && colorTable.GetCount() > 0;
                        
                        var buffer = new byte[width * height];
                        firstBand.ReadRaster(0, 0, width, height, buffer, width, height, 0, 0);
                        
                        if (hasColorTable)
                        {
                            // Используем палитру для преобразования индексов в цвета
                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    int idx = y * width + x;
                                    byte paletteIndex = buffer[idx];
                                    
                                    // Если это NoData значение, делаем прозрачным
                                    // Проверяем как установленное NoData значение, так и 255 (если NoData не установлен)
                                    if ((hasNoDataFlag && Math.Abs(paletteIndex - noDataValue) < 0.1) || 
                                        (!hasNoDataFlag && paletteIndex == 255))
                                    {
                                        rgbaImage[x, y] = new Rgba32(0, 0, 0, 0); // Прозрачный
                                    }
                                    else
                                    {
                                        // Получаем цвет из палитры
                                        // Проверяем, что индекс в пределах палитры
                                        int colorTableCount = colorTable.GetCount();
                                        if (paletteIndex < colorTableCount)
                                        {
                                            var colorEntry = colorTable.GetColorEntry(paletteIndex);
                                            if (colorEntry != null)
                                            {
                                                rgbaImage[x, y] = new Rgba32(
                                                    (byte)colorEntry.c1, // R
                                                    (byte)colorEntry.c2, // G
                                                    (byte)colorEntry.c3, // B
                                                    (byte)colorEntry.c4  // A
                                                );
                                            }
                                            else
                                            {
                                                // Если цвет не найден в палитре, делаем прозрачным
                                                rgbaImage[x, y] = new Rgba32(0, 0, 0, 0);
                                            }
                                        }
                                        else
                                        {
                                            // Индекс вне палитры (например, 255 для NoData) - делаем прозрачным
                                            rgbaImage[x, y] = new Rgba32(0, 0, 0, 0);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Нет палитры - пробуем загрузить через ImageSharp для сохранения цвета
                            // (возможно, это цветное изображение, сохраненное в формате, который GDAL видит как одноканальное)
                            try
                            {
                                using var tiffStreamForColor = new MemoryStream(croppedTiffBytes);
                                using var colorImage = Image.Load<Rgba32>(tiffStreamForColor);
                                
                                // Копируем пиксели из ImageSharp изображения
                                for (int y = 0; y < Math.Min(height, colorImage.Height); y++)
                                {
                                    for (int x = 0; x < Math.Min(width, colorImage.Width); x++)
                                    {
                                        var pixel = colorImage[x, y];
                                        rgbaImage[x, y] = pixel;
                                    }
                                }
                                
                                Logger.LogInformation($"Loaded color image via ImageSharp: {colorImage.Width}x{colorImage.Height}");
                            }
                            catch (Exception ex)
                            {
                                Logger.LogWarning(ex, "Failed to load via ImageSharp, using GDAL grayscale conversion");
                                // Fallback на grayscale через GDAL
                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        int idx = y * width + x;
                                        byte value = buffer[idx];
                                        
                                        // Если это NoData значение, делаем прозрачным
                                        if (hasNoDataFlag && Math.Abs(value - noDataValue) < 0.1)
                                        {
                                            rgbaImage[x, y] = new Rgba32(0, 0, 0, 0); // Прозрачный
                                        }
                                        else
                                        {
                                            // Grayscale -> RGB
                                            rgbaImage[x, y] = new Rgba32(value, value, value, 255);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (bandCount >= 3)
                    {
                        // Многоканальное изображение (RGB или RGBA)
                        var rBuffer = new byte[width * height];
                        var gBuffer = new byte[width * height];
                        var bBuffer = new byte[width * height];
                        byte[]? aBuffer = bandCount >= 4 ? new byte[width * height] : null;
                        
                        dataset.GetRasterBand(1).ReadRaster(0, 0, width, height, rBuffer, width, height, 0, 0);
                        dataset.GetRasterBand(2).ReadRaster(0, 0, width, height, gBuffer, width, height, 0, 0);
                        dataset.GetRasterBand(3).ReadRaster(0, 0, width, height, bBuffer, width, height, 0, 0);
                        if (aBuffer != null && bandCount >= 4)
                        {
                            dataset.GetRasterBand(4).ReadRaster(0, 0, width, height, aBuffer, width, height, 0, 0);
                        }
                        
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                int idx = y * width + x;
                                byte r = rBuffer[idx];
                                byte g = gBuffer[idx];
                                byte b = bBuffer[idx];
                                byte a = aBuffer != null ? aBuffer[idx] : (byte)255;
                                
                                // Если это NoData значение (проверяем по первому каналу), делаем прозрачным
                                // Проверяем как установленное NoData значение, так и 255 (если NoData не установлен)
                                if ((hasNoDataFlag && Math.Abs(r - noDataValue) < 0.1) || 
                                    (!hasNoDataFlag && r == 255))
                                {
                                    rgbaImage[x, y] = new Rgba32(0, 0, 0, 0); // Прозрачный
                                }
                                else
                                {
                                    rgbaImage[x, y] = new Rgba32(r, g, b, a);
                                }
                            }
                        }
                    }
                    
                    using var outputPngStream = new MemoryStream();
                    rgbaImage.SaveAsPng(outputPngStream);
                    outputPngStream.Position = 0;
                    return (outputPngStream.ToArray(), geoTransform);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error converting TIFF to PNG with NoData handling, using simple conversion");
                    // Fallback на простую конвертацию
                    using var tiffStream = new MemoryStream(croppedTiffBytes);
                    using var fallbackImage = Image.Load<Rgba32>(tiffStream);
                    using var fallbackPngStream = new MemoryStream();
                    fallbackImage.SaveAsPng(fallbackPngStream);
                    fallbackPngStream.Position = 0;
                    return (fallbackPngStream.ToArray(), geoTransform);
                }
                finally
                {
                    if (tempTiffPath2 != null && File.Exists(tempTiffPath2))
                        File.Delete(tempTiffPath2);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to convert TIFF to PNG for field {field.Id}");
                return (null, null);
            }
        }

        private (byte[]? tiffBytes, double[]? geoTransform) CropTiffByFieldPolygon(byte[] tiffBytes, Field field)
        {
            if (field.Polygon == null || field.Polygon.IsEmpty)
            {
                // Если полигона нет, возвращаем оригинал и получаем геотрансформацию
                string? tempTiffPathForGeo = null;
                double[]? geoTransform = null;
                try
                {
                    tempTiffPathForGeo = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}.tif");
                    File.WriteAllBytes(tempTiffPathForGeo, tiffBytes);
                    using var srcDataset = Gdal.Open(tempTiffPathForGeo, Access.GA_ReadOnly);
                    if (srcDataset != null)
                    {
                        geoTransform = new double[6];
                        srcDataset.GetGeoTransform(geoTransform);
                    }
                }
                catch
                {
                    // Игнорируем ошибку
                }
                finally
                {
                    if (tempTiffPathForGeo != null && File.Exists(tempTiffPathForGeo))
                        File.Delete(tempTiffPathForGeo);
                }
                return (tiffBytes, geoTransform);
            }

            string? tempTiffPath = null;
            string? tempOutputPath = null;

            try
            {
                // Создаем временные файлы
                tempTiffPath = Path.Combine(Path.GetTempPath(), $"input_{Guid.NewGuid()}.tif");
                tempOutputPath = Path.Combine(Path.GetTempPath(), $"output_{Guid.NewGuid()}.tif");

                // Сохраняем TIFF во временный файл
                File.WriteAllBytes(tempTiffPath, tiffBytes);

                // Открываем исходный TIFF
                using var srcDataset = Gdal.Open(tempTiffPath, Access.GA_ReadOnly);
                if (srcDataset == null)
                {
                    Logger.LogWarning($"Failed to open TIFF file: {tempTiffPath}");
                    return (null, null);
                }

                // Получаем проекцию и геотрансформацию исходного TIFF
                string srcProjection = srcDataset.GetProjection();
                double[] srcGeoTransform = new double[6];
                srcDataset.GetGeoTransform(srcGeoTransform);

                // Создаем SpatialReference для исходного растра
                var srcSrs = new SpatialReference(srcProjection);
                if (srcSrs == null)
                {
                    Logger.LogWarning("Failed to create source spatial reference");
                    return (null, null);
                }

                // Создаем SpatialReference для WGS84 (полигон поля в WGS84)
                var wgs84Srs = new SpatialReference(string.Empty);
                wgs84Srs.SetWellKnownGeogCS("WGS84");
                wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

                // Трансформируем полигон поля в проекцию растра, если нужно
                var fieldPolygon = field.Polygon;
                if (!string.IsNullOrEmpty(srcProjection) && !srcProjection.Contains("WGS84"))
                {
                    try
                    {
                        // Создаем координатную трансформацию
                        var coordTransform = new CoordinateTransformation(wgs84Srs, srcSrs);
                        if (coordTransform != null)
                        {
                            var transformedCoords = fieldPolygon.Coordinates
                                .Select(c =>
                                {
                                    double[] point = { c.X, c.Y, 0 };
                                    coordTransform.TransformPoint(point);
                                    return new Coordinate(point[0], point[1]);
                                })
                                .ToArray();

                            if (transformedCoords.Length > 0)
                            {
                                fieldPolygon = new Polygon(new LinearRing(transformedCoords));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning(ex, $"Failed to transform polygon coordinates, using original polygon");
                    }
                }

                // Используем метод обрезки по bounding box с маской полигона
                // Это обрезает по минимальному прямоугольнику и применяет маску для пикселей вне полигона
                return CropTiffByBoundingBoxWithMask(srcDataset, fieldPolygon, srcGeoTransform, srcProjection, tempOutputPath);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error cropping TIFF by polygon for field {field.Id}");
                return (null, null);
            }
            finally
            {
                // Удаляем временные файлы
                try
                {
                    if (tempTiffPath != null && File.Exists(tempTiffPath))
                        File.Delete(tempTiffPath);
                    if (tempOutputPath != null && File.Exists(tempOutputPath))
                        File.Delete(tempOutputPath);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to delete temporary files");
                }
            }
        }

        private (byte[]? tiffBytes, double[]? geoTransform) CropTiffByBoundingBoxWithMask(
            Dataset srcDataset, Polygon fieldPolygon, double[] srcGeoTransform, string srcProjection, string tempOutputPath)
        {
            try
            {
                // Получаем bounding box полигона
                var bbox = fieldPolygon.EnvelopeInternal;

                // Вычисляем пиксельные координаты bounding box
                double[] invGeoTransform = new double[6];
                Gdal.InvGeoTransform(srcGeoTransform, invGeoTransform);

                int minX = (int)Math.Floor(
                    invGeoTransform[0] + invGeoTransform[1] * bbox.MinX + invGeoTransform[2] * bbox.MaxY);
                int maxX = (int)Math.Ceiling(
                    invGeoTransform[0] + invGeoTransform[1] * bbox.MaxX + invGeoTransform[2] * bbox.MinY);
                int minY = (int)Math.Floor(
                    invGeoTransform[3] + invGeoTransform[4] * bbox.MinX + invGeoTransform[5] * bbox.MaxY);
                int maxY = (int)Math.Ceiling(
                    invGeoTransform[3] + invGeoTransform[4] * bbox.MaxX + invGeoTransform[5] * bbox.MinY);

                // Ограничиваем координаты размерами растра
                int srcWidth = srcDataset.RasterXSize;
                int srcHeight = srcDataset.RasterYSize;
                minX = Math.Max(0, Math.Min(srcWidth - 1, minX));
                maxX = Math.Max(0, Math.Min(srcWidth - 1, maxX));
                minY = Math.Max(0, Math.Min(srcHeight - 1, minY));
                maxY = Math.Max(0, Math.Min(srcHeight - 1, maxY));

                if (minX >= maxX || minY >= maxY)
                {
                    Logger.LogWarning($"Invalid crop bounds");
                    return (null, null);
                }

                int cropWidth = maxX - minX + 1;
                int cropHeight = maxY - minY + 1;
                int bandCount = srcDataset.RasterCount;

                // Вычисляем новую геотрансформацию
                double[] newGeoTransform = new double[6];
                Array.Copy(srcGeoTransform, newGeoTransform, 6);
                newGeoTransform[0] = srcGeoTransform[0] + srcGeoTransform[1] * minX + srcGeoTransform[2] * minY;
                newGeoTransform[3] = srcGeoTransform[3] + srcGeoTransform[4] * minX + srcGeoTransform[5] * minY;

                // Создаем выходной TIFF
                var driver = Gdal.GetDriverByName("GTiff");
                Dataset? dstDataset = null;
                try
                {
                    dstDataset = driver.Create(
                        tempOutputPath,
                        cropWidth,
                        cropHeight,
                        bandCount,
                        srcDataset.GetRasterBand(1).DataType,
                        new[] { "COMPRESS=DEFLATE", "TILED=YES" });

                    dstDataset.SetGeoTransform(newGeoTransform);
                    
                    // НЕ устанавливаем проекцию вообще, чтобы избежать проблем с PROJ
                    // Геотрансформации достаточно для обрезки и дальнейшей обработки
                    // Проекция не критична для конвертации в PNG

                    // Копируем данные
                var dataType = srcDataset.GetRasterBand(1).DataType;
                for (int i = 1; i <= bandCount; i++)
                {
                    var srcBand = srcDataset.GetRasterBand(i);
                    var dstBand = dstDataset.GetRasterBand(i);

                    try
                    {
                        if (dataType == GdalDataType.GDT_Byte)
                        {
                            var buffer = new byte[cropWidth * cropHeight];
                            srcBand.ReadRaster(minX, minY, cropWidth, cropHeight, buffer, cropWidth, cropHeight, 0, 0);
                            dstBand.WriteRaster(0, 0, cropWidth, cropHeight, buffer, cropWidth, cropHeight, 0, 0);
                        }
                        else if (dataType == GdalDataType.GDT_Float32)
                        {
                            var buffer = new float[cropWidth * cropHeight];
                            srcBand.ReadRaster(minX, minY, cropWidth, cropHeight, buffer, cropWidth, cropHeight, 0, 0);
                            dstBand.WriteRaster(0, 0, cropWidth, cropHeight, buffer, cropWidth, cropHeight, 0, 0);
                        }

                        // Копируем палитру цветов, если она есть (для сохранения цвета индексов)
                        try
                        {
                            var srcColorTable = srcBand.GetColorTable();
                            if (srcColorTable != null && srcColorTable.GetCount() > 0)
                            {
                                dstBand.SetColorTable(srcColorTable);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogWarning(ex, $"Failed to copy color table for band {i}");
                        }
                        
                        // НЕ устанавливаем NoData значение, чтобы избежать проблем с PROJ
                        // NoData значения обрабатываются в ApplyPolygonMask
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"Failed to copy band {i} data, attempting to continue");
                        // Продолжаем с другими каналами
                    }
                }
                
                    // Применяем маску полигона
                    ApplyPolygonMask(dstDataset, fieldPolygon, newGeoTransform, cropWidth, cropHeight);
                }
                finally
                {
                    // Закрываем dataset явно, НЕ вызывая FlushCache, чтобы избежать ошибки PROJ
                    // Данные сохранятся при закрытии, но без валидации проекции
                    try
                    {
                        dstDataset?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning(ex, "Error disposing dataset, but data should be saved");
                    }
                }
                
                var resultBytes = File.ReadAllBytes(tempOutputPath);
                return (resultBytes, newGeoTransform);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in CropTiffByBoundingBoxWithMask");
                return (null, null);
            }
        }

        private void ApplyPolygonMask(Dataset dataset, Polygon polygon, double[] geoTransform, int width, int height)
        {
            try
            {
                int bandCount = dataset.RasterCount;
                
                // Получаем NoData значение из первого канала
                // Используем 255 (максимальное значение для byte) если NoData не установлен
                // чтобы избежать конфликта с цветами палитры (обычно палитра использует 0-254)
                double noDataValue = 255.0;
                var firstBand = dataset.GetRasterBand(1);
                int hasNoData;
                firstBand.GetNoDataValue(out double existingNoData, out hasNoData);
                if (hasNoData != 0)
                {
                    noDataValue = existingNoData;
                }
                else
                {
                    // Устанавливаем NoData = 255 для всех каналов, если не установлен
                    // Это значение не используется в палитре (обычно палитра 0-254)
                    for (int i = 1; i <= bandCount; i++)
                    {
                        try
                        {
                            dataset.GetRasterBand(i).SetNoDataValue(255.0);
                        }
                        catch
                        {
                            // Игнорируем ошибку (может быть проблема с PROJ, но это не критично)
                        }
                    }
                }

                // Читаем все данные для каждого канала
                for (int i = 1; i <= bandCount; i++)
                {
                    var band = dataset.GetRasterBand(i);
                    var dataType = band.DataType;
                    
                    // Читаем данные в зависимости от типа
                    byte[]? buffer = null;
                    if (dataType == GdalDataType.GDT_Byte)
                    {
                        buffer = new byte[width * height];
                        band.ReadRaster(0, 0, width, height, buffer, width, height, 0, 0);
                    }
                    else if (dataType == GdalDataType.GDT_Float32)
                    {
                        var floatBuffer = new float[width * height];
                        band.ReadRaster(0, 0, width, height, floatBuffer, width, height, 0, 0);
                        // Конвертируем в byte для обработки
                        buffer = new byte[width * height];
                        for (int idx = 0; idx < floatBuffer.Length; idx++)
                        {
                            buffer[idx] = (byte)Math.Max(0, Math.Min(255, floatBuffer[idx]));
                        }
                    }

                    if (buffer == null)
                        continue;

                    // Применяем маску - устанавливаем NoData для пикселей вне полигона
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Преобразуем пиксельные координаты в географические
                            double geoX = geoTransform[0] + geoTransform[1] * x + geoTransform[2] * y;
                            double geoY = geoTransform[3] + geoTransform[4] * x + geoTransform[5] * y;

                            var point = new NetTopologySuite.Geometries.Point(geoX, geoY);
                            
                            // Если точка вне полигона, устанавливаем NoData значение
                            if (!polygon.Contains(point))
                            {
                                int idx = y * width + x;
                                if (dataType == GdalDataType.GDT_Byte)
                                {
                                    buffer[idx] = (byte)noDataValue;
                                }
                                else if (dataType == GdalDataType.GDT_Float32)
                                {
                                    // Для Float32 нужно работать с floatBuffer
                                    // Это обработаем ниже
                                }
                            }
                        }
                    }

                    // Записываем обратно
                    if (dataType == GdalDataType.GDT_Byte)
                    {
                        band.WriteRaster(0, 0, width, height, buffer, width, height, 0, 0);
                    }
                    else if (dataType == GdalDataType.GDT_Float32)
                    {
                        var floatBuffer = new float[width * height];
                        for (int idx = 0; idx < buffer.Length; idx++)
                        {
                            floatBuffer[idx] = buffer[idx];
                        }
                        // Применяем маску для Float32
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                double geoX = geoTransform[0] + geoTransform[1] * x + geoTransform[2] * y;
                                double geoY = geoTransform[3] + geoTransform[4] * x + geoTransform[5] * y;
                                var point = new NetTopologySuite.Geometries.Point(geoX, geoY);
                                if (!polygon.Contains(point))
                                {
                                    int idx = y * width + x;
                                    floatBuffer[idx] = (float)noDataValue;
                                }
                            }
                        }
                        band.WriteRaster(0, 0, width, height, floatBuffer, width, height, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error applying polygon mask, continuing without mask");
            }
        }

        private byte[] CreatePreviewImage(byte[] imageBytes, int maxSize = 150, int quality = 75)
        {
            try
            {
                using var inputStream = new MemoryStream(imageBytes);
                using var image = Image.Load<Rgba32>(inputStream);

                // Вычисляем размеры для сохранения пропорций
                int newWidth, newHeight;
                if (image.Width > image.Height)
                {
                    newWidth = maxSize;
                    newHeight = (int)(image.Height * (double)maxSize / image.Width);
                }
                else
                {
                    newHeight = maxSize;
                    newWidth = (int)(image.Width * (double)maxSize / image.Height);
                }

                // Изменяем размер
                using var resizedImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(newWidth, newHeight),
                    Mode = ResizeMode.Max
                }));

                // Сохраняем как PNG с сжатием
                using var outputStream = new MemoryStream();
                var encoder = new ImageSharpPngEncoder
                {
                    CompressionLevel = PngCompressionLevel.BestCompression
                };
                resizedImage.SaveAsPng(outputStream, encoder);
                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to create preview image");
                return imageBytes; // Возвращаем оригинал при ошибке
            }
        }

        private byte[] OverlayAnomaliesOnCroppedImage(byte[] imageBytes, List<AnomalyPolygon> anomalies, Field field, double[]? geoTransform)
        {
            try
            {
                using var inputStream = new MemoryStream(imageBytes);
                using var image = Image.Load<Rgba32>(inputStream);

                if (geoTransform == null || geoTransform.Length != 6)
                {
                    Logger.LogWarning("GeoTransform is not available, using bbox for coordinate conversion");
                    var bbox = field.Polygon?.EnvelopeInternal;
                    if (bbox == null)
                        return imageBytes;

                    foreach (var anomaly in anomalies)
                    {
                        if (anomaly.Polygon == null || anomaly.Polygon.IsEmpty)
                            continue;

                        // Получаем центр полигона аномалии
                        var centroid = anomaly.Polygon.Centroid;
                        var lon = centroid.X;
                        var lat = centroid.Y;

                        // Преобразуем географические координаты в пиксели используя bbox
                        var x = (int)((lon - bbox.MinX) / (bbox.MaxX - bbox.MinX) * image.Width);
                        var y = (int)((bbox.MaxY - lat) / (bbox.MaxY - bbox.MinY) * image.Height);

                        // Ограничиваем координаты
                        x = Math.Max(0, Math.Min(image.Width - 1, x));
                        y = Math.Max(0, Math.Min(image.Height - 1, y));

                        // Рисуем точку-маркер (центр аномалии) цветом из БД
                        var color = ParseRgba32Color(anomaly.Color);
                        DrawAnomalyPoint(image, x, y, color);
                    }
                }
                else
                {
                    // Используем геотрансформацию для точного преобразования координат
                    // Геотрансформация обрезанного изображения соответствует bbox поля
                    // Вычисляем bbox обрезанного изображения из геотрансформации
                    var imageBboxMinX = geoTransform[0];
                    var imageBboxMaxY = geoTransform[3];
                    var imageBboxMaxX = geoTransform[0] + geoTransform[1] * image.Width + geoTransform[2] * image.Height;
                    var imageBboxMinY = geoTransform[3] + geoTransform[4] * image.Width + geoTransform[5] * image.Height;

                    // Используем bbox поля для преобразования координат (более надежно)
                    var fieldBbox = field.Polygon?.EnvelopeInternal;
                    if (fieldBbox == null)
                        return imageBytes;

                    foreach (var anomaly in anomalies)
                    {
                        if (anomaly.Polygon == null || anomaly.Polygon.IsEmpty)
                            continue;

                        // Получаем центр полигона аномалии (в WGS84)
                        var centroid = anomaly.Polygon.Centroid;
                        var lon = centroid.X;
                        var lat = centroid.Y;

                        // Преобразуем координаты аномалии в пиксели используя bbox поля
                        // Обрезанное изображение соответствует bbox поля
                        var x = (int)((lon - fieldBbox.MinX) / (fieldBbox.MaxX - fieldBbox.MinX) * image.Width);
                        var y = (int)((fieldBbox.MaxY - lat) / (fieldBbox.MaxY - fieldBbox.MinY) * image.Height);

                        // Ограничиваем координаты
                        x = Math.Max(0, Math.Min(image.Width - 1, x));
                        y = Math.Max(0, Math.Min(image.Height - 1, y));

                        // Рисуем точку-маркер (центр аномалии) цветом из БД
                        var color = ParseRgba32Color(anomaly.Color);
                        DrawAnomalyPoint(image, x, y, color);
                    }
                }

                using var outputStream = new MemoryStream();
                image.SaveAsPng(outputStream);
                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to overlay anomalies on image");
                return imageBytes;
            }
        }

        /// <summary>
        /// Парсит цвет из строки в Rgba32 (поддерживает форматы: "#FF0000", "FF0000", "rgb(255,0,0)")
        /// </summary>
        private Rgba32 ParseRgba32Color(string? colorString)
        {
            if (string.IsNullOrWhiteSpace(colorString))
            {
                // Если цвет не указан, используем красный по умолчанию
                return new Rgba32(255, 0, 0, 255);
            }

            colorString = colorString.Trim();

            // Формат "#FF0000" или "FF0000"
            if (colorString.StartsWith("#"))
            {
                colorString = colorString.Substring(1);
            }

            // Пробуем распарсить как hex (6 символов: RRGGBB)
            if (colorString.Length == 6 && System.Text.RegularExpressions.Regex.IsMatch(colorString, "^[0-9A-Fa-f]{6}$"))
            {
                var r = Convert.ToByte(colorString.Substring(0, 2), 16);
                var g = Convert.ToByte(colorString.Substring(2, 2), 16);
                var b = Convert.ToByte(colorString.Substring(4, 2), 16);
                return new Rgba32(r, g, b, 255);
            }

            // Формат "rgb(255,0,0)" или "rgba(255,0,0,1)"
            if (colorString.StartsWith("rgb", StringComparison.OrdinalIgnoreCase))
            {
                var match = System.Text.RegularExpressions.Regex.Match(colorString, @"rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*([\d.]+))?\)");
                if (match.Success)
                {
                    var r = byte.Parse(match.Groups[1].Value);
                    var g = byte.Parse(match.Groups[2].Value);
                    var b = byte.Parse(match.Groups[3].Value);
                    var a = match.Groups[4].Success ? (byte)(float.Parse(match.Groups[4].Value) * 255) : (byte)255;
                    return new Rgba32(r, g, b, a);
                }
            }

            // Если не удалось распарсить, используем красный по умолчанию
            Logger.LogWarning($"Failed to parse color '{colorString}', using red as default");
            return new Rgba32(255, 0, 0, 255);
        }

        private void DrawAnomalyPoint(Image<Rgba32> image, int x, int y, Rgba32 color)
        {
            // Рисуем точку-маркер (центр аномалии) с белой обводкой для видимости
            var radius = 2;
            var innerRadius = 1; // Внутренний радиус для точки
            var whiteColor = new Rgba32(255, 255, 255, 255);
            
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    var px = x + dx;
                    var py = y + dy;
                    if (px >= 0 && px < image.Width && py >= 0 && py < image.Height)
                    {
                        var distanceSquared = dx * dx + dy * dy;
                        
                        // Сначала рисуем белую обводку (внешний круг)
                        if (distanceSquared <= radius * radius && distanceSquared > innerRadius * innerRadius)
                        {
                            var currentPixel = image[px, py];
                            // Рисуем белую обводку только если пиксель не прозрачный
                            // (на прозрачном фоне белая обводка не нужна)
                            if (currentPixel.A > 0)
                            {
                                image[px, py] = whiteColor;
                            }
                        }
                        // Затем рисуем точку заданным цветом (внутренний круг)
                        else if (distanceSquared <= innerRadius * innerRadius)
                        {
                            // Точку рисуем всегда, даже на прозрачном фоне
                            image[px, py] = color;
                        }
                    }
                }
            }
        }

        private List<AnomalyCoordinateVm> ExtractAnomalyCoordinates(List<AnomalyPolygon> anomalies)
        {
            var coordinates = new List<AnomalyCoordinateVm>();
            foreach (var anomaly in anomalies)
            {
                if (anomaly.Polygon == null || anomaly.Polygon.IsEmpty)
                    continue;

                // Получаем центр полигона аномалии
                var centroid = anomaly.Polygon.Centroid;
                coordinates.Add(new AnomalyCoordinateVm
                {
                    Latitude = centroid.Y,
                    Longitude = centroid.X
                });
            }
            return coordinates;
        }

        private byte[] AssembleTilesIntoImage(List<(int x, int y, byte[] data)> tiles, int minTileX, int minTileY, int maxTileX, int maxTileY, int zoom, double minX, double minY, double maxX, double maxY)
        {
            const int TileSize = 256;
            var tileWidth = maxTileX - minTileX + 1;
            var tileHeight = maxTileY - minTileY + 1;
            var imageWidth = tileWidth * TileSize;
            var imageHeight = tileHeight * TileSize;

            using var surface = SKSurface.Create(new SKImageInfo(imageWidth, imageHeight, SKColorType.Rgba8888, SKAlphaType.Premul));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            foreach (var (x, y, data) in tiles)
            {
                var offsetX = (x - minTileX) * TileSize;
                var offsetY = (y - minTileY) * TileSize;

                using var bitmap = SKBitmap.Decode(data);
                if (bitmap != null)
                {
                    canvas.DrawBitmap(bitmap, offsetX, offsetY);
                }
            }

            using var image = surface.Snapshot();
            using var png = image.Encode(SKEncodedImageFormat.Png, 100);
            return png.ToArray();
        }

        private byte[] OverlayAnomaliesOnImage(byte[] baseImageBytes, List<AnomalyPolygon> anomalies, Field field, int zoom, double minX, double minY, double maxX, double maxY)
        {
            using var baseImage = SKBitmap.Decode(baseImageBytes);
            using var surface = SKSurface.Create(new SKImageInfo(baseImage.Width, baseImage.Height, SKColorType.Rgba8888, SKAlphaType.Premul));
            var canvas = surface.Canvas;

            using var basePaint = new SKPaint();
            canvas.DrawBitmap(baseImage, 0, 0, basePaint);

            const int TileSize = 256;
            const int EarthRadius = 6378137;
            double res = 2 * Math.PI * EarthRadius / (TileSize * Math.Pow(2, zoom));

            foreach (var anomaly in anomalies)
            {
                if (anomaly.Polygon == null || anomaly.Polygon.IsEmpty)
                    continue;

                var color = ParseColor(anomaly.Color);
                using var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = color,
                    StrokeWidth = 3,
                    IsAntialias = true
                };

                var path = new SKPath();
                var coords = anomaly.Polygon.ExteriorRing.Coordinates;
                for (int i = 0; i < coords.Length; i++)
                {
                    var (x, y) = LonLatToWebMercator(coords[i].X, coords[i].Y);
                    float px = (float)((x - minX) / res);
                    float py = (float)((maxY - y) / res);

                    if (i == 0) path.MoveTo(px, py);
                    else path.LineTo(px, py);
                }
                path.Close();
                canvas.DrawPath(path, paint);
            }

            using var image = surface.Snapshot();
            using var png = image.Encode(SKEncodedImageFormat.Png, 100);
            return png.ToArray();
        }

        private (double x, double y) LonLatToWebMercator(double lon, double lat)
        {
            double x = lon * EarthRadiusOnPi / 180;
            double y = Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180);
            y = y * EarthRadiusOnPi / 180;
            return (x, y);
        }

        private (int tileX, int tileY) WebMercatorToTile(double x, double y, int z)
        {
            double tileCount = Math.Pow(2, z);
            double tileSize = EarthRadiusOnPi * 2 / tileCount;

            int tileX = (int)((x + EarthRadiusOnPi) / tileSize);
            int tileY = (int)((EarthRadiusOnPi - y) / tileSize);

            return (tileX, tileY);
        }

        private SKColor ParseColor(string colorStr)
        {
            if (string.IsNullOrEmpty(colorStr))
                return SKColors.Red;

            if (colorStr.StartsWith("#"))
            {
                if (uint.TryParse(colorStr.Substring(1), System.Globalization.NumberStyles.HexNumber, null, out uint hex))
                {
                    return new SKColor((byte)((hex >> 16) & 0xFF), (byte)((hex >> 8) & 0xFF), (byte)(hex & 0xFF), 255);
                }
            }

            return colorStr.ToLower() switch
            {
                "red" => SKColors.Red,
                "green" => SKColors.Green,
                "blue" => SKColors.Blue,
                "yellow" => SKColors.Yellow,
                "orange" => SKColors.Orange,
                _ => SKColors.Red
            };
        }

        private async Task<string> GenerateFieldTitleAsync(Field field, Season season, string locale, CancellationToken ct)
        {
            string cropName = "";
            if (season.CropId != null)
            {
                var crop = await MasofaDictionariesDbContext.Crops
                    .AsNoTracking()
                    .Where(c => c.Id == season.CropId)
                    .FirstOrDefaultAsync(ct);
                if (crop != null)
                {
                    var localeName = !string.IsNullOrEmpty(crop.Names[locale]) ? crop.Names[locale] : "";
                    var ruName = !string.IsNullOrEmpty(crop.Names["ru-RU"]) ? crop.Names["ru-RU"] : "";
                    cropName = !string.IsNullOrEmpty(localeName) ? localeName : ruName;
                }
            }

            string varietyName = "";
            if (season.VarietyId != null)
            {
                var variety = await MasofaDictionariesDbContext.Varieties
                    .AsNoTracking()
                    .Where(v => v.Id == season.VarietyId)
                    .FirstOrDefaultAsync(ct);
                if (variety != null)
                {
                    varietyName = variety.NameLa;
                    if (string.IsNullOrEmpty(varietyName))
                    {
                        var localeName = !string.IsNullOrEmpty(variety.Names[locale]) ? variety.Names[locale] : "";
                        var ruName = !string.IsNullOrEmpty(variety.Names["ru-RU"]) ? variety.Names["ru-RU"] : "";
                        varietyName = !string.IsNullOrEmpty(localeName) ? localeName : ruName;
                    }
                }
            }

            var parts = new List<string> { field.Name };
            if (!string.IsNullOrEmpty(cropName))
                parts.Add(cropName);
            if (!string.IsNullOrEmpty(varietyName))
                parts.Add($"Variety {varietyName}");

            return string.Join(". ", parts) + (parts.Count > 1 ? "." : "");
        }

        private async Task<(DateTime imageDate, string satellite)> GetImageDateAndSatelliteAsync(Field field, List<AnomalyPolygon> anomalies, DateOnly reportDate, CancellationToken ct)
        {
            var reportDateUtc = DateTime.SpecifyKind(reportDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            
            var satelliteType = await MasofaCropMonitoringDbContext.FieldProductMappings
                .AsNoTracking()
                .Where(fpm => fpm.FieldId == field.Id && fpm.Status == StatusType.Active)
                .Select(fpm => fpm.SatelliteType)
                .FirstOrDefaultAsync(ct);

            var satellite = satelliteType switch
            {
                ProductSourceType.Sentinel2 => "Sentinel-2",
                ProductSourceType.Landsat => "Landsat",
                _ => "Unknown"
            };

            return (reportDateUtc, satellite);
        }

        private async Task<string> GetQwenResultJson(Guid fieldId)
        {
            var bids = await MasofaCropMonitoringDbContext.Bids
                .Where(b => b.FieldId == fieldId)
                .ToListAsync();

            string qwenResult = string.Empty;

            foreach (var b in bids)
            {
                qwenResult = b.QwenResultJson;
            }

            return qwenResult;
        }

        public async Task<Era5DayWeatherForecastByCoordinates> GetByCoordinatesAndDate(CoordinatesAndDateViewModel query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByCoordinatesAndDate)}";
            try
            {
                var targetPoint = new NetTopologySuite.Geometries.Point(query.Longitude, query.Latitude)
                {
                    SRID = 4326
                };

                var closestStation = await MasofaWeatherReportDbContext.EraWeatherStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return null;
                }

                var inputDate = DateOnly.FromDateTime(query.InputDate);

                var closestFutureReport = await MasofaWeatherReportDbContext.Era5DayWeatherForecasts
                    .Where(x => x.WeatherStation == closestStation.Id && x.Date == inputDate)
                    .OrderBy(x => x.Date)
                    .FirstOrDefaultAsync();

                if (closestFutureReport is null)
                {
                    return null;
                }

                var newGetEraWeatherSumsCommand = new GetEraWeatherSumsCommand()
                {
                    Date = closestFutureReport.Date,
                    StationIds = [closestStation.Id]
                };

                var sums = await Mediator.Send(newGetEraWeatherSumsCommand);

                var result = new Era5DayWeatherForecastByCoordinates();
                result.CopyFrom(closestFutureReport);

                result.SumOfActiveTemperaturesBase5 = sums.SumOfActiveTemperaturesBase5;
                result.SumOfActiveTemperaturesBase7 = sums.SumOfActiveTemperaturesBase7;
                result.SumOfActiveTemperaturesBase10 = sums.SumOfActiveTemperaturesBase10;
                result.SumOfActiveTemperaturesBase12 = sums.SumOfActiveTemperaturesBase12;
                result.SumOfActiveTemperaturesBase15 = sums.SumOfActiveTemperaturesBase15;
                result.SumOfSolarRadiation = sums.SumOfSolarRadiation;
                result.SumOfFallout = sums.SumOfFallout;

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return null;
            }
        }

        public async Task<BidResultVm> GetBidResults(Guid FieldId, BuildFarmerReportCommand request, Guid? cropId)
        {
            if (FieldId == Guid.Empty)
                return new BidResultVm { Rows = new List<BidResultRow>() };

            var bids = await MasofaCropMonitoringDbContext.Bids
                .AsNoTracking()
                .Where(b => b.FieldId == FieldId)
                .Where(b => b.CropId == cropId)
                .ToListAsync();

            if (bids == null || bids.Count == 0)
                return new BidResultVm { Rows = new List<BidResultRow>() };

            var bidResultRows = new List<BidResultRow>(bids.Count);

            foreach (var bid in bids)
            {
                var crop = await MasofaDictionariesDbContext.Crops
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == bid.CropId);

                var status = bid?.BidState.ToString() ?? "—";
                var start = bid?.StartDate;
                var end = bid?.EndDate;
                var workPeriod = start.HasValue || end.HasValue
                    ? $"{(start.HasValue ? start.Value.ToString("dd.MM.yyyy") : "—")} - {(end.HasValue ? end.Value.ToString("dd.MM.yyyy") : "—")}"
                    : "—";

                var bidResultRow = new BidResultRow()
                {
                    Crop = crop.Names[request.Locale],
                    Status = status,
                    WorkPeriod = workPeriod
                };

                bidResultRows.Add(bidResultRow);
            }

            return new BidResultVm { Rows = bidResultRows };
        }

        private async Task<List<AnomalyPhotoTableRow>> BuildAnomalyTableRowsAsync(
            FarmerRecomendationReport reportEntity,
            Stream? zipStream,
            string locale,
            string? cropName,
            CancellationToken ct)
        {
            var rows = new List<AnomalyPhotoTableRow>();

            if(zipStream == null)
            {
                return rows;
            }

            // 1) Берём уже типизированные items (через твой кеш/DeserializeQwen)
            var items = reportEntity.QwenItems;
            if (items == null || items.Count == 0)
                return rows;

            // 2) Только аномальные + с валидной ссылкой на изображение
            var anomalyItems = items
                .Where(x => x.AnomalyPresence)
                .Where(x => !string.IsNullOrWhiteSpace(x.Image))
                // дедуп по имени файла (иначе легко получить дубли при мердже/повторном анализе)
                .GroupBy(x => Path.GetFileName(x.Image), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            if (anomalyItems.Count == 0)
                return rows;

            const int MaxPhotos = 30;
            string? tempZipPath = null;

            try
            {
                // 3) Сохраняем ZIP во временный файл (без RAM)
                tempZipPath = Path.Combine(Path.GetTempPath(), $"bid_{Guid.NewGuid():N}.zip");

                await using (var fs = new FileStream(
                    tempZipPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Read,
                    bufferSize: 1024 * 1024,
                    options: FileOptions.SequentialScan))
                {
                    await zipStream.CopyToAsync(fs, ct);
                    await fs.FlushAsync(ct);
                }

                // 4) Открываем ZIP
                await using var zipFs = new FileStream(
                    tempZipPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 1024 * 1024,
                    options: FileOptions.SequentialScan);

                using var archive = new ZipArchive(zipFs, ZipArchiveMode.Read);

                // 5) Индексируем entries по имени файла (быстрее чем FirstOrDefault в цикле)
                var entryByName = archive.Entries
                    .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                    .GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                foreach (var item in anomalyItems.Take(MaxPhotos))
                {
                    var fileName = Path.GetFileName(item.Image);
                    string? dataUri = null;

                    if (!string.IsNullOrWhiteSpace(fileName) && entryByName.TryGetValue(fileName, out var entry))
                    {
                        await using var es = entry.Open();
                        using var ms = new MemoryStream();
                        await es.CopyToAsync(ms, ct);

                        var bytes = ms.ToArray();

                        var ext = Path.GetExtension(fileName).ToLowerInvariant();
                        var mime = ext switch
                        {
                            ".png" => "image/png",
                            ".webp" => "image/webp",
                            ".jpg" => "image/jpeg",
                            ".jpeg" => "image/jpeg",
                            _ => "image/jpeg"
                        };

                        dataUri = $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
                    }

                    rows.Add(new AnomalyPhotoTableRow
                    {
                        PhotoDataUri = dataUri,
                        Crop = cropName ?? item.PlantType,
                        AnomalyPresence = item.AnomalyPresence,
                        AnomalyDescription = item.AnomalyDescription,
                        ProblemType = item.ProblemType,
                        Anomaly1 = item.Anomaly1,
                        Anomaly2 = item.Anomaly2,
                        ClassificationDescription = item.Classification_description,
                        Recommendations = item.Recommendations
                    });
                }

                return rows;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(tempZipPath) && File.Exists(tempZipPath))
                {
                    try { File.Delete(tempZipPath); } catch { /* не критично */ }
                }
            }
        }


        //private async Task<List<AnomalyPhotoTableRow>> BuildAnomalyTableRowsAsync(FarmerRecomendationReport reportEntity, Stream zipStream,
        //    string locale,
        //    string? cropName,
        //    CancellationToken ct)
        //{
        //    var rows = new List<AnomalyPhotoTableRow>();

        //    if (string.IsNullOrWhiteSpace(reportEntity.QwenJobResultJson))
        //        return rows;

        //    QwenResultRoot? root;
        //    try
        //    {
        //        root = JsonConvert.DeserializeObject<QwenResultRoot>(reportEntity.QwenJobResultJson);
        //    }
        //    catch
        //    {
        //        return rows;
        //    }

        //    if (root?.Items == null || root.Items.Count == 0)
        //        return rows;

        //    var anomalyItems = root.Items
        //        .Where(x => x.anomaly_presence)
        //        .Where(x => !string.IsNullOrWhiteSpace(x.image))
        //        .ToList();

        //    if (anomalyItems.Count == 0)
        //        return rows;

        //    const int MaxPhotos = 30;

        //    string? tempZipPath = null;

        //    try
        //    {
        //        // === Скачиваем ZIP во временный файл (БЕЗ RAM)
        //        tempZipPath = Path.Combine(
        //            Path.GetTempPath(),
        //            $"bid_{Guid.NewGuid():N}.zip");

        //        await using (var fs = new FileStream(
        //            tempZipPath,
        //            FileMode.Create,
        //            FileAccess.Write,
        //            FileShare.Read,
        //            bufferSize: 1024 * 1024,
        //            options: FileOptions.SequentialScan))
        //        {
        //            await zipStream.CopyToAsync(fs, ct);
        //            await fs.FlushAsync(ct);
        //        }

        //        // === Открываем ZIP как файл
        //        await using var zipFs = new FileStream(
        //            tempZipPath,
        //            FileMode.Open,
        //            FileAccess.Read,
        //            FileShare.Read,
        //            bufferSize: 1024 * 1024,
        //            options: FileOptions.SequentialScan);

        //        using var archive = new ZipArchive(zipFs, ZipArchiveMode.Read);

        //        foreach (var item in anomalyItems.Take(MaxPhotos))
        //        {
        //            var fileName = Path.GetFileName(item.image!);

        //            var entry = archive.Entries.FirstOrDefault(e =>
        //                string.Equals(e.Name, fileName, StringComparison.OrdinalIgnoreCase) ||
        //                e.FullName.EndsWith("/" + fileName, StringComparison.OrdinalIgnoreCase) ||
        //                e.FullName.EndsWith("\\" + fileName, StringComparison.OrdinalIgnoreCase));

        //            string? dataUri = null;

        //            if (entry != null)
        //            {
        //                await using var es = entry.Open();
        //                using var ms = new MemoryStream();
        //                await es.CopyToAsync(ms, ct);

        //                var bytes = ms.ToArray();

        //                var ext = Path.GetExtension(fileName).ToLowerInvariant();
        //                var mime = ext switch
        //                {
        //                    ".png" => "image/png",
        //                    ".webp" => "image/webp",
        //                    ".jpeg" => "image/jpeg",
        //                    _ => "image/jpeg"
        //                };

        //                dataUri = $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
        //            }

        //            rows.Add(new AnomalyPhotoTableRow
        //            {
        //                PhotoDataUri = dataUri,
        //                Crop = cropName ?? item.plant_type,
        //                AnomalyPresence = item.anomaly_presence,
        //                AnomalyDescription = item.anomaly_description,
        //                ProblemType = item.problem_type,
        //                Anomaly1 = item.anomaly1,
        //                Anomaly2 = item.anomaly2,
        //                ClassificationDescription = item.classification_description,
        //                Recommendations = item.recommendations
        //            });
        //        }
        //    }
        //    finally
        //    {
        //        // === Чистим temp-файл
        //        if (!string.IsNullOrWhiteSpace(tempZipPath) && File.Exists(tempZipPath))
        //        {
        //            try { File.Delete(tempZipPath); }
        //            catch { /* не критично */ }
        //        }
        //    }

        //    return rows;
        //}

        #endregion
    }


    #region ViewModels

    public static class SoilCanon
    {
        public const string Sand = "sand";
        public const string Silt = "silt";
        public const string Clay = "clay";
        public const string Ph = "phh2o";
        public const string CEC = "cec";
        public const string SOC = "soc";
        public const string Density = "bdod";
        public const string CFVO = "cfvo";
        public const string N = "nitrogen";
        public const string Humus = "humus";
        public const string P = "phosphorus";
        public const string Salinity = "salinity";
    }

    public class CoordinatesAndDateViewModel
    {
        /// <summary>
        /// Широта
        /// </summary>
        [Required]
        public required double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [Required]
        public required double Longitude { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        [Required]
        public required DateTime InputDate { get; set; }
    }

    public class Era5DayWeatherForecastByCoordinates : Era5DayWeatherForecast, IWeatherReportWithSums
    {
        public double? SumOfActiveTemperaturesBase5 { get; set; }
        public double? SumOfActiveTemperaturesBase7 { get; set; }
        public double? SumOfActiveTemperaturesBase10 { get; set; }
        public double? SumOfActiveTemperaturesBase12 { get; set; }
        public double? SumOfActiveTemperaturesBase15 { get; set; }
        public double? SumOfSolarRadiation { get; set; }
        public double? SumOfFallout { get; set; }
    }

    public sealed class QwenResultRoot
    {
        public List<QwenImageItem> Items { get; set; } = new();
        public QwenSummary? Summary { get; set; }
    }

    public sealed class QwenImageItem
    {
        public string? archive_name { get; set; }
        public string? image { get; set; }
        public string? plant_type { get; set; }
        public bool anomaly_presence { get; set; }
        public string? anomaly_description { get; set; }
        public string? problem_type { get; set; }
        public string? anomaly1 { get; set; }
        public string? anomaly2 { get; set; }
        public string? classification_description { get; set; }
        public string? recommendations { get; set; }
    }

    public sealed class QwenSummary
    {
        public int anomaly_count { get; set; }
        public int total_images { get; set; }
    }
    #endregion
}