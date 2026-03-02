using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minio;
using NodaTime;
using System.Linq;
using System.Net.Http.Headers;

namespace Masofa.BusinessLogic.Cli.Migration
{
    public class BidAndResultMigrationCommand : IRequest<BidAndResultMigrationResult>
    {

    }

    public class BidAndResultMigrationCommandHandler : IRequestHandler<BidAndResultMigrationCommand, BidAndResultMigrationResult>
    {
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.DepricatedUdictServerTwoDbContext DepricatedUdictServerTwoDbContext { get; set; }
        private Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.DepricatedUmapiServerTwoDbContext DepricatedUmapiServerTwoDbContext { get; set; }
        private Masofa.Depricated.DataAccess.DepricatedAuthServerTwo.DepricatedAuthServerTwoDbContext DepricatedAuthServerTwoDbContext { get; set; }
        private Masofa.Depricated.DataAccess.DepricatedAuthServerOne.DepricatedAuthServerOneDbContext DepricatedAuthServerOneDbContext { get; set; }
        private Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.DepricatedUmapiServerOneDbContext DepricatedUmapiServerOneDbContext { get; set; }
        private Masofa.Depricated.DataAccess.DepricatedUalertsServerOne.DepricatedUalertsServerOneDbContext DepricatedUalertsServerOneDbContext { get; set; }

        private IFileStorageProvider FileStorageProvider { get; set; }
        private ILogger Logger { get; set; }

        private List<Masofa.Common.Models.Dictionaries.Crop> _allCrops = new List<Masofa.Common.Models.Dictionaries.Crop>();
        private List<Masofa.Common.Models.Dictionaries.BidType> _allBidTypes = new List<Masofa.Common.Models.Dictionaries.BidType>();


        private List<Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidType> _oldTwoBidTypes = new List<Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidType>();
        private List<Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidState> _oldTwoBidStates = new List<Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidState>();


        public BidAndResultMigrationCommandHandler(MasofaCommonDbContext masofaCommonDbContext,
            MasofaCropMonitoringDbContext masofaCropMonitoringDbContext,
            MasofaDictionariesDbContext masofaDictionariesDbContext,
            MasofaIdentityDbContext masofaIdentityDbContext,
            Depricated.DataAccess.DepricatedUdictServerTwo.DepricatedUdictServerTwoDbContext depricatedUdictServerTwoDbContext,
            Depricated.DataAccess.DepricatedUmapiServerTwo.DepricatedUmapiServerTwoDbContext depricatedUmapiServerTwoDbContext,
            Depricated.DataAccess.DepricatedAuthServerTwo.DepricatedAuthServerTwoDbContext depricatedAuthServerTwoDbContext,
            Depricated.DataAccess.DepricatedAuthServerOne.DepricatedAuthServerOneDbContext depricatedAuthServerOneDbContext,
            Depricated.DataAccess.DepricatedUmapiServerOne.DepricatedUmapiServerOneDbContext depricatedUmapiServerOneDbContext,
            Depricated.DataAccess.DepricatedUalertsServerOne.DepricatedUalertsServerOneDbContext depricatedUalertsServerOneDbContext,
            IFileStorageProvider fileStorageProvider,
            ILogger<BidAndResultMigrationCommandHandler> logger)
        {
            MasofaCommonDbContext = masofaCommonDbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            MasofaIdentityDbContext = masofaIdentityDbContext;
            DepricatedUdictServerTwoDbContext = depricatedUdictServerTwoDbContext;
            DepricatedUmapiServerTwoDbContext = depricatedUmapiServerTwoDbContext;
            DepricatedAuthServerTwoDbContext = depricatedAuthServerTwoDbContext;
            DepricatedAuthServerOneDbContext = depricatedAuthServerOneDbContext;
            DepricatedUmapiServerOneDbContext = depricatedUmapiServerOneDbContext;
            DepricatedUalertsServerOneDbContext = depricatedUalertsServerOneDbContext;
            FileStorageProvider = fileStorageProvider;
            Logger = logger;
        }


