//using Masofa.Common.Models.System;
//using Masofa.Cli.DevopsUtil.Converters;
//using Masofa.Common.Models.Identity;
//using Masofa.DataAccess;
//using Masofa.Depricated.DataAccess.DepricatedAuthServerOne;
//using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedUmapiServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Migration
//{
//    [BaseCommand("Migrate Dictionaries", "Миграция справочных данных из старой системы в новую")]
//    public class MigrateDictionariesCommand : IBaseCommand
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


//        public MigrateDictionariesCommand(MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext, MasofaCommonDbContext masofaCommonDbContext,
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
//            await MigrateDictionariesAsync();
//        }

//        public void Dispose()
//        {

//        }

//        #region MigrateDictionaries
//        public async Task MigrateDictionariesAsync()
//        {
//            //Console.WriteLine("Enter pls aplly_all_migration.sql path");
//            //var sqlPath = Console.ReadLine();
//            //await ExecuteSqlScriptFromFile(sqlPath);
//            await MigrateCompareModelsAsync();
//            await MigrateTagsAsync();
//            await MigrateCompareAttachFilesAsync();
//        }

//        public async Task MigrateCompareModelsAsync()
//        {
//            var oldTypes = typeof(Depricated.DataAccess.DepricatedUdictServerTwo.Models.AdministrativeUnit).Assembly
//                .GetTypes()
//                .Where(m => m.GetCustomAttribute<MigrationCompareAttribute>() != null)
//                .ToList();

//            foreach (var oldType in oldTypes)
//            {
//                try
//                {
//                    var newType = oldType.GetCustomAttribute<MigrationCompareAttribute>()?.CompareToType;

//                    MethodInfo oldSetMethod = typeof(DepricatedUdictServerTwoDbContext).GetMethods().First(m => m.Name.Equals("Set"));
//                    MethodInfo oldGenericSetMethod = oldSetMethod.MakeGenericMethod(oldType);
//                    IQueryable<object> oldQuery = (IQueryable<object>)(oldGenericSetMethod.Invoke(DepricatedUdictServerTwoDbContext, null));


//                    MethodInfo setMethod = typeof(MasofaDictionariesDbContext).GetMethods().First(m => m.Name.Equals("Set"));
//                    MethodInfo genericSetMethod = oldSetMethod.MakeGenericMethod(newType);
//                    IQueryable<object> query = (IQueryable<object>)(oldGenericSetMethod.Invoke(MasofaDictionariesDbContext, null));

//                    foreach (var item in oldQuery)
//                    {
//                        try
//                        {

//                            var crUser = item.GetType().GetProperty("CreateUser").GetValue(item).ToString();
//                            var upUser = item.GetType().GetProperty("UpdateUser").GetValue(item).ToString();
//                            var crUserId = ResolveUserId(crUser);
//                            var upUserId = ResolveUserId(upUser);

//                            var newItem = typeof(ImplicitConverters).GetMethods()
//                                .First(m => m.GetParameters().Any(p => p.ParameterType == oldType))
//                                .Invoke(null, [item]);
//                            newType.GetProperty("CreateUser").SetValue(newItem, crUserId);
//                            newType.GetProperty("LastUpdateUser").SetValue(newItem, upUserId);
//                            typeof(DbSetAddInvoce).GetMethods()
//                                .First(m => m.GetParameters().Any(p => p.ParameterType == newType))
//                                .Invoke(null, [newItem, MasofaDictionariesDbContext]);
//                            Console.WriteLine($"SUCCESS: Add new Dictonary item with type {newType.Name} and id {newType.GetProperty("Id").GetValue(newItem)}");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"ERROR: {ex.Message}");
//                            Console.WriteLine($"ERROR: {ex.InnerException?.Message}");
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"ERROR: {ex.Message}");
//                    Console.WriteLine($"ERROR: {ex.InnerException?.Message}");
//                }

//            }

//        }

//        public async Task MigrateTagsAsync()
//        {
//            var oldTags = DepricatedUdictServerTwoDbContext.Tags.ToList();
//            //данные сломаны
//        }

//        public async Task MigrateCompareAttachFilesAsync()
//        {
//            var oldFiles = DepricatedUdictServerTwoDbContext.AttachedFiles.ToList();
//            //данные сломаны
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

//        public Task Execute(string[] args)
//        {
//            return Execute();
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
//        //        await MasofaDictionariesDbContext.Database.ExecuteSqlRawAsync(sql);
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
