using Masofa.BusinessLogic.Common.EmailSender;
using Masofa.BusinessLogic.Services;
using Masofa.Cli.DevopsUtil.Commands.CodeGenerators;
using Masofa.Cli.DevopsUtil.Commands.CropMonitoring;
using Masofa.Cli.DevopsUtil.Commands.DataCompare;
using Masofa.Cli.DevopsUtil.Commands.Demo;
using Masofa.Cli.DevopsUtil.Commands.Dictionaries;
using Masofa.Cli.DevopsUtil.Commands.Export;
using Masofa.Cli.DevopsUtil.Commands.Import;
using Masofa.Cli.DevopsUtil.Commands.Indices;
using Masofa.Cli.DevopsUtil.Commands.Satellite;
using Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2;
using Masofa.Cli.DevopsUtil.Commands.Tiles;
using Masofa.Cli.DevopsUtil.Commands.Weather;
using Masofa.Client.Copernicus;
using Masofa.Client.EarthExplorer;
using Masofa.Client.Era5;
using Masofa.Common;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.EmailSender;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Depricated.DataAccess.DepricatedAuthServerOne;
using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;
using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;
using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
using Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerOne;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
using Masofa.Depricated.DataAccess.DepricatedWeatherServerOne;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using NetTopologySuite;
using OSGeo.GDAL;
using System.Runtime.InteropServices;

