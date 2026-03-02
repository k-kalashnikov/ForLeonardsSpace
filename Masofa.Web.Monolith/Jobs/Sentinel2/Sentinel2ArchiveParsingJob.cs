using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.Common.Helper;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime.Text;
using Quartz;
using System.Drawing;
using System.IO.Compression;

namespace Masofa.Web.Monolith.Jobs.Sentinel2
{
    /// <summary>
    /// Джоб для парсинга архивов Sentinel2 продуктов
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "Sentinel2ArchiveParsingJob", "Sentinel")]
    public class Sentinel2ArchiveParsingJob : BaseJob<Sentinel2ArchiveParsingJobResult>, IJob
    {
        private readonly ILogger<Sentinel2ArchiveParsingJob> _logger;
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly MasofaCommonDbContext MasofaCommonDbContext;
        private readonly MasofaSentinelDbContext SentinelDbContext;
        private readonly MasofaDictionariesDbContext MasofaDictionariesDbContext;

        public Sentinel2ArchiveParsingJob(
            ILogger<Sentinel2ArchiveParsingJob> logger,
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext commonDbContext,
            MasofaSentinelDbContext sentinelDbContext,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            MasofaIdentityDbContext identityDbContext,
            MasofaDictionariesDbContext masofaDictionariesDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            _logger = logger;
            _fileStorageProvider = fileStorageProvider;
            MasofaCommonDbContext = commonDbContext;
            SentinelDbContext = sentinelDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start Sentinel2ArchiveParsingJob");

            try
            {
                var result = new List<Sentinel2ProductQueue>();
                List<Sentinel2ProductQueue> productQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                    .Where(m => m.QueueStatus == ProductQueueStatusType.MediaLoaded)
                    .ToListAsync();

                var pqIds = productQueue.Select(m => m.ProductId).ToList();

                var products = await MasofaCommonDbContext.SatelliteProducts
                    .Where(sp => pqIds.Contains(sp.ProductId ?? string.Empty))
                    .ToListAsync();

                var pIds = products.Select(m => m.Id).ToList();

                var files = await MasofaCommonDbContext.FileStorageItems
                    .Where(fs => fs.FileStorageBacket == "sentinel")
                    .Where(fs => pIds.Contains(fs.OwnerId))
                    .ToListAsync();

                var regions = await MasofaDictionariesDbContext.Regions
                    .Where(r => r.Level == 3)
                    .ToListAsync();

                var regionMaps = await MasofaDictionariesDbContext.RegionMaps
                    .Where(r => r.Polygon != null)
                    .ToListAsync();

                var index = 0;

                foreach (var file in files)
                {
                    try
                    {
                        Console.WriteLine($"Processing archive: {file.FileStoragePath}");
                        using var archiveStream = await _fileStorageProvider.GetFileStreamAsync(file);
                        using var zip = new ZipArchive(archiveStream, ZipArchiveMode.Read);

                        string? productId = null;
                        Guid? l1cMetadataId = null;
                        Guid? inspireMetadataId = null;
                        Guid? tileMetadataId = null;
                        Guid? qualityMetadataId = null;

                        var parseResult = new SentinelInspireMetadata();

                        // Сначала обрабатываем mtd_msil1c файл для извлечения даты спутника
                        DateTime? satelliteDate = null;
                        var mtdEntry = zip.Entries.FirstOrDefault(e => e.Name.ToLowerInvariant().Contains("mtd_msil1c"));

                        foreach (var entry in zip.Entries)
                        {
                            if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                            {
                                var name = entry.Name.ToLowerInvariant();

                                // Пропускаем уже обработанный mtd_msil1c файл
                                if (name.Contains("mtd_msil1c"))
                                    continue;

                                if (name.Contains("inspire"))
                                {
                                    using (var stream = new StreamReader(entry.Open()))
                                    {
                                        string xmlContent = await stream.ReadToEndAsync();
                                        string simplifiedXml = Sentinel2ArchiveParserHelper.SimplifyInspireMetadataXml(xmlContent);
                                        parseResult = Sentinel2ArchiveParserHelper.ParseInspireMetadataAsyncAsString(simplifiedXml);
                                        inspireMetadataId = await SaveSentinelInspireParseResultAsync(parseResult);
                                        Console.WriteLine($"INSPIRE metadata saved with ID: {inspireMetadataId}");
                                    }

                                }

                            }
                        }

                        // Получаем продукт для дальнейшего использования
                        var pId = products.First(m => m.Id.Equals(file.OwnerId));

                        var sentinelProductEntity = new Sentinel2ProductEntity
                        {
                            SatellateProductId = pId.Id.ToString(),
                            SentinelL1CProductMetadataId = l1cMetadataId,
                            SentinelInspireMetadataId = inspireMetadataId,
                            SentinelL1CTileMetadataId = tileMetadataId,
                            SentinelProductQualityMetadataId = qualityMetadataId
                        };

                        SentinelDbContext.Sentinel2Products.Add(sentinelProductEntity);

                        await SaveSatelliteRegionRelation(pId, regions, regionMaps);

                        // Обновляем статус в очереди
                        var productQueueItem = productQueue.First(pq => pq.ProductId.Equals(pId.ProductId));
                        productQueueItem.QueueStatus = ProductQueueStatusType.Parsed;
                        SentinelDbContext.Set<Sentinel2ProductQueue>().Update(productQueueItem);
                        await SentinelDbContext.SaveChangesAsync();
                        result.Add(productQueueItem);
                        Result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        Result.Errors.Add($"Error processing file {file.FileStoragePath}: {ex.Message}");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.InnerException?.Message);
                    }
                }

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
                }, context);
            }
            catch (Exception ex)
            {
                Result.Errors.Add(ex.Message);
                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }

            _logger.LogInformation("End Sentinel2ArchiveParsingJob");
        }

        private async Task<Guid?> SaveSentinelInspireParseResultAsync(SentinelInspireMetadata parseResult)
        {
            if (parseResult != null)
            {
                var result = (await SentinelDbContext.SentinelInspireMetadata.AddAsync(parseResult)).Entity;
                await SentinelDbContext.SaveChangesAsync();
                return result.Id;
            }
            return null;
        }

        private async Task<List<Guid?>?> SaveSatelliteRegionRelation(SatelliteProduct satelliteProduct, List<Common.Models.Dictionaries.Region> regions, List<RegionMap> regionMaps)
        {
            try
            {

                var relations = new List<SatelliteRegionRelation?>();

                if (satelliteProduct == null || satelliteProduct.Polygon == null)
                {
                    return null;
                }

                var poly = satelliteProduct.Polygon;
                poly.SRID = 4326;

                var regionMapIds = regionMaps
                    .Where(r => r.Polygon.Intersects(poly))
                    .Select(r => r.Id)
                    .ToList();

                foreach (var regionMapId in regionMapIds)
                {
                    var region = regions.Find(r => r.RegionMapId == regionMapId);
                    if (region == null) { continue; }

                    var exists = relations
                        .Any(r => r.SatelliteProductId == satelliteProduct.Id && r.RegionId == region.Id);

                    if (!exists)
                    {
                        relations.Add(new SatelliteRegionRelation()
                        {
                            RegionId = region.Id,
                            SatelliteProductId = satelliteProduct.Id
                        });
                    }
                }

                try
                {
                    await MasofaCommonDbContext.SatelliteRegionRelations.AddRangeAsync(relations);
                    await MasofaCommonDbContext.SaveChangesAsync();

                    return relations.Select(r => (Guid?)r.Id).ToList();
                }
                catch (Exception ex)
                {
                    Result.Errors.Add(ex.Message);
                    _logger.LogError($"Error in savechanges SatelliteProductRegionRelation \n Message: {ex.Message}. \n InnerException: {ex.InnerException}");

                    return null;
                }
            }
            catch (Exception ex)
            {
                Result.Errors.Add(ex.Message);
                _logger.LogError($"Error in finding region for product. \n Message: {ex.Message}. \n InnerException: {ex.InnerException}");

                return null;
            }
            //try
            //{
            //    double westBound = (double)parseResult.WestBoundLongitude;
            //    double eastBound = (double)parseResult.EastBoundLongitude;
            //    double southBound = (double)parseResult.SouthBoundLatitude;
            //    double northBound = (double)parseResult.NorthBoundLatitude;

            //    var envelope = new Envelope(westBound, eastBound, southBound, northBound);
            //    var geometryFactory = new GeometryFactory();
            //    var geometry = geometryFactory.ToGeometry(envelope);

            //    var regionMapIds = await MasofaDictionariesDbContext.RegionMaps
            //        .Where(r => r.Polygon.Intersects(geometry))
            //        .Select(r => r.Id)
            //        .ToListAsync();

            //    if (!regionMapIds.Any())
            //    {
            //        return new List<Guid?>();
            //    }

            //    var relations = new List<SatelliteRegionRelation?>();
            //    var regions = await MasofaDictionariesDbContext.Regions
            //        .Where(r => r.Level == 3)
            //        .ToListAsync();

            //    foreach (var regionMapId in regionMapIds)
            //    {
            //        var region = regions.Find(r => r.RegionMapId == regionMapId);

            //        if (region == null)
            //        {
            //            continue;
            //        }

            //        var satelliteRegionRelation = new SatelliteRegionRelation()
            //        {
            //            RegionId = region.Id,
            //            SatelliteProductId = ProductId,
            //        };

            //        relations.Add(satelliteRegionRelation);
            //    }

            //    await CommonDbContext.SatelliteRegionRelations.AddRangeAsync(relations);
            //    await CommonDbContext.SaveChangesAsync();

            //    return relations.Select(r => (Guid?)r.Id).ToList();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Error in finding region for product. \n Message: {ex.Message}. \n InnerException: {ex.InnerException}");
            //    return null;
            //}
        }
    }

    public class Sentinel2ArchiveParsingJobResult : BaseJobResult
    {
        public int SuccessCount { get; set; } = 0;
    }
}