        public async Task<BidAndResultMigrationResult> Handle(BidAndResultMigrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation("Starting bid migration from both servers...");
                
                var result = new BidAndResultMigrationResult();
                _allCrops = await MasofaDictionariesDbContext.Crops.ToListAsync();
                _oldTwoBidTypes = await DepricatedUdictServerTwoDbContext.BidTypes.ToListAsync();
                _allBidTypes = await MasofaDictionariesDbContext.BidTypes.ToListAsync();
                _oldTwoBidStates = await DepricatedUdictServerTwoDbContext.BidStates.ToListAsync();
                var serverTwoResult = await MigrateBidsFromServerTwoAsync();
                result.ServerTwoBidsMigrated = serverTwoResult;

                //var serverOneResult = await MigrateBidsFromServerOneAsync();
                //result.ServerOneBidsMigrated = serverOneResult;
                
                //result.TotalBidsMigrated = 0 + serverOneResult;
                
                //var fileMigrationResult = await MigrateBidFilesAsync();
                //result.FilesMigrated = fileMigrationResult;
                
                Logger.LogInformation($"Bid migration completed. Total migrated: {result.TotalBidsMigrated}, Files migrated: {result.FilesMigrated}");
                return result;
            }
            catch (Exception ex) 
            {
                Logger.LogCritical(ex, "Error during bid migration");
                return new BidAndResultMigrationResult()
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<int> MigrateBidsFromServerTwoAsync()
        {
            try
            {
                var oldBids = await DepricatedUmapiServerTwoDbContext.Bids.ToListAsync();
                var currentBids = await MasofaCropMonitoringDbContext.Bids
                    .Select(m => m.Number)
                    .ToListAsync();
                var newBids = new List<Masofa.Common.Models.CropMonitoring.Bid>();

                oldBids = oldBids.Where(m => (currentBids.Contains(m.Number) == false)).ToList();

                foreach (var oldBid in oldBids)
                {
                    var cropId = await MapCropAsync(oldBid.CropId);
                    var tempBid = new Masofa.Common.Models.CropMonitoring.Bid()
                    {
                        Number = oldBid.Number,
                        FieldPlantingDate = oldBid.FieldPlantingDate.HasValue ? ResolveDateTime(oldBid.FieldPlantingDate.Value) : null,
                        CreateAt = ResolveDateTime(oldBid.CreateDate),
                        LastUpdateAt = ResolveDateTime(oldBid.ModifyDate),
                        StartDate = oldBid.StartDate.HasValue ? ResolveDateTime(oldBid.StartDate.Value) : null,
                        DeadlineDate = oldBid.DeadlineDate.HasValue ? ResolveDateTime(oldBid.DeadlineDate.Value) : default,
                        EndDate = oldBid.EndDate.HasValue ? ResolveDateTime(oldBid.EndDate.Value) : null,
                        Comment = oldBid.Comment,
                        Description = oldBid.Description,
                        Lng = oldBid.Lng.HasValue ? oldBid.Lng.Value : default,
                        Lat = oldBid.Lat.HasValue ? oldBid.Lat.Value : default,
                        IsUnvalidBid = true,
                        BidTypeId = await MapBidTypeAsync(oldBid.BidTypeId),
                        BidState = await MapBidStateAsync(oldBid.BidStateId),
                        CropId = cropId,
                        RegionId = oldBid.RegionId,
                        VarietyId = oldBid.VarietyId,
                        ForemanId = await ResolveUserIdAsync(oldBid.ForemanId),
                        WorkerId = await ResolveUserIdAsync(oldBid.WorkerId),
                        ParentId = oldBid.ParentId,
                        CreateUser = await ResolveUserIdAsync(oldBid.CreateUser),
                        LastUpdateUser = await ResolveUserIdAsync(oldBid.ModifyUser),
                        BidTemplateId = await MapBidTemplateAsync(cropId)
                    };

                    newBids.Add(tempBid);
                }

                if (newBids.Any())
                {
                    await MasofaCropMonitoringDbContext.Bids.AddRangeAsync(newBids);
                    await MasofaCropMonitoringDbContext.SaveChangesAsync();
                    Logger.LogInformation($"Successfully migrated {newBids.Count} bids from Server Two");
                }

                return newBids.Count;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error migrating bids from Server Two");
                return 0;
            }
        }

        private async Task<int> MigrateBidsFromServerOneAsync()
        {
            try
            {
                var oldBids = await DepricatedUmapiServerOneDbContext.Bids.ToListAsync();
                var currentBids = await MasofaCropMonitoringDbContext.Bids.ToListAsync();
                var newBids = new List<Masofa.Common.Models.CropMonitoring.Bid>();

                foreach (var oldBid in oldBids)
                {
                    if (currentBids.Any(m => m.Number == oldBid.Number))
                    {
                        Logger.LogDebug($"Bid with number {oldBid.Number} already exists, skipping");
                        continue;
                    }

                    var cropId = await MapCropAsync(oldBid.CropId, isServerOne: true);
                    var tempBid = new Masofa.Common.Models.CropMonitoring.Bid()
                    {
                        Number = oldBid.Number,
                        FieldPlantingDate = oldBid.FieldPlantingDate.HasValue ? ResolveDateTime(oldBid.FieldPlantingDate.Value) : null,
                        CreateAt = ResolveDateTime(oldBid.CreateDate),
                        LastUpdateAt = ResolveDateTime(oldBid.ModifyDate),
                        StartDate = oldBid.StartDate.HasValue ? ResolveDateTime(oldBid.StartDate.Value) : null,
                        DeadlineDate = oldBid.DeadlineDate.HasValue ? ResolveDateTime(oldBid.DeadlineDate.Value) : default,
                        EndDate = oldBid.EndDate.HasValue ? ResolveDateTime(oldBid.EndDate.Value) : null,
                        Comment = oldBid.Comment,
                        Description = oldBid.Description,
                        Lng = oldBid.Lng.HasValue ? oldBid.Lng.Value : default,
                        Lat = oldBid.Lat.HasValue ? oldBid.Lat.Value : default,
                        IsUnvalidBid = true,
                        BidTypeId = await MapBidTypeAsync(oldBid.BidTypeId, isServerOne: true),
                        BidState = await MapBidStateAsync(oldBid.BidStateId),
                        CropId = cropId,
                        RegionId = oldBid.RegionId,
                        VarietyId = oldBid.VarietyId,
                        ForemanId = await ResolveUserIdAsync(oldBid.ForemanId),
                        WorkerId = await ResolveUserIdAsync(oldBid.WorkerId),
                        ParentId = oldBid.ParentId,
                        CreateUser = await ResolveUserIdAsync(oldBid.CreateUser),
                        LastUpdateUser = await ResolveUserIdAsync(oldBid.ModifyUser),
                        BidTemplateId = await MapBidTemplateAsync(cropId)
                    };

                    newBids.Add(tempBid);
                }

                if (newBids.Any())
                {
                    await MasofaCropMonitoringDbContext.Bids.AddRangeAsync(newBids);
                    await MasofaCropMonitoringDbContext.SaveChangesAsync();
                    Logger.LogInformation($"Successfully migrated {newBids.Count} bids from Server One");
                }

                return newBids.Count;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error migrating bids from Server One");
                return 0;
            }
        }

        private DateTime ResolveDateTime(LocalDateTime localDateTime)
        {
            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day)
                 .AddHours(localDateTime.Hour)
                 .AddMinutes(localDateTime.Minute)
                 .ToUniversalTime();
        }

