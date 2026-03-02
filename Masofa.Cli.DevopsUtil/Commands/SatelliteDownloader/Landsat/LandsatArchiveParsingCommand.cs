//using Masofa.Common.Services.FileStorage;
//using Masofa.Common.Models.Satellite.Landsat;
//using Masofa.DataAccess;
//using Microsoft.Extensions.Logging;
//using System.IO.Compression;
//using System.Text.Json;
//using NetTopologySuite.Geometries;
//using NetTopologySuite.Geometries.Implementation;
//using Microsoft.EntityFrameworkCore;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.System;
//using Masofa.BusinessLogic.Jobs.Parsers;
//using Masofa.Common.Helper;
//using Masofa.Common.Models.Satellite.Parse.Landsat;
//using Masofa.BusinessLogic.FieldSatellite.Commands;
//using Masofa.BusinessLogic.FieldSatellite.Helpers;
//using Masofa.DataAccess.Models.Landsat;
//using MediatR;
//using System.Text.Json.Nodes;

//namespace Masofa.Cli.DevopsUtil.Commands.Satellite
//{
//    [BaseCommand("Landsat Archive Parsing", "Парсинг архивных данных Landsat из очереди")]
//    public class LandsatArchiveParsingCommand : IBaseCommand
//    {
//        private readonly IFileStorageProvider _fileStorageProvider;
//        private readonly LandsatArchiveParserHelper _parserHelper;
//        private readonly IMediator _mediator;
//        private readonly MasofaLandsatDbContext _landsatDbContext;
//        private readonly ILogger<LandsatArchiveParsingCommand> _logger;
//        private MasofaLandsatDbContext LandsatDbContext { get; set; }
//        private MasofaCommonDbContext CommonDbContext { get; set; }
//        private readonly GeometryCalculationHelper _geometryHelper;


//        public LandsatArchiveParsingCommand(
//            IFileStorageProvider fileStorageProvider,
//            LandsatArchiveParserHelper parserHelper,
//            IMediator mediator,
//            ILogger<LandsatArchiveParsingCommand> logger,
//            MasofaLandsatDbContext landsatDbContext,
//            MasofaCommonDbContext commonDbContext,
//            GeometryCalculationHelper geometryHelper)
//        {
//            _fileStorageProvider = fileStorageProvider;
//            _parserHelper = parserHelper;
//            _mediator = mediator;
//            _landsatDbContext = landsatDbContext;
//            _logger = logger;
//            LandsatDbContext = landsatDbContext;
//            CommonDbContext = commonDbContext;
//            _geometryHelper = geometryHelper;
//        }

//        public async Task Execute()
//        {
//            var productQueue = await LandsatDbContext.Set<LandsatProductQueue>()
//                .Where(m => m.Status == ProductQueueStatusType.MediaLoaded)
//                .ToListAsync();

//            if (!productQueue.Any())
//            {
//                return;
//            }

//            var products = await CommonDbContext.SatelliteProducts
//                .Where(sp => productQueue.Any(pq => pq.ProductId.Equals(sp.ProductId)))
//                .ToListAsync();

//            if (!products.Any())
//            {
//                return;
//            }

//            var files = await CommonDbContext.FileStorageItems
//                .Where(fs => products.Any(p => p.Id.Equals(fs.OwnerId)))
//                .ToListAsync();

//            if (!files.Any())
//            {
//                return;
//            }

//            foreach (var item in files)
//            {
//                try
//                {
//                    _logger.LogInformation($"Processing archive: {item.FileStoragePath}");
//                    string? productId = null;

//                    using (var archiveStream = await _fileStorageProvider.GetFileStreamAsync(item))
//                    {
//                        using (var zip = new ZipArchive(archiveStream, ZipArchiveMode.Read))
//                        {

//                            foreach (var entry in zip.Entries)
//                            {
//                                if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
//                                {
//                                    using var entryStream = entry.Open();
//                                    var name = entry.Name.ToLowerInvariant();
//                                    if (name.Contains("mtl"))
//                                    {
//                                        var parseResult = await _parserHelper.ParseMtlFileAsync(entryStream);
                                        
