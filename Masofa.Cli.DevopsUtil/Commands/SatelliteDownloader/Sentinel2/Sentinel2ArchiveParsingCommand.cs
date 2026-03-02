//using Masofa.BusinessLogic.Jobs.Parsers;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.System;
//using Masofa.Common.Services.FileStorage;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using NetTopologySuite.Geometries;
//using System.IO.Compression;
//using Masofa.Common.Helper;
//using Masofa.Common.Models.Satellite.Parse.Sentinel2;
//using Masofa.BusinessLogic.FieldSatellite.Commands;

//namespace Masofa.Cli.DevopsUtil.Commands.Satellite
//{
//    [BaseCommand("Sentinel2 Archive Parsing", "Парсинг архивных данных Sentinel2 из очереди")]
//    public class Sentinel2ArchiveParsingCommand : IBaseCommand
//    {
//        private IFileStorageProvider FileStorageProvider { get; set; }
//        private Sentinel2ArchiveParserHelper ParserHelper { get; set; }
//        private IMediator Mediator { get; set; }
//        private ILogger<Sentinel2ArchiveParsingCommand> Logger { get; set; }
//        private MasofaSentinelDbContext SentinelDbContext { get; set; }
//        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }

//        public Sentinel2ArchiveParsingCommand(
//            IFileStorageProvider fileStorageProvider,
//            Sentinel2ArchiveParserHelper parserHelper,
//            IMediator mediator,
//            ILogger<Sentinel2ArchiveParsingCommand> logger,
//            MasofaSentinelDbContext sentinelDbContext,
//            MasofaCommonDbContext masofaCommonDbContext)
//        {
//            FileStorageProvider = fileStorageProvider;
//            ParserHelper = parserHelper;
//            Logger = logger;
//            Mediator = mediator;
//            SentinelDbContext = sentinelDbContext;
//            MasofaCommonDbContext = masofaCommonDbContext;
//        }

//        public async Task Execute()
//        {
//            var productQueue = await SentinelDbContext.Sentinel2ProductsQueue
//                .Where(m => m.Status == ProductQueueStatusType.MediaLoaded)
//                .ToListAsync();

//            if (!productQueue.Any())
//            {
//                return;
//            }

//            var pqIds = productQueue.Select(m => m.ProductId).ToList();

//            var products = await MasofaCommonDbContext.SatelliteProducts
//                .Where(sp => pqIds.Contains(sp.ProductId ?? string.Empty))
//                .ToListAsync();

//            if (!products.Any())
//            {
//                return;
//            }

//            var pIds = products.Select(m => m.Id).ToList();

//            var files = await MasofaCommonDbContext.FileStorageItems
//                .Where(fs => pIds.Contains(fs.OwnerId))
//                .ToListAsync();

//            foreach (var file in files)
//            {
//                try
//                {
//                    Logger.LogInformation($"Processing archive: {file.FileStoragePath}");
//                    using (var archiveStream = await FileStorageProvider.GetFileStreamAsync(file))
//                    {
//                        using (var zip = new ZipArchive(archiveStream, ZipArchiveMode.Read))
//                        {
//                            string? productId = null;
//                            foreach (var entry in zip.Entries)
//                            {
//                                if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
//                                {
//                                    using var entryStream = entry.Open();
//                                    var name = entry.Name.ToLowerInvariant();
//                                    if (name.Contains("mtd_msil1c"))
//                                    {
//                                        var parseResult = await ParserHelper.ParseL1CProductMetadataAsync(entryStream);
//                                        await SaveSentinelL1CParseResultAsync(parseResult);
                                        
//                                        if (productId == null)
//                                        {
//                                            productId = parseResult.Metadata.SatellateProductId;
//                                        }
//                                    }
//                                    else if (name.Contains("inspire"))
//                                    {
//                                        var parseResult = await ParserHelper.ParseInspireMetadataAsync(entryStream);
//                                        await SaveSentinelInspireParseResultAsync(parseResult);
                                        
//                                        if (productId == null)
//                                        {
//                                            productId = parseResult.Metadata.SatellateProductId;
//                                        }
//                                    }
//                                    else if (name.Contains("mtd_tl"))
//                                    {
//                                        var parseResult = await ParserHelper.ParseL1CTileMetadataAsync(entryStream);
//                                        await SaveSentinelTileParseResultAsync(parseResult);
                                        
//                                        if (productId == null)
//                                        {
//                                            productId = parseResult.Metadata.SatellateProductId;
//                                        }
//                                    }
//                                    else if (name.Contains("general_quality"))
//                                    {
//                                        var parseResult = await ParserHelper.ParseProductQualityMetadataAsync(entryStream);
//                                        await SaveSentinelQualityParseResultAsync(parseResult);
                                        
//                                        if (productId == null)
//                                        {
//                                            productId = parseResult.Metadata.SatellateProductId;
//                                        }
//                                    }
//                                }
//                            }

