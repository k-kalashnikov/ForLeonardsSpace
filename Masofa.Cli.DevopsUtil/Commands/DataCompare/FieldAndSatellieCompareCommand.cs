using Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2;
using Masofa.Client.Copernicus;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Cli.DevopsUtil.Commands.Demo
{
    [BaseCommand("FieldAndSatellieCompareCommand", "FieldAndSatellieCompareCommand")]
    public class FieldAndSatellieCompareCommand : IBaseCommand
    {
        ILogger<FieldAndSatellieCompareCommand> Logger { get; }
        private SentinelServiceOptions SentinelServiceOptions { get; }
        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }
        public FieldAndSatellieCompareCommand(ILogger<FieldAndSatellieCompareCommand> logger,
                    CopernicusApiUnitOfWork copernicusApiUnitOfWork,
                    MasofaSentinelDbContext sentinelDbContext,
                    IOptions<SentinelServiceOptions> options,
                    MasofaCommonDbContext masofaCommonDbContext,
                    IFileStorageProvider fileStorageProvider,
                    MasofaIndicesDbContext masofaIndicesDbContext,
                    MasofaCropMonitoringDbContext masofaCropMonitoringDbContext,
                    MasofaDictionariesDbContext masofaDictionariesDbContext)
        {
            Logger = logger;
            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
            SentinelDbContext = sentinelDbContext;
            SentinelServiceOptions = options.Value;
            SentinelServiceOptions.SatelliteSearchConfig = new SatelliteSearchConfig();
            MasofaCommonDbContext = masofaCommonDbContext;
            FileStorageProvider = fileStorageProvider;
            MasofaIndicesDbContext = masofaIndicesDbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }
        public async Task Execute()
        {
            try
            {
                var satelliteProducts = MasofaCommonDbContext.SatelliteProducts
                    .Where(m => m.ProductSourceType == ProductSourceType.Sentinel2)
                    .Where(m => m.Polygon != null)
                    .ToList();

                var fields = MasofaCropMonitoringDbContext.Fields
                    .Where(f => f.Polygon != null)
                    .ToList();

                var neededFields = fields
                    .Where(f => satelliteProducts.Select(sp => sp.Polygon).Any(sp => sp.Contains(f.Polygon) || sp.Intersects(f.Polygon)))
                    .ToList();

                var satelliteProductsWithFields = new Dictionary<Guid, List<Masofa.Common.Models.CropMonitoring.Field>>();

                foreach (var satelliteProduct in satelliteProducts)
                {
                    if (!satelliteProductsWithFields.ContainsKey(satelliteProduct.Id))
                    {
                        satelliteProductsWithFields.Add(satelliteProduct.Id, new List<Common.Models.CropMonitoring.Field>());
                    }
                    foreach (var field in neededFields)
                    {
                        if ((satelliteProduct.Polygon.Contains(field.Polygon)) || (satelliteProduct.Polygon.Intersects(field.Polygon)))
                        {
                            satelliteProductsWithFields[satelliteProduct.Id].Add(field);
                            continue;
                        }
                    }
                }

                Console.WriteLine($"satelliteProducts without fields = {satelliteProductsWithFields.Where(m => !m.Value.Any()).Count()}");
                Console.WriteLine($"satelliteProducts with fields = {satelliteProductsWithFields.Where(m => m.Value.Any()).Count()}");

                foreach (var spwfItem in satelliteProductsWithFields.Where(m => m.Value.Any()).ToList())
                {
                    Console.WriteLine($"Satellite Product {spwfItem.Key} has {spwfItem.Value.Count} fields");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                Console.WriteLine(ex.InnerException?.Message);
            }

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }



        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
