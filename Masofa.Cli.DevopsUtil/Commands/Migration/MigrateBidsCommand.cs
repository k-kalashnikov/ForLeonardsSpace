//using Masofa.Common.Models.CropMonitoring;
//using Masofa.Common.Models.Identity;
//using Masofa.Common.Models.System;
//using Masofa.DataAccess;
//using Masofa.Depricated.DataAccess.DepricatedAuthServerOne;
//using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedUmapiServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
//using Microsoft.AspNetCore.Identity;
//using NodaTime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Migration
//{
//    [BaseCommand("Migrate Bids", "Миграция данных о заявках (bids) из старой системы в новую")]
//    public class MigrateBidsCommand : IBaseCommand
//    {
//        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
//        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
//        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
//        private UserManager<Masofa.Common.Models.Identity.User> UserManager { get; set; }
//        private DepricatedUmapiServerOneDbContext DepricatedUmapiServerOneDbContext { get; set; }
//        private DepricatedAuthServerOneDbContext DepricatedAuthServerOneDbContext { get; set; }
//        private DepricatedUmapiServerTwoDbContext DepricatedUmapiServerTwoDbContext { get; set; }
//        private DepricatedAuthServerTwoDbContext DepricatedAuthServerTwoDbContext { get; set; }
//        private DepricatedUdictServerTwoDbContext DepricatedUdictServerTwoDbContext { get; set; }
//        private RoleManager<Masofa.Common.Models.Identity.Role> RoleManager { get; set; }
//        private DepricatedUalertsServerOneDbContext DepricatedUalertsServerOneDbContext { get; set; }


//        public MigrateBidsCommand(MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext, MasofaCommonDbContext masofaCommonDbContext,
//            MasofaDictionariesDbContext masofaDictionariesDbContext, UserManager<User> userManager, RoleManager<Role> roleManager,
//            DepricatedAuthServerTwoDbContext depricatedAuthServerTwoDbContext,
//            DepricatedUmapiServerTwoDbContext depricatedUmapiServerTwoDbContext,
//            DepricatedUmapiServerOneDbContext depricatedUmapiServerOneDbContext,
//            DepricatedAuthServerOneDbContext depricatedAuthServerOneDbContext,
//            DepricatedUalertsServerOneDbContext depricatedUalertsServerOneContext,
//            DepricatedUdictServerTwoDbContext depricatedUdictServerTwoDbContext)
//        {
//            MasofaCropMonitoringDbContext = MasofaCropMonitoringDbContext;
//            MasofaCommonDbContext = masofaCommonDbContext;
//            MasofaDictionariesDbContext = masofaDictionariesDbContext;
//            UserManager = userManager;
//            RoleManager = roleManager;
//            DepricatedUmapiServerOneDbContext = depricatedUmapiServerOneDbContext;
//            DepricatedAuthServerOneDbContext = depricatedAuthServerOneDbContext;
//            DepricatedUalertsServerOneDbContext = depricatedUalertsServerOneContext;

//            DepricatedAuthServerTwoDbContext = depricatedAuthServerTwoDbContext;
//            DepricatedUmapiServerTwoDbContext = depricatedUmapiServerTwoDbContext;
//            DepricatedUdictServerTwoDbContext = depricatedUdictServerTwoDbContext;
//        }

//        public async Task Execute()
//        {
//            await MigrateBidsAsync();
//        }

//        public async Task Execute(string[] args)
//        {
//            await MigrateBidsAsync();
//        }

//        public void Dispose()
//        {

//        }

//        #region MigrateBids
//        //TODO: сначала сделать миграцию словарей и только потом Заявки
//        private async Task MigrateBidsAsync()
//        {
//            await MigrateBidsFromOneAsync();
//            await MigrateBidsFromTwoAsync();
//        }

//        private async Task MigrateBidsFromOneAsync()
//        {
//            var oldBids = DepricatedUmapiServerOneDbContext.Bids
//                .Where(m => m.CreateDate >= LocalDateTime.FromDateTime(new DateTime(2025, 5, 1)))
//                .ToList();

//            if (oldBids.Count == 0)
//            {
//                Console.WriteLine("Error: old Bids count: 0");
//                return;
//            }

//            foreach (var oldBid in oldBids)
//            {

//                // Нужно притащить файл в олдбид есть файлрезулт и нужно перетащить его в минио. 
//                // Добавляю запись в файлстореджайтем и его айди записываю в поле бида по результату.

