//using Masofa.BusinessLogic.Services.BusinessLogicLogger;
//using Masofa.Common.Attributes;
//using Masofa.Common.Helper;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Services.FileStorage;
//using Masofa.DataAccess;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using NetTopologySuite.Geometries;
//using Quartz;
//using System.IO.Compression;
//using System.Text.Json;

//namespace Masofa.Web.Monolith.Jobs.Landsat
//{
//    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "LandsatArchiveParsingJob")]
//    public class LandsatArchiveParsingJob : BaseJob<string>
//    {
//        private readonly ILogger<LandsatArchiveParsingJob> _logger;
//        private readonly IFileStorageProvider _fileStorageProvider;
//        private readonly LandsatArchiveParserHelper _parserHelper;
//        private readonly MasofaLandsatDbContext _landsatDbContext;
//        private readonly MasofaCommonDbContext _commonDbContext;

//        public LandsatArchiveParsingJob(
//            ILogger<LandsatArchiveParsingJob> logger,
//            IFileStorageProvider fileStorageProvider,
//            LandsatArchiveParserHelper parserHelper,
//            MasofaLandsatDbContext landsatDbContext,
//            MasofaCommonDbContext commonDbContext,
//            IConfiguration configuration,
//            IMediator mediator,
//            IBusinessLogicLogger businessLogicLogger) : base(mediator, businessLogicLogger, logger)
//        {
//            _logger = logger;
//            _fileStorageProvider = fileStorageProvider;
//            _parserHelper = parserHelper;
//            _landsatDbContext = landsatDbContext;
//            _commonDbContext = commonDbContext;
//        }

//        public override async Task Execute(IJobExecutionContext context)
//        {
//            _logger.LogInformation("Start LandsatArchiveParsingJob");

//            var processedCount = 0;
//            var errors = new List<string>();

//            try
//            {
//                var dataMap = context.JobDetail.JobDataMap;
//                var productIdsString = dataMap.GetString("ProductIds");
//                var chainId = dataMap.GetString("ChainID");

//                List<LandsatProductQueue> productQueue;

//                if (!string.IsNullOrEmpty(productIdsString) && !string.IsNullOrEmpty(chainId))
//                {
//                    var productIds = productIdsString.Split(',').ToList();
//                    productQueue = await _landsatDbContext.Set<LandsatProductQueue>()
//                        .Where(m => m.Status == ProductQueueStatusType.MediaLoaded && productIds.Contains(m.ProductId))
//                        .ToListAsync();

//                    _logger.LogInformation("Processing specific products for chain {ChainID}: {Count} items", chainId, productQueue.Count);
//                }
//                else
//                {
//                    productQueue = await _landsatDbContext.Set<LandsatProductQueue>()
//                        .Where(m => m.Status == ProductQueueStatusType.MediaLoaded)
//                        .Take(10)
//                        .ToListAsync();
//                }

//                if (!productQueue.Any())
//                {
//                    await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                    {
//                        ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success,
//                        TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { 
//                            processedCount = 0,
//                            message = "No products to process"
//                        }),
//                        TaskResultJsonType = typeof(string)
//                    }, context);
//                    return;
//                }

//                var pqIds = productQueue.Select(m => m.ProductId).ToList();

//                var products = await _commonDbContext.SatelliteProducts
//                    .Where(sp => pqIds.Contains(sp.ProductId ?? string.Empty))
//                    .ToListAsync();

//                if (!products.Any())
//                {
//                    await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                    {
//                        ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success,
//                        TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { 
//                            processedCount = 0,
//                            message = "No satellite products found"
//                        }),
//                        TaskResultJsonType = typeof(string)
//                    }, context);
//                    return;
//                }

//                var pIds = products.Select(m => m.Id).ToList();

//                var files = await _commonDbContext.FileStorageItems
//                    .Where(fs => pIds.Contains(fs.OwnerId))
//                    .ToListAsync();

//                foreach (var file in files)
//                {
//                    try
//                    {
//                        _logger.LogInformation("Processing archive: {FileStoragePath}", file.FileStoragePath);

//                        using var archiveStream = await _fileStorageProvider.GetFileStreamAsync(file);
//                        using var zip = new ZipArchive(archiveStream, ZipArchiveMode.Read);

//                        string? productId = null;