        private DateTime ResolveDateTime(LocalDate localDateTime)
        {
            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day).ToUniversalTime();
        }

        #region Mapping Methods

        private async Task<Guid> MapBidTypeAsync(Guid oldBidTypeId, bool isServerOne = false)
        {
            try
            {
                var oldBidType = _oldTwoBidTypes
                    .FirstOrDefault(bt => bt.Id == oldBidTypeId);
                
                if (oldBidType == null) return Guid.Empty;

                // Load all bid types to memory and then filter by Names["en-US"] on the client side
                var newBidType = _allBidTypes.FirstOrDefault(bt => bt.Names["en-US"] == oldBidType.NameEn);
                
                return newBidType?.Id ?? Guid.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Error mapping BidType {oldBidTypeId}: {ex.Message}");
                return Guid.Empty;
            }
        }

        private async Task<BidStateType> MapBidStateAsync(Guid oldBidStateId)
        {
            try
            {
                var oldBidState = _oldTwoBidStates
                    .FirstOrDefault(bs => bs.Id == oldBidStateId);
                
                if (oldBidState == null) return BidStateType.New;

                return oldBidState.NameEn?.ToLower() switch
                {
                    "new" => BidStateType.New,
                    "active" => BidStateType.Active,
                    "in progress" => BidStateType.InProgress,
                    "completed" => BidStateType.Finished,
                    "rejected" => BidStateType.Rejected,
                    "cancelled" => BidStateType.Canceled,
                    _ => BidStateType.New
                };
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Error mapping BidState {oldBidStateId}: {ex.Message}");
                return BidStateType.New;
            }
        }