//                if (MasofaCropMonitoringDbContext.Bids.Any(m => m.Number.Equals(oldBid.Number)))
//                {
//                    Console.WriteLine($"WARNING: Bid with number:{oldBid.Number} is exist");
//                    continue;
//                }

//                var bidState = ResolveBidStateId(oldBid.BidStateId);

//                var oldParentBid = DepricatedUmapiServerOneDbContext.Bids.FirstOrDefault(m => m.Id.Equals(oldBid.ParentId));
//                var newParentId = (oldParentBid == null) ? Guid.Empty : (MasofaCropMonitoringDbContext.Bids.FirstOrDefault(m => m.Number.Equals(oldParentBid.Number))?.Id ?? Guid.Empty);

//                var tempCropName = DepricatedUalertsServerOneDbContext.Crops.FirstOrDefault(x => x.Id.Equals(oldBid.CropId))?.NameEn?.ToLower() ?? string.Empty;
//                var dictContain = MasofaDictionariesDbContext.Crops.Any(m => m.Id.Equals(oldBid.CropId));
//                var currentCropId = (!oldBid.CropId.HasValue) ? Guid.Empty
//                    : (dictContain
//                    ? oldBid.CropId
//                    : (string.IsNullOrEmpty(tempCropName) ? MasofaDictionariesDbContext.Crops.First(m => m.NameEn.ToLower().Equals(tempCropName)).Id : Guid.Empty));

//                var bidTemplateId = MasofaCropMonitoringDbContext.BidTemplates.FirstOrDefault(m => m.CropId.Equals(currentCropId))?.Id ?? Guid.Empty;

//                var createdId = ResolveUserId(oldBid.CreateUser);

//                var updatedId = ResolveUserId(oldBid.ModifyUser);

//                var foremanId = ResolveUserId(oldBid.ForemanId);

//                var workerId = ResolveUserId(oldBid.WorkerId);

//                // Разделяем запросы для избежания множественных контекстов
//                var currentBidTypeId = Guid.Empty;
//                if (oldBid.BidTypeId != Guid.Empty)
//                {
//                    if (MasofaDictionariesDbContext.BidTypes.Any(m => m.Id.Equals(oldBid.BidTypeId)))
//                    {
//                        currentBidTypeId = oldBid.BidTypeId;
//                    }
//                    else
//                    {
//                        var oldBidType = DepricatedUmapiServerOneDbContext.BidTypes.FirstOrDefault(m => m.Id.Equals(oldBid.BidTypeId));
//                        if (oldBidType != null)
//                        {
//                            var matchingBidType = MasofaDictionariesDbContext.BidTypes.FirstOrDefault(m => m.NameEn.ToLower().Equals(oldBidType.NameEn.ToLower()));
//                            currentBidTypeId = matchingBidType?.Id ?? Guid.Empty;
//                        }
//                    }
//                }

//                var currentRegionId = MasofaDictionariesDbContext.Regions.Any(m => m.Id.Equals(oldBid.RegionId))
//                    ? oldBid.RegionId
//                    : Guid.Empty;
//                var currentVarietyId = MasofaDictionariesDbContext.Varieties.Any(m => m.Id.Equals(oldBid.VarietyId))
//                    ? oldBid.VarietyId
//                    : Guid.Empty;

//                var currentFieldId = Guid.Empty; //TODO - отдельной командой подсвечивать//заполнять заявки у которых нет полей

//                var newBid = new Bid
//                {
//                    ParentId = newParentId,
//                    CreateUser = createdId,
//                    CreateAt = ResolveDateTime(oldBid.CreateDate),
//                    LastUpdateUser = updatedId,
//                    LastUpdateAt = ResolveDateTime(oldBid.ModifyDate),
//                    BidTypeId = currentBidTypeId,
//                    BidState = bidState,
//                    ForemanId = foremanId,
//                    WorkerId = workerId,
//                    StartDate = oldBid.StartDate.HasValue ? ResolveDateTime(oldBid.StartDate.Value) : null,
//                    DeadlineDate = oldBid.DeadlineDate.HasValue ? ResolveDateTime(oldBid.DeadlineDate.Value) : default,
//                    EndDate = oldBid.EndDate.HasValue ? ResolveDateTime(oldBid.EndDate.Value) : null,
//                    FieldId = currentFieldId,
//                    RegionId = currentRegionId,
//                    CropId = currentCropId ?? default,
//                    VarietyId = currentVarietyId,
//                    Comment = oldBid.Comment,
//                    Description = oldBid.Description,
//                    Lat = oldBid.Lat ?? default,
//                    Lng = oldBid.Lng ?? default,
//                    Number = oldBid.Number,
//                    FieldPlantingDate = oldBid.FieldPlantingDate.HasValue ? ResolveDateTime(oldBid.FieldPlantingDate.Value) : null,
//                    BidTemplateId = bidTemplateId,
//                    FileResultId = Guid.Empty
//                };

