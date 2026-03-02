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
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Migration
//{
//    [BaseCommand("Migrate Templates", "Миграция шаблонов из старой системы в новую")]
//    public class MigrateTemplatesCommand : IBaseCommand
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


//        public MigrateTemplatesCommand(MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext, MasofaCommonDbContext masofaCommonDbContext,
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

//        public async Task Execute(string[] args)
//        {
//            await MigrateTemplatesAsync();
//        }
//        public async Task Execute()
//        {
//            //Console.WriteLine("Enter pls aplly_all_migration.sql path");
//            //var sqlPath = Console.ReadLine();
//            //await ExecuteSqlScriptFromFile(sqlPath);
//            await MigrateTemplatesAsync();
//        }

//        public void Dispose()
//        {
//        }

//        #region MigrateTemplate

//        private async Task MigrateTemplatesAsync()
//        {
//            await MigrateTemplatesFromTwo();
//            //await MigrateTemplatesFromFiles(); - а нужно ли?
//        }

//        private async Task MigrateTemplatesFromTwo()
//        {
//            var oldTemplates = DepricatedUmapiServerTwoDbContext.Templates.ToList();
//            foreach (var item in oldTemplates)
//            {
//                var createdId = ResolveUserId(item.CreateUser);

//                var updatedId = ResolveUserId(item.UpdateUser);

//                var oldCrop = DepricatedUdictServerTwoDbContext.Crops.FirstOrDefault(m => m.Id.Equals(item.CropId));
//                var cropId = MasofaDictionariesDbContext.Crops.FirstOrDefault(m => m.NameRu.Equals(oldCrop.NameRu))?.Id ?? Guid.Empty;
//                if (MasofaCropMonitoringDbContext.BidTemplates.Any(bt => bt.ContentVersion.Equals(item.ContentVersion) && bt.SchemaVersion.Equals(item.SchemaVersion) && bt.CropId.Equals(cropId)))
//                {
//                    continue;
//                }
//                var newBidT = new BidTemplate()
//                {
//                    Comment = item.Comment,
//                    ContentVersion = item.ContentVersion,
//                    DataJson = item.Data,
//                    CropId = cropId,
//                    SchemaVersion = item.SchemaVersion,
//                    Status = item.IsActive ? Common.Models.StatusType.Active : Common.Models.StatusType.Hiden,
//                    CreateAt = item.CreateDate.ToDateTimeUtc(),
//                    LastUpdateAt = item.UpdateDate.ToDateTimeUtc(),
//                    CreateUser = createdId,
//                    LastUpdateUser = updatedId,
//                };
//                MasofaCropMonitoringDbContext.BidTemplates.Add(newBidT);

//                Console.WriteLine($"SUCCESS: Added new template by cropId:{item.CropId}; version: {item.ContentVersion}");
//            }

//            MasofaCropMonitoringDbContext.SaveChanges();
//        }

//        private async Task MigrateTemplatesFromFiles()
//        {
//            var templateList = Directory.GetFiles(Path.Combine("Resources", "CropTemplates"))?.ToList() ?? new List<string>();
//            if (!templateList.Any())
//            {
//                Console.WriteLine($"ERROR: TEMPLATES NOT FOUND IN {Path.Combine("Resources", "CropTemplates")}");
//            }

//            foreach (var item in templateList)
//            {
//                var templateJson = File.ReadAllText(item);
//                var templateObj = new BidTemplateSchema();
//                try
//                {
//                    templateObj = JsonConvert.DeserializeObject<BidTemplateSchema>(templateJson);
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine($"ERROR: TEMPLATES CANT BE DESERIALIZED FROM {item}");
//                    continue;
//                }
//                MasofaCropMonitoringDbContext.BidTemplates.Add(new BidTemplate()
//                {
//                    ContentVersion = templateObj.ContentVersion,
//                    SchemaVersion = templateObj.SchemaVersion,
//                });
//            }
//        }
//        #endregion

//        private Guid ResolveUserId(string? userName)
//        {
//            if (string.IsNullOrEmpty(userName))
//            {
//                return Guid.Empty;
//            }
//            return UserManager.Users.FirstOrDefault(m => m.UserName.Equals(userName))?.Id ?? Guid.Empty;
//        }

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
//    }
//}