        private async Task<Guid> MapCropAsync(Guid? oldCropId, bool isServerOne = false)
        {
            try
            {
                if (!oldCropId.HasValue) return Guid.Empty;

                string oldCropName = string.Empty;

                if (isServerOne)
                {
                    var oldCrop = await DepricatedUalertsServerOneDbContext.Crops
                        .FirstOrDefaultAsync(c => c.Id == oldCropId.Value);
                    if (oldCrop == null) return Guid.Empty;
                    oldCropName = oldCrop.NameEn;
                }
                else
                {
                    var oldCrop = await DepricatedUdictServerTwoDbContext.Crops
                        .FirstOrDefaultAsync(c => c.Id == oldCropId.Value);
                    if (oldCrop == null) return Guid.Empty;
                    oldCropName = oldCrop.NameEn;
                }

                // Load all crops to memory and then filter by Names["en-US"] on the client side
                var newCrop = _allCrops.FirstOrDefault(c => c.Names["en-US"] == oldCropName);

                return newCrop?.Id ?? Guid.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Error mapping crop {oldCropId}: {ex.Message}");
                return Guid.Empty;
            }
        }

        private async Task<Guid> ResolveUserIdAsync(Guid? oldUserId)
        {
            try
            {
                if (!oldUserId.HasValue) return Guid.Empty;

                var oldName = DepricatedAuthServerOneDbContext.AspNetUsers.Any(m => m.Id.Equals(oldUserId))
                    ? DepricatedAuthServerOneDbContext.AspNetUsers.FirstOrDefault(m => m.Id.Equals(oldUserId))?.UserName ?? string.Empty
                    : DepricatedAuthServerTwoDbContext.AspNetUsers.FirstOrDefault(m => m.Id.Equals(oldUserId))?.UserName ?? string.Empty;

                if (oldName == null) return Guid.Empty;

                var newUser = await MasofaIdentityDbContext.Users
                    .FirstOrDefaultAsync(u => u.UserName == oldName);

                return newUser?.Id ?? Guid.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Error resolving user {oldUserId}: {ex.Message}");
                return Guid.Empty;
            }
        }

        private async Task<Guid> MapBidTemplateAsync(Guid cropId)
        {
            try
            {
                if (cropId == Guid.Empty) return Guid.Empty;

                var bidTemplate = await MasofaCropMonitoringDbContext.BidTemplates
                    .FirstOrDefaultAsync(bt => bt.CropId == cropId);

                return bidTemplate?.Id ?? Guid.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Error mapping bid template for crop {cropId}: {ex.Message}");
                return Guid.Empty;
            }
        }