//                try
//                {
//                    MasofaCropMonitoringDbContext.Set<Bid>().Add(newBid);
//                    MasofaCropMonitoringDbContext.SaveChanges();
//                    Console.WriteLine($"SUCCESS: Add new bid with number:{newBid.Number}");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"ERROR: {ex?.Message}");
//                    Console.WriteLine($"ERROR: {ex?.InnerException?.Message}");
//                    continue;
//                }
//            }
//        }

//        private async Task MigrateBidsFromTwoAsync()
//        {
//            var oldBids = DepricatedUmapiServerTwoDbContext.Bids
//                .Where(m => m.CreateDate >= LocalDateTime.FromDateTime(new DateTime(2025, 5, 1)))
//                .ToList();

//            if (oldBids.Count == 0)
//            {
//                Console.WriteLine("Error: old Bids count: 0");
//                return;
//            }

//            foreach (var oldBid in oldBids)
//            {

//                // Нужно притащить файл в олдбид есть файлрезулт и нужно перетащить его в минио. 
//                // Добавляю запись в файлстореджайтем и его айди записываю в поле бида по результату.

//                if (MasofaCropMonitoringDbContext.Bids.Any(m => m.Number.Equals(oldBid.Number)))
//                {
//                    Console.WriteLine($"WARNING: Bid with number:{oldBid.Number} is exist");
//                    continue;
//                }

//                var bidState = ResolveBidStateId(oldBid.BidStateId);

//                var oldParentBid = DepricatedUmapiServerTwoDbContext.Bids.FirstOrDefault(m => m.Id.Equals(oldBid.ParentId));
//                var newParentId = (oldParentBid == null) ? Guid.Empty : (MasofaCropMonitoringDbContext.Bids.FirstOrDefault(m => m.Number.Equals(oldParentBid.Number))?.Id ?? Guid.Empty);

//                var dictContain = MasofaDictionariesDbContext.Crops.Any(m => m.Id.Equals(oldBid.CropId));
//                var currentCropId = ((oldBid.CropId.HasValue) && dictContain) ? oldBid.CropId : Guid.Empty;

//                var bidTemplateId = MasofaCropMonitoringDbContext.BidTemplates.FirstOrDefault(m => m.CropId.Equals(currentCropId))?.Id ?? Guid.Empty;

//                var createdId = ResolveUserId(oldBid.CreateUser);

//                var updatedId = ResolveUserId(oldBid.ModifyUser);

//                var foremanId = ResolveUserId(oldBid.ForemanId);

//                var workerId = ResolveUserId(oldBid.WorkerId);

//                // Разделяем запросы для избежания множественных контекстов
//                var currentBidTypeId = Guid.Empty;
//                if (oldBid.BidTypeId != Guid.Empty)
//                {
//                    if (MasofaDictionariesDbContext.BidTypes.Any(m => m.Id.Equals(oldBid.BidTypeId)))
//                    {
//                        currentBidTypeId = oldBid.BidTypeId;
//                    }
//                    else
//                    {
//                        var oldBidType = DepricatedUmapiServerTwoDbContext.BidTypes.FirstOrDefault(m => m.Id.Equals(oldBid.BidTypeId));
//                        if (oldBidType != null)
//                        {
//                            var matchingBidType = MasofaDictionariesDbContext.BidTypes.FirstOrDefault(m => m.NameEn.ToLower().Equals(oldBidType.NameEn.ToLower()));
//                            currentBidTypeId = matchingBidType?.Id ?? Guid.Empty;
//                        }
//                    }
//                }

//                var currentRegionId = MasofaDictionariesDbContext.Regions.Any(m => m.Id.Equals(oldBid.RegionId))
//                    ? oldBid.RegionId
//                    : Guid.Empty;
//                var currentVarietyId = MasofaDictionariesDbContext.Varieties.Any(m => m.Id.Equals(oldBid.VarietyId))
//                    ? oldBid.VarietyId
//                    : Guid.Empty;
//                var currentFieldId = Guid.Empty; //TODO - отдельной командой подсвечивать//заполнять заявки у которых нет полей

