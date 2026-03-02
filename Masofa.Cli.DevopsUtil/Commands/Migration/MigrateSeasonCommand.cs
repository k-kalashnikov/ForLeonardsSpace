//using Masofa.Common.Models.CropMonitoring;
//using Masofa.Common.Models.Identity;
//using Masofa.Common.Models.System;
//using Masofa.DataAccess;
//using Masofa.Depricated.DataAccess.DepricatedAuthServerOne;
//using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUmapiServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using NetTopologySuite.Geometries;
//using NetTopologySuite.IO;
//using Newtonsoft.Json;
//using NodaTime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Migration
//{
//    [BaseCommand("Migrate Seasons", "Миграция данных о сезонах из старой системы в новую")]
//    public class MigrateSeasonCommand : IBaseCommand
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
//        private DepricatedUfieldsServerOneDbContext DepricatedUfieldsServerOneDbContext { get; set; }


//        public MigrateSeasonCommand(MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext, MasofaCommonDbContext masofaCommonDbContext,
//            MasofaDictionariesDbContext masofaDictionariesDbContext, UserManager<User> userManager, RoleManager<Role> roleManager,
//            DepricatedAuthServerTwoDbContext depricatedAuthServerTwoDbContext,
//            DepricatedUmapiServerTwoDbContext depricatedUmapiServerTwoDbContext,
//            DepricatedUmapiServerOneDbContext depricatedUmapiServerOneDbContext,
//            DepricatedAuthServerOneDbContext depricatedAuthServerOneDbContext,
//            DepricatedUalertsServerOneDbContext depricatedUalertsServerOneContext,
//            DepricatedUdictServerTwoDbContext depricatedUdictServerTwoDbContext,
//            DepricatedUfieldsServerOneDbContext depricatedUfieldsServerOneDbContext)
//        {
//            MasofaCropMonitoringDbContext = MasofaCropMonitoringDbContext;
//            MasofaCommonDbContext = masofaCommonDbContext;
//            MasofaDictionariesDbContext = masofaDictionariesDbContext;
//            UserManager = userManager;
//            RoleManager = roleManager;
//            DepricatedUmapiServerOneDbContext = depricatedUmapiServerOneDbContext;
//            DepricatedAuthServerOneDbContext = depricatedAuthServerOneDbContext;
//            DepricatedUalertsServerOneDbContext = depricatedUalertsServerOneContext;
//            DepricatedUfieldsServerOneDbContext = depricatedUfieldsServerOneDbContext;

//            DepricatedAuthServerTwoDbContext = depricatedAuthServerTwoDbContext;
//            DepricatedUmapiServerTwoDbContext = depricatedUmapiServerTwoDbContext;
//            DepricatedUdictServerTwoDbContext = depricatedUdictServerTwoDbContext;
//        }

//        public async Task Execute()
//        {
//            //Console.WriteLine("Enter pls aplly_all_migration.sql path");
//            //var sqlPath = Console.ReadLine();
//            //await ExecuteSqlScriptFromFile(sqlPath);
//            await MigrateSeasonsAsync();
//        }

//        public void Dispose()
//        {
//        }

//        #region MigrateSeason

//        private async Task MigrateSeasonsAsync()
//        {
//            var oldSeasons = DepricatedUfieldsServerOneDbContext.Seasons.ToList();

//            foreach (var season in oldSeasons)
//            {
//                var createdId = ResolveUserId(season.CreateUser);

//                var updatedId = ResolveUserId(season.ModifyUser);

//                var tempCropName = DepricatedUalertsServerOneDbContext.Crops.FirstOrDefault(x => x.Id.Equals(season.CropId))?.NameEn?.ToLower() ?? string.Empty;
//                var dictContain = MasofaDictionariesDbContext.Crops.Any(m => m.Id.Equals(season.CropId));
//                var cropId = (!season.CropId.HasValue) ? Guid.Empty
//                    : (dictContain
//                    ? season.CropId
//                    : (string.IsNullOrEmpty(tempCropName) ? MasofaDictionariesDbContext.Crops.First(m => m.NameEn.ToLower().Equals(tempCropName)).Id : Guid.Empty));