        private async Task<int> MigrateBidFilesAsync()
        {
            try
            {
                var bidResultDownloader = new BidResultDownloaderWithBasicAuth("masofaapi", "strongAPIpassw0rd");
                var bidsWithFiles = new List<(int Number, Depricated.DataAccess.DepricatedUmapiServerTwo.Models.Bid OldBid, Bid NewBid)>();
                
                var bids = await DepricatedUmapiServerTwoDbContext.Bids.ToListAsync();
                var bidFiles = await DepricatedUmapiServerTwoDbContext.BidFiles.ToListAsync();
                var newBids = await MasofaCropMonitoringDbContext.Bids.ToListAsync();

                foreach (var bidFile in bidFiles)
                {
                    var oldBid = bids.FirstOrDefault(m => m.Id.Equals(bidFile.BidId));
                    if (oldBid == null) continue;
                    
                    var newBid = newBids.FirstOrDefault(m => m.Number.Equals(oldBid.Number));
                    if (newBid == null) continue;

                    if (!oldBid.CropId.HasValue) continue;

                    bidsWithFiles.Add(new()
                    {
                        Number = (int)oldBid.Number,
                        OldBid = oldBid,
                        NewBid = newBid,
                    });
                }

                var filesMigrated = 0;
                foreach (var item in bidsWithFiles)
                {
                    try
                    {
                        using var stream = await bidResultDownloader.DownloadBidResultAsync(item.OldBid.CropId.Value, item.OldBid.Id);
                        if (stream == null) continue;

                        var fileName = $"result_{item.NewBid.Id}";
                        fileName = await FileStorageProvider.PushFileAsync(stream, fileName, "bids");

                        var fileItem = new FileStorageItem()
                        {
                            FileContentType = FileContentType.ArchiveZIP,
                            FileStoragePath = fileName,
                            FileStorageBacket = "bids",
                            OwnerTypeFullName = typeof(Bid).FullName,
                            OwnerId = item.NewBid.Id,
                            Status = StatusType.Active,
                            CreateAt = item.NewBid.LastUpdateAt,
                            CreateUser = item.NewBid.LastUpdateUser,
                            LastUpdateAt = item.NewBid.LastUpdateAt,
                            LastUpdateUser = item.NewBid.LastUpdateUser,
                        };

                        MasofaCommonDbContext.FileStorageItems.Add(fileItem);
                        await MasofaCommonDbContext.SaveChangesAsync();
                        
                        item.NewBid.FileResultId = fileItem.Id;
                        await MasofaCropMonitoringDbContext.SaveChangesAsync();
                        
                        filesMigrated++;
                        Logger.LogDebug($"Migrated file for bid {item.Number}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning($"Error migrating file for bid {item.Number}: {ex.Message}");
                    }
                }

                Logger.LogInformation($"Successfully migrated {filesMigrated} bid result files");
                return filesMigrated;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error migrating bid files");
                return 0;
            }
        }

        #endregion
    }

    public class BidResultDownloaderWithBasicAuth
    {
        private HttpClient HttpClient { get; set; }
        private const string BASE_URL = "https://masofa-yer.agro.uz";

        public BidResultDownloaderWithBasicAuth(string username, string password)
        {
            HttpClient = new HttpClient();
            var credentials = $"{username}:{password}";
            var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
        }

        public async Task<Stream?> DownloadBidResultAsync(Guid cropId, Guid bidId)
        {
            var url = $"{BASE_URL}/api/v1/dev/crops/bids/{cropId}/{bidId}/result";

            try
            {
                var response = await HttpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStream();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class BidAndResultMigrationResult
    {
        public int ServerOneBidsMigrated { get; set; }
        public int ServerTwoBidsMigrated { get; set; }
        public int TotalBidsMigrated { get; set; }
        public int FilesMigrated { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    }
}