//                var newBid = new Bid
//                {
//                    ParentId = newParentId,
//                    CreateUser = createdId,
//                    CreateAt = ResolveDateTime(oldBid.CreateDate),
//                    LastUpdateUser = updatedId,
//                    LastUpdateAt = ResolveDateTime(oldBid.ModifyDate),
//                    BidTypeId = currentBidTypeId,
//                    BidState = bidState,
//                    ForemanId = foremanId,
//                    WorkerId = workerId,
//                    StartDate = oldBid.StartDate.HasValue ? ResolveDateTime(oldBid.StartDate.Value) : null,
//                    DeadlineDate = oldBid.DeadlineDate.HasValue ? ResolveDateTime(oldBid.DeadlineDate.Value) : default,
//                    EndDate = oldBid.EndDate.HasValue ? ResolveDateTime(oldBid.EndDate.Value) : null,
//                    FieldId = currentFieldId,
//                    RegionId = currentRegionId,
//                    CropId = currentCropId ?? default,
//                    VarietyId = currentVarietyId,
//                    Comment = oldBid.Comment,
//                    Description = oldBid.Description,
//                    Lat = oldBid.Lat ?? default,
//                    Lng = oldBid.Lng ?? default,
//                    Number = oldBid.Number,
//                    FieldPlantingDate = oldBid.FieldPlantingDate.HasValue ? ResolveDateTime(oldBid.FieldPlantingDate.Value) : null,
//                    BidTemplateId = bidTemplateId,
//                    FileResultId = Guid.Empty
//                };

//                try
//                {
//                    MasofaCropMonitoringDbContext.Set<Bid>().Add(newBid);
//                    MasofaCropMonitoringDbContext.SaveChanges();
//                    Console.WriteLine($"SUCCESS: Add new bid with number:{newBid.Number}");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"ERROR: {ex?.Message}");
//                    Console.WriteLine($"ERROR: {ex?.InnerException?.Message}");
//                    continue;
//                }
//            }
//        }
//        #endregion

//        private Guid ResolveUserId(Guid? userId)
//        {
//            if (!userId.HasValue)
//            {
//                return Guid.Empty;
//            }
//            var oldName = DepricatedAuthServerOneDbContext.AspNetUsers.Any(m => m.Id.Equals(userId))
//                ? DepricatedAuthServerOneDbContext.AspNetUsers.FirstOrDefault(m => m.Id.Equals(userId))?.UserName ?? string.Empty
//                : DepricatedAuthServerTwoDbContext.AspNetUsers.FirstOrDefault(m => m.Id.Equals(userId))?.UserName ?? string.Empty;

//            return UserManager.Users.FirstOrDefault(m => m.UserName.Equals(oldName))?.Id ?? Guid.Empty;
//        }

//        private Guid ResolveUserId(string? userName)
//        {
//            if (string.IsNullOrEmpty(userName))
//            {
//                return Guid.Empty;
//            }
//            return UserManager.Users.FirstOrDefault(m => m.UserName.Equals(userName))?.Id ?? Guid.Empty;
//        }

//        private BidStateType ResolveBidStateId(Guid bitStateId)
//        {
//            var resultName = DepricatedUmapiServerOneDbContext.BidStates.FirstOrDefault(m => m.Id == bitStateId)?.NameEn
//              ?? DepricatedUmapiServerTwoDbContext.BidStates.FirstOrDefault(m => m.Id == bitStateId)?.NameEn;

//            return resultName?.ToLower() switch
//            {
//                "active" => BidStateType.Active,
//                "in progress" => BidStateType.InProgress,
//                "finished" => BidStateType.Finished,
//                "rejected" => BidStateType.Rejected,
//                "canceled" => BidStateType.Canceled,
//                _ => BidStateType.New
//            };
//        }



//        private DateTime ResolveDateTime(LocalDateTime localDateTime)
//        {
//            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day)
//                 .AddHours(localDateTime.Hour)
//                 .AddMinutes(localDateTime.Minute)
//                 .ToUniversalTime();
//        }

//        private DateTime ResolveDateTime(LocalDate localDateTime)
//        {
//            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day).ToUniversalTime();
//        }



//        //private async Task<bool> ExecuteSqlScriptFromFile(string filePath)
//        //{
//        //    if (!File.Exists(filePath))
//        //    {
//        //        Console.WriteLine($"SQL файл не найден: {filePath}");
//        //        return false;
//        //    }

//        //    try
//        //    {
//        //        var sql = await File.ReadAllTextAsync(filePath);
//        //        await MasofaIdentityDbContext.Database.ExecuteSqlRawAsync(sql);
//        //        return true;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"Ошибка выполнения SQL скрипта: {ex.Message}");
//        //        return false;
//        //    }
//        //}
//    }
//}