//                var varietyId = MasofaDictionariesDbContext.Varieties.Any(m => m.Id.Equals(season.VarietyId))
//                    ? season.VarietyId
//                    : Guid.Empty;

//                var fieldId = MasofaCropMonitoringDbContext.Fields.Any(m => m.Id.Equals(season.FieldId))
//                    ? season.FieldId
//                    : Guid.Empty;

//                var newSeason = new Common.Models.CropMonitoring.Season
//                {
//                    CreateAt = ResolveDateTime(season.CreateDate ?? default),
//                    LastUpdateAt = ResolveDateTime(season.ModifyDate ?? default),
//                    CreateUser = createdId,
//                    LastUpdateUser = updatedId,
//                    FieldArea = season.FieldArea,
//                    FieldId = fieldId,
//                    CropId = cropId,
//                    StartDate = ResolveDateOnly(season.StartDate),
//                    EndDate = ResolveDateOnly(season.EndDate),
//                    HarvestingDatePlan = ResolveDateOnly(season.HarvestingDatePlan),
//                    HarvestingDate = ResolveDateOnly(season.HarvestingDate),
//                    PlantingDatePlan = ResolveDateOnly(season.PlantingDatePlan),
//                    PlantingDate = ResolveDateOnly(season.PlantingDate),
//                    Latitude = season.Latitude,
//                    Longitude = season.Longitude,
//                    Title = season.Title,
//                    VarietyId = varietyId,
//                    Yield = season.Yield,
//                    YieldHa = season.YieldHa,
//                };

//                try
//                {
//                    MasofaCropMonitoringDbContext.Add(newSeason);
//                    MasofaCropMonitoringDbContext.SaveChanges();
//                    Console.WriteLine($"SUCCESS: Added new season with seasonId:{season.Id}");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"ERROR: {ex.Message}");
//                    Console.WriteLine($"ERROR: {ex?.InnerException?.ToString()}");
//                    continue;
//                }
//            }
//        }


//        private Guid ResolveUserId(string? userName)
//        {
//            if (string.IsNullOrEmpty(userName))
//            {
//                return Guid.Empty;
//            }
//            return UserManager.Users.FirstOrDefault(m => m.UserName.Equals(userName))?.Id ?? Guid.Empty;
//        }

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

//        private DateTime ResolveDateTime(LocalDateTime localDateTime)
//        {
//            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day)
//                 .AddHours(localDateTime.Hour)
//                 .AddMinutes(localDateTime.Minute)
//                 .ToUniversalTime();
//        }

//        private static DateOnly? ResolveDateOnly(LocalDate? date)
//        {
//            return date.HasValue
//                ? new DateOnly(date.Value.Year, date.Value.Month, date.Value.Day)
//                : null;
//        }

//        public static Geometry? ResolveGeometry(Geometry? original, int srid = 4326)
//        {
//            if (original == null) return null;

//            var writer = new WKBWriter();
//            var reader = new WKBReader();

//            var bytes = writer.Write(original);
//            var clone = reader.Read(bytes);
//            clone.SRID = srid;

//            return clone;
//        }
//        #endregion

//        private async Task<bool> ExecuteSqlScriptFromFile(string filePath)
//        {
//            if (!File.Exists(filePath))
//            {
//                Console.WriteLine($"SQL файл не найден: {filePath}");
//                return false;
//            }

//            try
//            {
//                var sql = await File.ReadAllTextAsync(filePath);
//                await MasofaCropMonitoringDbContext.Database.ExecuteSqlRawAsync(sql);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Ошибка выполнения SQL скрипта: {ex.Message}");
//                return false;
//            }
//        }

//        public Task Execute(string[] args)
//        {
//            return Execute();
//        }
//    }
//}