//                            // Обрабатываем маппинг один раз после полного парсинга архива
//                            if (!string.IsNullOrEmpty(productId))
//                            {
//                                await ProcessProductMappingAsync(productId);
//                            }
//                        }
//                    }

//                    var pId = products.First(m => m.Id.Equals(file.OwnerId));
//                    var productQueueItem = productQueue.First(pq => pq.ProductId.Equals(pId.ProductId));
//                    productQueueItem.Status = ProductQueueStatusType.Parsed;
//                    SentinelDbContext.Sentinel2ProductsQueue.Update(productQueueItem);
//                    SentinelDbContext.SaveChanges();
//                }
//                catch (Exception ex)
//                {
//                    Logger.LogError(ex, $"Ошибка при обработке архива {file.FileStorageBacket}=>{file.FileStoragePath}");
//                }
//            }
//        }

//        private async Task SaveSentinelL1CParseResultAsync(SentinelL1CParseResult parseResult)
//        {
//            try
//            {
//                using var transaction = await SentinelDbContext.Database.BeginTransactionAsync();
                
//                try
//                {
//                    if (parseResult.Metadata != null)
//                    {
//                        SentinelDbContext.SentinelL1CProductMetadata.Add(parseResult.Metadata);
//                    }
                    
//                    await SentinelDbContext.SaveChangesAsync();
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
//                Logger.LogError(ex, "Error saving Sentinel L1C parse result to database");
//                throw;
//            }
//        }
        
//        private async Task SaveSentinelInspireParseResultAsync(SentinelInspireParseResult parseResult)
//        {
//            try
//            {
//                using var transaction = await SentinelDbContext.Database.BeginTransactionAsync();
                
//                try
//                {
//                    if (parseResult.Metadata != null)
//                    {
//                        SentinelDbContext.SentinelInspireMetadata.Add(parseResult.Metadata);
//                    }
                    
//                    await SentinelDbContext.SaveChangesAsync();
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
//                Logger.LogError(ex, "Error saving Sentinel Inspire parse result to database");
//                throw;
//            }
//        }
        
//        private async Task SaveSentinelTileParseResultAsync(SentinelTileParseResult parseResult)
//        {
//            try
//            {
//                using var transaction = await SentinelDbContext.Database.BeginTransactionAsync();
                
//                try
//                {
//                    if (parseResult.Metadata != null)
//                    {
//                        SentinelDbContext.SentinelL1CTileMetadata.Add(parseResult.Metadata);
//                    }
                    
//                    await SentinelDbContext.SaveChangesAsync();
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
//                Logger.LogError(ex, "Error saving Sentinel Tile parse result to database");
//                throw;
//            }
//        }
        
//        private async Task SaveSentinelQualityParseResultAsync(SentinelQualityParseResult parseResult)
//        {
//            try
//            {
//                using var transaction = await SentinelDbContext.Database.BeginTransactionAsync();
                
//                try
//                {
//                    if (parseResult.Metadata != null)
//                    {
//                        SentinelDbContext.SentinelProductQualityMetadata.Add(parseResult.Metadata);
//                    }
                    
//                    await SentinelDbContext.SaveChangesAsync();
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
//                Logger.LogError(ex, "Error saving Sentinel Quality parse result to database");
//                throw;
//            }
//        }

//        private async Task ProcessProductMappingAsync(string productId)
//        {
//            try
//            {
//                // Получаем геометрию продукта из Inspire метаданных
//                var inspireMetadata = await SentinelDbContext.SentinelInspireMetadata
//                    .FirstOrDefaultAsync(sim => sim.FileIdentifier == productId);

//                if (inspireMetadata != null)
//                {
//                    var productGeometry = CreatePolygonFromBoundingBox(
//                        inspireMetadata.WestBoundLongitude,
//                        inspireMetadata.SouthBoundLatitude,
//                        inspireMetadata.EastBoundLongitude,
//                        inspireMetadata.NorthBoundLatitude
//                    );

//                    if (productGeometry != null)
//                    {
//                        await Mediator.Send(new ProcessNewProductCommand(
//                            productId,
//                            ProductSourceType.Sentinel2,
//                            productGeometry
//                        ));
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Logger.LogError(ex, "Error processing product mapping for {ProductId}", productId);
//            }
//        }

//        private Polygon? CreatePolygonFromBoundingBox(decimal west, decimal south, decimal east, decimal north)
//        {
//            try
//            {
//                var coordinates = new[]
//                {
//                    new Coordinate((double)west, (double)south),
//                    new Coordinate((double)east, (double)south),
//                    new Coordinate((double)east, (double)north),
//                    new Coordinate((double)west, (double)north),
//                    new Coordinate((double)west, (double)south) // замкнуть полигон
//                };

//                var shell = new LinearRing(coordinates);
//                return new Polygon(shell);
//            }
//            catch (Exception ex)
//            {
//                Logger.LogWarning(ex, "Error creating polygon from bounding box: {West}, {South}, {East}, {North}", west, south, east, north);
//                return null;
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