//                        foreach (var entry in zip.Entries)
//                        {
//                            if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
//                            {
//                                using var entryStream = entry.Open();
//                                var name = entry.Name.ToLowerInvariant();

//                                if (name.Contains("mtl"))
//                                {
//                                    var parseResult = await _parserHelper.ParseMtlFileAsync(entryStream);
//                                    await SaveLandsatParseResultAsync(parseResult);

//                                    if (productId == null)
//                                        productId = parseResult.SourceMetadata.SatellateProductId;
//                                }
//                            }
//                            else if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
//                            {
//                                using var entryStream = entry.Open();
//                                var name = entry.Name.ToLowerInvariant();

//                                if (name.Contains("stac"))
//                                {
//                                    // Определяем тип STAC файла по имени
//                                    if (name.Contains("sr"))
//                                    {
//                                        var parseResult = await _parserHelper.ParseSrStacFileAsync(entryStream);
//                                        await SaveLandsatStacParseResultAsync(parseResult);

//                                        if (productId == null)
//                                            productId = parseResult.Feature.FeatureId;
//                                    }
//                                    else if (name.Contains("st"))
//                                    {
//                                        var parseResult = await _parserHelper.ParseStStacFileAsync(entryStream);
//                                        await SaveLandsatStacParseResultAsync(parseResult);

//                                        if (productId == null)
//                                            productId = parseResult.Feature.FeatureId;
//                                    }
//                                }
//                            }
//                        }

//                        if (!string.IsNullOrEmpty(productId))
//                        {
//                            await ProcessProductMappingAsync(productId);
//                        }

//                        var pId = products.First(m => m.Id.Equals(file.OwnerId));
//                        var productQueueItem = productQueue.First(pq => pq.ProductId.Equals(pId.ProductId));
//                        productQueueItem.Status = ProductQueueStatusType.Parsed;
//                        _landsatDbContext.Set<LandsatProductQueue>().Update(productQueueItem);
//                        _landsatDbContext.SaveChanges();

//                        processedCount++;
//                    }
//                    catch (Exception ex)
//                    {
//                        errors.Add($"Error processing archive {file.FileStorageBacket}=>{file.FileStoragePath}: {ex.Message}");
//                        _logger.LogError(ex, "Error processing archive {FileStorageBacket}=>{FileStoragePath}", file.FileStorageBacket, file.FileStoragePath);
//                    }
//                }

//                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                {
//                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success,
//                    TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { 
//                        processedCount = processedCount,
//                        totalInQueue = productQueue.Count,
//                        errors = errors
//                    }),
//                    TaskResultJsonType = typeof(string)
//                }, context);
//            }
//            catch (Exception ex)
//            {
//                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                {
//                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed,
//                    TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { 
//                        error = ex.Message,
//                        processedCount = processedCount,
//                        errors = errors
//                    }),
//                    TaskResultJsonType = typeof(string)
//                }, context);
//            }

//            _logger.LogInformation("End LandsatArchiveParsingJob");
//        }

//        private async Task SaveLandsatParseResultAsync(LandsatParseResult parseResult)
//        {
//            using var transaction = await _landsatDbContext.Database.BeginTransactionAsync();

//            try
//            {
//                if (parseResult.SourceMetadata != null)
//                    _landsatDbContext.LandsatSourceMetadata.Add(parseResult.SourceMetadata);

//                if (parseResult.ProductContents != null)
//                    _landsatDbContext.ProductContents.Add(parseResult.ProductContents);

//                if (parseResult.ImageAttributes != null)
//                    _landsatDbContext.ImageAttributes.Add(parseResult.ImageAttributes);

//                if (parseResult.ProjectionAttributes != null)
//                    _landsatDbContext.ProjectionAttributes.Add(parseResult.ProjectionAttributes);

//                if (parseResult.Level2ProcessingRecord != null)
//                    _landsatDbContext.Level2ProcessingRecords.Add(parseResult.Level2ProcessingRecord);

//                if (parseResult.Level2SurfaceReflectanceParameters != null)
//                    _landsatDbContext.Level2SurfaceReflectanceParameters.Add(parseResult.Level2SurfaceReflectanceParameters);

//                if (parseResult.Level2SurfaceTemperatureParameters != null)
//                    _landsatDbContext.Level2SurfaceTemperatureParameters.Add(parseResult.Level2SurfaceTemperatureParameters);