//                                        // Сохраняем все Entity в БД
//                                        await SaveLandsatParseResultAsync(parseResult);
                                        
//                                        if (productId == null)
//                                        {
//                                            productId = parseResult.SourceMetadata.SatellateProductId;
//                                        }
//                                    }
//                                    else if (name.Contains("sr"))
//                                    {
//                                        var parseResult = await _parserHelper.ParseSrStacFileAsync(entryStream);
                                        
//                                        // Сохраняем все Entity в БД
//                                        await SaveLandsatParseResultAsync(parseResult);
                                        
//                                        if (productId == null)
//                                        {
//                                            productId = parseResult.Feature.FeatureId;
//                                        }
//                                    }
//                                    else if (name.Contains("st"))
//                                    {
//                                        var parseResult = await _parserHelper.ParseStStacFileAsync(entryStream);
                                        
//                                        // Сохраняем все Entity в БД
//                                        await SaveLandsatParseResultAsync(parseResult);
                                        
//                                        if (productId == null)
//                                        {
//                                            productId = parseResult.Feature.FeatureId;
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }

//                    // Обрабатываем маппинг один раз после полного парсинга архива
//                    if (!string.IsNullOrEmpty(productId))
//                    {
//                        await ProcessProductMappingAsync(productId);
//                    }

//                    var productQueueItem = productQueue.FirstOrDefault(pq => pq.ProductId.Equals(products.FirstOrDefault(p => p.Id.Equals(item.OwnerId)).Id));
//                    productQueueItem.Status = ProductQueueStatusType.Parsed;
//                    LandsatDbContext.LandsatProductsQueue.Update(productQueueItem);
//                    LandsatDbContext.SaveChanges();
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, $"Ошибка при обработке архива {item.FileStorageBacket}=>{item.FileStoragePath}");
//                }

//            }
//        }

//        private async Task SaveLandsatParseResultAsync(LandsatParseResult parseResult)
//        {
//            try
//            {
//                using var transaction = await _landsatDbContext.Database.BeginTransactionAsync();
                
//                try
//                {
//                    // Сохраняем SourceMetadata
//                    if (parseResult.SourceMetadata != null)
//                    {
//                        _landsatDbContext.LandsatSourceMetadata.Add(parseResult.SourceMetadata);
//                    }
                    
//                    // Сохраняем ProductContents
//                    if (parseResult.ProductContents != null)
//                    {
//                        _landsatDbContext.ProductContents.Add(parseResult.ProductContents);
//                    }
                    
//                    // Сохраняем ImageAttributes
//                    if (parseResult.ImageAttributes != null)
//                    {
//                        _landsatDbContext.ImageAttributes.Add(parseResult.ImageAttributes);
//                    }
                    
//                    // Сохраняем ProjectionAttributes
//                    if (parseResult.ProjectionAttributes != null)
//                    {
//                        _landsatDbContext.ProjectionAttributes.Add(parseResult.ProjectionAttributes);
//                    }
                    
//                    // Сохраняем Level2ProcessingRecord
//                    if (parseResult.Level2ProcessingRecord != null)
//                    {
//                        _landsatDbContext.Level2ProcessingRecords.Add(parseResult.Level2ProcessingRecord);
//                    }
                    
//                    // Сохраняем Level2SurfaceReflectanceParameters
//                    if (parseResult.Level2SurfaceReflectanceParameters != null)
//                    {
//                        _landsatDbContext.Level2SurfaceReflectanceParameters.Add(parseResult.Level2SurfaceReflectanceParameters);
//                    }
                    
//                    // Сохраняем Level2SurfaceTemperatureParameters
//                    if (parseResult.Level2SurfaceTemperatureParameters != null)
//                    {
//                        _landsatDbContext.Level2SurfaceTemperatureParameters.Add(parseResult.Level2SurfaceTemperatureParameters);
//                    }
                    