namespace Masofa.Cli.DevopsUtil
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            RegisterGdal();
            using var host = CreateHostBuilder(args).Build();
            await BaseCommandCliService.ExecuteAsync(host.Services.CreateScope(), "Masofa DevOps Utils", args);
        }

        static void RegisterGdal()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var rid = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win-x64"
                    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux-x64"
                    : throw new PlatformNotSupportedException();
            var runtimes = Path.Combine(baseDir, "runtimes", rid, "native");

            Environment.SetEnvironmentVariable("GDAL_DATA", Path.Combine(runtimes, "gdal-data"));
            var projLib = Path.Combine(runtimes, "maxrev.gdal.core.libshared");
            var projDbPath = Path.Combine(projLib, "proj.db");

            Environment.SetEnvironmentVariable("PROJ_LIB", projLib);

            Console.WriteLine($"PROJ_LIB = {projLib}");
            Console.WriteLine($"proj.db exists: {File.Exists(projDbPath)}");

            Gdal.AllRegister();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {

                var serviceProvider = services.BuildServiceProvider();

                services.AddLogging(cfg =>
                {
                    cfg.ClearProviders();
                    //cfg.AddConsole();
                    //cfg.SetMinimumLevel(LogLevel.Information);
                });

                services.AddTransient<Era5ApiUnitOfWork>();


                services.AddDbContext<DepricatedUmapiServerOneDbContext>();
                services.AddDbContext<DepricatedAuthServerOneDbContext>();

                services.AddDbContext<DepricatedUmapiServerTwoDbContext>();
                services.AddDbContext<DepricatedAuthServerTwoDbContext>();

                services.AddDbContext<DepricatedUalertsServerOneDbContext>();
                services.AddDbContext<DepricatedUdictServerTwoDbContext>();

                services.AddDbContext<DepricatedUfieldsServerOneDbContext>();

                services.AddDbContext<DepricatedWeatherServerOneDbContext>();

                services.AddTransient<IFileStorageProvider, MinIOStorageProvider>();

                services.AddHttpClient();

                // Регистрируем LandsatApiUnitOfWork
                services.AddScoped<LandsatApiUnitOfWork>();

                // Регистрируем CopernicusApiUnitOfWork
                services.AddScoped<CopernicusApiUnitOfWork>();

                // Регистрируем IBMWeatherApiUnitOfWork
                services.AddScoped<Masofa.Client.IBMWeather.IBMWeatherApiUnitOfWork>();

                // Регистрируем IFileStorageProvider
                services.AddTransient<Masofa.Common.Services.FileStorage.IFileStorageProvider, Masofa.Common.Services.FileStorage.MinIOStorageProvider>();

                services.AddTransient<GeoServerService>();

                var templatesPath = Path.Combine(AppContext.BaseDirectory, "Templates", "EmailTemplates");

                services.AddTransient<IEmailSender>(sp =>
                {
                    var smtpOptions = sp.GetRequiredService<IOptions<SmtpOptions>>();
                    var dbContext = sp.GetRequiredService<MasofaCommonDbContext>();
                    return new EmailSender(dbContext, smtpOptions, templatesPath);
                });

                services.AddIdentity<User, Role>(options =>
                {
                    options.User.RequireUniqueEmail = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;
                }).AddEntityFrameworkStores<MasofaIdentityDbContext>();


                services.AddSingleton(_ => NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

                // Регистрируем IBusinessLogicLogger
                services.AddScoped<Masofa.BusinessLogic.Services.BusinessLogicLogger.IBusinessLogicLogger, Masofa.BusinessLogic.Services.BusinessLogicLogger.BusinessLogicLogger>();

                services.AddBusinessLogic();

                services.AddTransient<CreateReportCommand>();
                services.AddTransient<EraWeatherHistoricalDataLoaderCommand>();
                services.AddTransient<FetchProductsAndIndicesCommand>();
                services.AddTransient<ImportSoilDataTiffCommand>();
                services.AddTransient<SRIDCountCommand>();
                services.AddTransient<ExportFullCountryOsmMapCommand>();
                services.AddTransient<ImportSeasonsFieldsCommand>();
                services.AddTransient<RegionAndRegionMapCompareCommad>();
                services.AddTransient<GenerateLogMessageListCommand>();
                services.AddTransient<GenerateLogMessageListResourceCommand>();
                services.AddTransient<ParallelesTiffFetchProductCommand>();
                services.AddTransient<SentinelCreatePartitionCommand>();
                services.AddTransient<ParallelesPointFetchProductCommand>();
                services.AddTransient<FieldAndSatellieCompareCommand>();
                services.AddTransient<Sentinel2ProductPolygonFillCommand>();
                services.AddTransient<EraNormalizedDataRandomCommand>();
                services.AddTransient<EraIsFrostDangerCalculateCommand>();
                services.AddTransient<FieldAndSatellieCompareCommand>();
                services.AddTransient<EraNormalizedDataTilesCommand>();
                services.AddTransient<AddSatellitePolygonSeadonRelationsCommand>();
                services.AddTransient<StoringColorIndicesCommand>();
                services.AddTransient<CreateMultiPolygonJsonFileForMonth>();
                services.AddTransient<AddDemoFieldCommand>();
                services.AddTransient<ImportRegionsCommand>();
                services.AddTransient<FillWeatherTileLayers>();
                services.AddTransient<BidTemplateExport>();
                services.AddTransient<EraDeviationTilesCommand>();
                services.AddTransient<RepublishOldWeatherLayers>();
                services.AddTransient<FillSeasonPlantingDatesCommand>();
                services.AddTransient<FillFieldAltitudeCommand>();
                services.AddTransient<SaveIndicesPreviews>();
                services.AddTransient<UpdateNormalizeCommand>();
                services.AddTransient<FillRegionsAreasCommand>();
                services.AddTransient<Sentinel2ProductRegionRelationCommand>();
                services.AddTransient<MarkDownFormatingCommand>();
                services.AddTransient<CalculateAnomaliesCommand>();
                services.AddTransient<ParallelesPointAndTiffFetchProductCommand>();
                services.AddTransient<FillBidsPeriodCommand>();
                services.AddTransient<AddDataForDemoCommand>();
                services.AddTransient<ImportDemoBidResultsCommand>();
                services.AddTransient<CreateRandomDemoAnomalyPolygons>();
                services.AddTransient<GuidGenerationCommand>();
                services.AddTransient<RestoreLayersFromDbCommand>();
                services.AddTransient<SqlGeneratorCommand>();
                services.AddTransient<ExecuteSqlScriptsToDbContext>();
            });
    }
}