//                if (parseResult.Level1ProcessingRecord != null)
//                    _landsatDbContext.Level1ProcessingRecords.Add(parseResult.Level1ProcessingRecord);

//                if (parseResult.Level1MinMaxRadiance != null)
//                    _landsatDbContext.Level1MinMaxRadiances.Add(parseResult.Level1MinMaxRadiance);

//                if (parseResult.Level1MinMaxReflectance != null)
//                    _landsatDbContext.Level1MinMaxReflectances.Add(parseResult.Level1MinMaxReflectance);

//                if (parseResult.Level1MinMaxPixelValue != null)
//                    _landsatDbContext.Level1MinMaxPixelValues.Add(parseResult.Level1MinMaxPixelValue);

//                if (parseResult.Level1RadiometricRescaling != null)
//                    _landsatDbContext.Level1RadiometricRescalings.Add(parseResult.Level1RadiometricRescaling);

//                if (parseResult.Level1ThermalConstants != null)
//                    _landsatDbContext.Level1ThermalConstants.Add(parseResult.Level1ThermalConstants);

//                if (parseResult.Level1ProjectionParameters != null)
//                    _landsatDbContext.Level1ProjectionParameters.Add(parseResult.Level1ProjectionParameters);

//                await _landsatDbContext.SaveChangesAsync();
//                await transaction.CommitAsync();
//            }
//            catch
//            {
//                await transaction.RollbackAsync();
//                throw;
//            }
//        }

//        private async Task SaveLandsatStacParseResultAsync(LandsatStacParseResult parseResult)
//        {
//            using var transaction = await _landsatDbContext.Database.BeginTransactionAsync();

//            try
//            {
//                if (parseResult.Feature != null)
//                    _landsatDbContext.StacFeatures.Add(parseResult.Feature);

//                if (parseResult.Links != null && parseResult.Links.Any())
//                    _landsatDbContext.StacLinks.AddRange(parseResult.Links);

//                if (parseResult.Assets != null && parseResult.Assets.Any())
//                    _landsatDbContext.StacAssets.AddRange(parseResult.Assets);

//                await _landsatDbContext.SaveChangesAsync();
//                await transaction.CommitAsync();
//            }
//            catch
//            {
//                await transaction.RollbackAsync();
//                throw;
//            }
//        }

//        private async Task ProcessProductMappingAsync(string productId)
//        {
//            try
//            {
//                var stacFeature = await _landsatDbContext.StacFeatures
//                    .FirstOrDefaultAsync(sf => sf.FeatureId == productId);

//                if (stacFeature?.GeometryJson != null)
//                {
//                    var productGeometry = ParseGeometryFromJson(stacFeature.GeometryJson);
//                    if (productGeometry != null)
//                    {
//                        await Mediator.Send(new Masofa.BusinessLogic.FieldSatellite.Commands.ProcessNewProductCommand(
//                            productId,
//                            ProductSourceType.Landsat,
//                            productGeometry
//                        ));
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error processing product mapping for {ProductId}", productId);
//            }
//        }

//        private Polygon? ParseGeometryFromJson(string geometryJson)
//        {
//            try
//            {
//                var geometryData = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonElement>(geometryJson);

//                if (geometryData.TryGetProperty("coordinates", out var coordinatesElement))
//                {
//                    var coordinates = coordinatesElement.EnumerateArray().FirstOrDefault();
//                    if (coordinates.ValueKind == JsonValueKind.Array)
//                    {
//                        var coords = new List<Coordinate>();
//                        foreach (var coord in coordinates.EnumerateArray())
//                        {
//                            if (coord.ValueKind == JsonValueKind.Array)
//                            {
//                                var coordArray = coord.EnumerateArray().ToArray();
//                                if (coordArray.Length >= 2)
//                                {
//                                    coords.Add(new Coordinate(coordArray[0].GetDouble(), coordArray[1].GetDouble()));
//                                }
//                            }
//                        }

//                        if (coords.Count >= 4)
//                        {
//                            var shell = new LinearRing(coords.ToArray());
//                            return new Polygon(shell);
//                        }
//                    }
//                }

//                return null;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogWarning(ex, "Error parsing geometry from JSON: {GeometryJson}", geometryJson);
//                return null;
//            }
//        }
//    }
//}