//                    // Сохраняем Level1ProcessingRecord
//                    if (parseResult.Level1ProcessingRecord != null)
//                    {
//                        _landsatDbContext.Level1ProcessingRecords.Add(parseResult.Level1ProcessingRecord);
//                    }
                    
//                    // Сохраняем Level1MinMaxRadiance
//                    if (parseResult.Level1MinMaxRadiance != null)
//                    {
//                        _landsatDbContext.Level1MinMaxRadiance.Add(parseResult.Level1MinMaxRadiance);
//                    }
                    
//                    // Сохраняем Level1MinMaxReflectance
//                    if (parseResult.Level1MinMaxReflectance != null)
//                    {
//                        _landsatDbContext.Level1MinMaxReflectance.Add(parseResult.Level1MinMaxReflectance);
//                    }
                    
//                    // Сохраняем Level1MinMaxPixelValue
//                    if (parseResult.Level1MinMaxPixelValue != null)
//                    {
//                        _landsatDbContext.Level1MinMaxPixelValue.Add(parseResult.Level1MinMaxPixelValue);
//                    }
                    
//                    // Сохраняем Level1RadiometricRescaling
//                    if (parseResult.Level1RadiometricRescaling != null)
//                    {
//                        _landsatDbContext.Level1RadiometricRescaling.Add(parseResult.Level1RadiometricRescaling);
//                    }
                    
//                    // Сохраняем Level1ThermalConstants
//                    if (parseResult.Level1ThermalConstants != null)
//                    {
//                        _landsatDbContext.Level1ThermalConstants.Add(parseResult.Level1ThermalConstants);
//                    }
                    
//                    // Сохраняем Level1ProjectionParameters
//                    if (parseResult.Level1ProjectionParameters != null)
//                    {
//                        _landsatDbContext.Level1ProjectionParameters.Add(parseResult.Level1ProjectionParameters);
//                    }
                    
//                    await _landsatDbContext.SaveChangesAsync();
//                    await transaction.CommitAsync();
//                }
//                catch
//                {
//                    await transaction.RollbackAsync();
//                    throw;
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error saving Landsat parse result to database");
//                throw;
//            }
//        }
        
//        private async Task SaveLandsatStacParseResultAsync(LandsatStacParseResult parseResult)
//        {
//            try
//            {
//                using var transaction = await _landsatDbContext.Database.BeginTransactionAsync();
                
//                try
//                {
//                    // Сохраняем StacFeature
//                    if (parseResult.Feature != null)
//                    {
//                        _landsatDbContext.StacFeatures.Add(parseResult.Feature);
//                    }
                    
//                    // Сохраняем StacLinks
//                    if (parseResult.Links != null && parseResult.Links.Any())
//                    {
//                        _landsatDbContext.StacLinks.AddRange(parseResult.Links);
//                    }
                    
//                    // Сохраняем StacAssets
//                    if (parseResult.Assets != null && parseResult.Assets.Any())
//                    {
//                        _landsatDbContext.StacAssets.AddRange(parseResult.Assets);
//                    }
                    
//                    await _landsatDbContext.SaveChangesAsync();
//                    await transaction.CommitAsync();
//                }
//                catch
//                {
//                    await transaction.RollbackAsync();
//                    throw;
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error saving Landsat STAC parse result to database");
//                throw;
//            }
//        }

//        private async Task ProcessProductMappingAsync(string productId)
//        {
//            try
//            {
//                // Получаем геометрию продукта из STAC
//                var stacFeature = await _landsatDbContext.StacFeatures
//                    .FirstOrDefaultAsync(sf => sf.FeatureId == productId);

//                if (stacFeature?.GeometryJson != null)
//                {
//                    var productGeometry = _geometryHelper.ParseGeometryFromJson(stacFeature.GeometryJson);
//                    if (productGeometry != null)
//                    {
//                        await _mediator.Send(new ProcessNewProductCommand(
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

//        public Task Execute(string[] args)
//        {
//            throw new NotImplementedException();
//        }

//        public void Dispose()
//        {

//        }
//    }
//}
