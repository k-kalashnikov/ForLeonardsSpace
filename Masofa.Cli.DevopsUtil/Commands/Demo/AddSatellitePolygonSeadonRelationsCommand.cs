
using Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2;
using Masofa.Client.Copernicus;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Cli.DevopsUtil.Commands.Demo
{
    [BaseCommand("AddSatellitePolygonSeadonRelationsCommand", "AddSatellitePolygonSeadonRelationsCommand")]
    public class AddSatellitePolygonSeadonRelationsCommand : IBaseCommand
    {

        ILogger<AddSatellitePolygonSeadonRelationsCommand> Logger { get; }
        private SentinelServiceOptions SentinelServiceOptions { get; }
        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }


        private static bool GdalInitialized = false;
        private const double EPS = 1e-8;
        private static int outputH = 0;
        private static readonly object consoleLock = new object();
        private static readonly object dbContextLock = new object();
        private static readonly object fileStorageLock = new object();
        private Dictionary<Guid, Dictionary<string, bool>> globalResult = new Dictionary<Guid, Dictionary<string, bool>>();
        private string TempPath = "/";

        public AddSatellitePolygonSeadonRelationsCommand(ILogger<AddSatellitePolygonSeadonRelationsCommand> logger,
            CopernicusApiUnitOfWork copernicusApiUnitOfWork,
            MasofaSentinelDbContext sentinelDbContext,
            IOptions<SentinelServiceOptions> options,
            MasofaCommonDbContext masofaCommonDbContext,
            IFileStorageProvider fileStorageProvider,
            MasofaIndicesDbContext masofaIndicesDbContext,
            MasofaCropMonitoringDbContext masofaCropMonitoringDbContext)
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
        }
        private Dictionary<Guid, List<Masofa.Common.Models.CropMonitoring.Season>> GetSatelluteProductWithSeason()
        {
            var satelliteProductsWithFields = new Dictionary<Guid, List<Masofa.Common.Models.CropMonitoring.Season>>();

            try
            {
                var satelliteProducts = MasofaCommonDbContext.SatelliteProducts
                    .Where(m => m.ProductSourceType == ProductSourceType.Sentinel2)
                    .Where(m => m.Polygon != null)
                    .ToList();

                var seasons = MasofaCropMonitoringDbContext.Seasons
                    .Where(f => f.FieldId == Guid.Parse("3cd6eb9c-73e6-4b75-8f87-94de60384681"))
                    .Where(f => f.Polygon != null)
                    .ToList();

                var neededSeasons = seasons
                    .Where(f => satelliteProducts.Select(sp => sp.Polygon).Any(sp => sp.Contains(f.Polygon) || sp.Intersects(f.Polygon)))
                    .ToList();


                foreach (var satelliteProduct in satelliteProducts)
                {
                    var tempSeasons = new List<Common.Models.CropMonitoring.Season>();
                    foreach (var season in neededSeasons)
                    {
                        if (!((satelliteProduct.Polygon.Contains(season.Polygon)) || (satelliteProduct.Polygon.Intersects(season.Polygon))))
                        {
                            continue;
                        }

                        if (!((DateOnly.FromDateTime(satelliteProduct.OriginDate.Value) >= season.PlantingDate.Value) && (DateOnly.FromDateTime(satelliteProduct.OriginDate.Value) <= season.HarvestingDate.Value)))
                        {
                            continue;
                        }

                        tempSeasons.Add(season);
                    }
                    if (tempSeasons.Any())
                    {
                        if (!satelliteProductsWithFields.ContainsKey(satelliteProduct.Id))
                        {
                            satelliteProductsWithFields.Add(satelliteProduct.Id, new List<Common.Models.CropMonitoring.Season>());
                        }
                        satelliteProductsWithFields[satelliteProduct.Id].AddRange(tempSeasons);
                    }
                }

                Console.WriteLine($"satelliteProducts without fields = {satelliteProductsWithFields.Where(m => !m.Value.Any()).Count()}");
                Console.WriteLine($"satelliteProducts with fields = {satelliteProductsWithFields.Where(m => m.Value.Any()).Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
            return satelliteProductsWithFields;
        }

        public async Task Execute()
        {
            var resultCompare = GetSatelluteProductWithSeason();
            var indicesPolygons = new List<BaseIndexPolygon>();
            indicesPolygons.AddRange(MasofaIndicesDbContext.ArviPolygons.Where(m => m.IsColored == true).ToList());
            indicesPolygons.AddRange(MasofaIndicesDbContext.EviPolygons.Where(m => m.IsColored == true).ToList());
            indicesPolygons.AddRange(MasofaIndicesDbContext.GndviPolygons.Where(m => m.IsColored == true).ToList());
            indicesPolygons.AddRange(MasofaIndicesDbContext.MndwiPolygons.Where(m => m.IsColored == true).ToList());
            indicesPolygons.AddRange(MasofaIndicesDbContext.NdmiPolygons.Where(m => m.IsColored == true).ToList());
            indicesPolygons.AddRange(MasofaIndicesDbContext.NdviPolygons.Where(m => m.IsColored == true).ToList());
            indicesPolygons.AddRange(MasofaIndicesDbContext.OrviPolygons.Where(m => m.IsColored == true).ToList());
            indicesPolygons.AddRange(MasofaIndicesDbContext.OsaviPolygons.Where(m => m.IsColored == true).ToList());

            var fields = MasofaCropMonitoringDbContext.Fields.ToList();

            var indexGroup = 0;
            foreach (var group in resultCompare)
            {
                indexGroup++;
                Console.Write($"\r Start for product {group.Key} - {indexGroup} of {resultCompare.Count}");
                var tempPolygons = indicesPolygons.Where(m => m.SatelliteProductId == group.Key).ToList();
                var indexPolygon = 0;
                foreach (var polygon in tempPolygons)
                {
                    indexPolygon++;
                    Console.Write($"\r Start for polygon {polygon.Id} - {indexPolygon} of {tempPolygons.Count}");

                    var indexSeason = 0;
                    foreach (var item in group.Value)
                    {
                        indexSeason++;

                        var field = fields.FirstOrDefault(f => f.Id == item.FieldId);
                        if (polygon.GetType() == typeof(Masofa.Common.Models.Satellite.Indices.ArviPolygon))
                        {
                            MasofaIndicesDbContext.ArviPolygonRelations.Add(new ArviPolygonRelation()
                            {
                                FieldId = item.FieldId,
                                SeasonId = item.Id,
                                RegionId = field?.RegionId,
                                ArviId = polygon.Id,
                            });
                            continue;
                        }

                        if (polygon.GetType() == typeof(Masofa.Common.Models.Satellite.Indices.EviPolygon))
                        {
                            MasofaIndicesDbContext.EviPolygonRelations.Add(new EviPolygonRelation()
                            {
                                FieldId = item.FieldId,
                                SeasonId = item.Id,
                                RegionId = field?.RegionId,
                                EviId = polygon.Id,
                            });
                            continue;

                        }

                        if (polygon.GetType() == typeof(Masofa.Common.Models.Satellite.Indices.GndviPolygon))
                        {
                            MasofaIndicesDbContext.GndviPolygonRelations.Add(new GndviPolygonRelation()
                            {
                                FieldId = item.FieldId,
                                SeasonId = item.Id,
                                RegionId = field?.RegionId,
                                GNdviId = polygon.Id,
                            });
                            continue;

                        }
                        if (polygon.GetType() == typeof(Masofa.Common.Models.Satellite.Indices.MndwiPolygon))
                        {
                            MasofaIndicesDbContext.MndwiPolygonRelations.Add(new MndwiPolygonRelation()
                            {
                                FieldId = item.FieldId,
                                SeasonId = item.Id,
                                RegionId = field?.RegionId,
                                MndWiId = polygon.Id,
                            });
                            continue;

                        }
                        if (polygon.GetType() == typeof(Masofa.Common.Models.Satellite.Indices.NdmiPolygon))
                        {
                            MasofaIndicesDbContext.NdmiPolygonRelations.Add(new NdmiPolygonRelation()
                            {
                                FieldId = item.FieldId,
                                SeasonId = item.Id,
                                RegionId = field?.RegionId,
                                NdMiId = polygon.Id,
                            });
                            continue;

                        }

                        if (polygon.GetType() == typeof(Masofa.Common.Models.Satellite.Indices.NdviPolygon))
                        {
                            MasofaIndicesDbContext.NdviPolygonRelations.Add(new NdviPolygonRelation()
                            {
                                FieldId = item.FieldId,
                                SeasonId = item.Id,
                                RegionId = field?.RegionId,
                                NdViId = polygon.Id,
                            });
                            continue;

                        }

                        if (polygon.GetType() == typeof(Masofa.Common.Models.Satellite.Indices.OrviPolygon))
                        {
                            MasofaIndicesDbContext.OrviPolygonRelations.Add(new OrviPolygonRelation()
                            {
                                FieldId = item.FieldId,
                                SeasonId = item.Id,
                                RegionId = field?.RegionId,
                                OrViId = polygon.Id
                            });
                            continue;

                        }

                        if (polygon.GetType() == typeof(Masofa.Common.Models.Satellite.Indices.OsaviPolygon))
                        {
                            MasofaIndicesDbContext.OsaviPolygonRelations.Add(new OsaviPolygonRelation()
                            {
                                FieldId = item.FieldId,
                                SeasonId = item.Id,
                                RegionId = field?.RegionId,
                                OsaViId = polygon.Id,
                            });
                            continue;

                        }

                        Console.Write($"\r Complite for season {item.Id} - {indexSeason} of {group.Value.Count}");

                    }

                    MasofaIndicesDbContext.SaveChanges();
                }

            }
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
