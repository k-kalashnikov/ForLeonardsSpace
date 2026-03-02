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
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Migration
//{
//    [BaseCommand("Migrate Identity", "Миграция данных пользователей и ролей из старой системы в новую")]
//    public class MigrateIdentityCommand : IBaseCommand
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
//        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }


//        public MigrateIdentityCommand(MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext, MasofaCommonDbContext masofaCommonDbContext,
//            MasofaDictionariesDbContext masofaDictionariesDbContext, UserManager<User> userManager, RoleManager<Role> roleManager,
//            DepricatedAuthServerTwoDbContext depricatedAuthServerTwoDbContext,
//            DepricatedUmapiServerTwoDbContext depricatedUmapiServerTwoDbContext,
//            DepricatedUmapiServerOneDbContext depricatedUmapiServerOneDbContext,
//            DepricatedAuthServerOneDbContext depricatedAuthServerOneDbContext,
//            DepricatedUalertsServerOneDbContext depricatedUalertsServerOneContext,
//            DepricatedUdictServerTwoDbContext depricatedUdictServerTwoDbContext,
//            MasofaIdentityDbContext masofaIdentityDbContext)
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
//            MasofaIdentityDbContext = masofaIdentityDbContext;
//        }

//        public async Task Execute()
//        {
//            await MigrateIdentityAsync();
//        }

//        public void Dispose()
//        {

//        }

//        #region MigrateIdentity
//        private async Task MigrateIdentityAsync()
//        {
//            //Console.WriteLine("Enter pls aplly_all_migration.sql path");
//            //var sqlPath = Console.ReadLine();
//            //await ExecuteSqlScriptFromFile(sqlPath);
//            await MigrateIdentityOneAsync();
//            await MigrateIdentityTwoAsync();
//        }
//        private async Task MigrateIdentityOneAsync()
//        {
//            var oldUsers = DepricatedAuthServerOneDbContext.AspNetUsers.ToList();
//            var oldRoles = DepricatedUmapiServerOneDbContext.UserTypes.ToList();
//            var oldUserTree = DepricatedUmapiServerOneDbContext.Users.ToList();

//            await MigrateRolesOneAsync(oldRoles);
//            await MigrateUserOneAsync(oldUsers);
//            await MigrateUserTreeOneAsync(oldUsers, oldRoles, oldUserTree);
//        }

//        private async Task MigrateRolesOneAsync(List<Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models.UserType> oldRoles)
//        {
//            foreach (var userRoles in oldRoles)
//            {
//                if (await RoleManager.RoleExistsAsync(userRoles.NameEn.ToLower()))
//                {
//                    continue;
//                }

//                var result = await RoleManager.CreateAsync(new Role()
//                {
//                    NormalizedName = userRoles.NameEn.ToUpper(),
//                    Name = userRoles.NameEn.ToLower(),
//                    NameRu = userRoles.Name.ToLower(),
//                    NameEn = userRoles.NameEn.ToLower(),
//                });

//                if (result.Succeeded)
//                {
//                    Console.WriteLine($"Add new role {userRoles.NameEn} from {typeof(Depricated.DataAccess.DepricatedUmapiServerOne.DepricatedUmapiServerOneDbContext).Name}");
//                    continue;
//                }

//                Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
//            }
//        }

//        private async Task MigrateUserOneAsync(List<Masofa.Depricated.DataAccess.DepricatedAuthServerOne.Models.AspNetUser> oldUsers)
//        {
//            var logMessages = new List<string>();
//            foreach (var oldUser in oldUsers)
//            {
//                if ((UserManager.Users.Any(m => m.UserName.Equals(oldUser.UserName))))
//                {
//                    continue;
//                }

//                var newPassword = PasswordGenerator.GeneratePassword(8);
//                var result = await UserManager.CreateAsync(new User()
//                {
//                    UserName = oldUser.UserName,
//                    Email = oldUser.UserName,
//                    FirstName = oldUser.FirstName,
//                    LastName = oldUser.LastName,
//                }, newPassword);

//                if (result.Succeeded)
//                {
//                    Console.WriteLine($"Add new user {oldUser.UserName} from {typeof(Depricated.DataAccess.DepricatedAuthServerOne.DepricatedAuthServerOneDbContext).Name}");
//                    logMessages.Add($"Login: {oldUser.UserName}; Password: {newPassword}");
//                    continue;
//                }

//                Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
//            }
//            File.AppendAllLines("users.txt", logMessages);
//        }

//        private async Task MigrateUserTreeOneAsync(List<Masofa.Depricated.DataAccess.DepricatedAuthServerOne.Models.AspNetUser> oldUsers,
//            List<Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models.UserType> oldRoles,
//            List<Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models.User> oldUserTree)
//        {
//            foreach (var oldUser in oldUserTree)
//            {
//                if (!oldUser.ParentId.HasValue)
//                {
//                    continue;
//                }
//                var oldUserName = oldUsers.FirstOrDefault(m => m.Id.Equals(oldUser.Id))?.UserName ?? string.Empty;
//                var oldParentName = oldUsers.FirstOrDefault(m => m.Id.Equals(oldUser.ParentId))?.UserName ?? string.Empty;

//                if (string.IsNullOrEmpty(oldUserName))
//                {
//                    Console.WriteLine($"ERROR: User from tree with Id {oldUser.Id} not found in old Auth");
//                    continue;
//                }

//                if (string.IsNullOrEmpty(oldParentName))
//                {
//                    Console.WriteLine($"ERROR: User from tree with Id {oldUser.ParentId.Value} not found in old Auth");
//                    continue;
//                }

//                if (!(UserManager.Users.Any(m => m.UserName.Equals(oldUserName))))
//                {
//                    Console.WriteLine($"ERROR: User from old Auth with Name {oldUserName} not migrated!!!");
//                    continue;
//                }

//                if (!(UserManager.Users.Any(m => m.UserName.Equals(oldParentName))))
//                {
//                    Console.WriteLine($"ERROR: User from old Auth with Name {oldParentName} not migrated!!!");
//                    continue;
//                }

//                var currentUser = await UserManager.Users.FirstOrDefaultAsync(m => m.UserName.Equals(oldUserName));
//                var currentParentId = (await UserManager.Users.FirstOrDefaultAsync(m => m.UserName.Equals(oldParentName))).Id;
//                currentUser.ParentId = currentParentId;
//                var result = await UserManager.UpdateAsync(currentUser);

//                if (result.Succeeded)
//                {
//                    Console.WriteLine($"Add parent fof user {currentUser.UserName}");
//                }
//                else
//                {
//                    Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
//                }

//                var userRole = oldRoles.FirstOrDefault(m => m.Id.Equals(oldUser.TypeId))?.NameEn?.ToLower() ?? string.Empty;
//                if (string.IsNullOrEmpty(userRole))
//                {
//                    Console.WriteLine($"ERROR: UserType from old with Id {oldUser.TypeId} not found!!!");
//                    continue;
//                }

//                if ((await UserManager.IsInRoleAsync(currentUser, userRole)))
//                {
//                    continue;
//                }

//                result = await UserManager.AddToRoleAsync(currentUser, userRole);

//                if (result.Succeeded)
//                {
//                    Console.WriteLine($"Add role for user {currentUser.UserName}");
//                }
//                else
//                {
//                    Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
//                }
//            }
//        }

//        private async Task MigrateIdentityTwoAsync()
//        {
//            var oldUsers = DepricatedAuthServerTwoDbContext.AspNetUsers.ToList();
//            var oldRoles = DepricatedUmapiServerTwoDbContext.UserTypes.ToList();
//            var oldUserTree = DepricatedUmapiServerTwoDbContext.Users.ToList();

//            await MigrateRolesTwoAsync(oldRoles);
//            await MigrateUserTwoAsync(oldUsers);
//            await MigrateUserTreeTwoAsync(oldUsers, oldRoles, oldUserTree);
//        }

//        private async Task MigrateRolesTwoAsync(List<Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models.UserType> oldRoles)
//        {
//            foreach (var userRoles in oldRoles)
//            {
//                if (await RoleManager.RoleExistsAsync(userRoles.NameEn.ToLower()))
//                {
//                    continue;
//                }

//                var result = await RoleManager.CreateAsync(new Role()
//                {
//                    NormalizedName = userRoles.NameEn.ToUpper(),
//                    Name = userRoles.NameEn.ToLower(),
//                    NameRu = userRoles.Name.ToLower(),
//                    NameEn = userRoles.NameEn.ToLower(),
//                });

//                if (result.Succeeded)
//                {
//                    Console.WriteLine($"Add new role {userRoles.NameEn} from {typeof(Depricated.DataAccess.DepricatedUmapiServerTwo.DepricatedUmapiServerTwoDbContext).Name}");
//                    continue;
//                }

//                Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
//            }
//        }

//        private async Task MigrateUserTwoAsync(List<Masofa.Depricated.DataAccess.DepricatedAuthServerTwo.Models.AspNetUser> oldUsers)
//        {
//            var logMessages = new List<string>();

//            foreach (var oldUser in oldUsers)
//            {
//                if ((UserManager.Users.Any(m => m.UserName.Equals(oldUser.UserName))))
//                {
//                    continue;
//                }

//                var newPassword = PasswordGenerator.GeneratePassword(8);
//                var result = await UserManager.CreateAsync(new User()
//                {
//                    UserName = oldUser.UserName,
//                    Email = oldUser.UserName,
//                    FirstName = oldUser.FirstName,
//                    LastName = oldUser.LastName,
//                }, newPassword);

//                if (result.Succeeded)
//                {
//                    Console.WriteLine($"Add new user {oldUser.UserName} from {typeof(Depricated.DataAccess.DepricatedAuthServerTwo.DepricatedAuthServerTwoDbContext).Name}");
//                    logMessages.Add($"Login: {oldUser.UserName}; Password: {newPassword}");
//                    continue;
//                }

//                Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
//            }
//            File.AppendAllLines("users.txt", logMessages);
//        }

//        private async Task MigrateUserTreeTwoAsync(List<Masofa.Depricated.DataAccess.DepricatedAuthServerTwo.Models.AspNetUser> oldUsers,
//            List<Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models.UserType> oldRoles,
//            List<Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models.User> oldUserTree)
//        {
//            foreach (var oldUser in oldUserTree)
//            {
//                if (!oldUser.ParentId.HasValue)
//                {
//                    continue;
//                }
//                var oldUserName = oldUsers.FirstOrDefault(m => m.Id.Equals(oldUser.Id))?.UserName ?? string.Empty;
//                var oldParentName = oldUsers.FirstOrDefault(m => m.Id.Equals(oldUser.ParentId))?.UserName ?? string.Empty;
//                if (string.IsNullOrEmpty(oldUserName))
//                {
//                    Console.WriteLine($"ERROR: User from tree with Id {oldUser.Id} not found in old Auth");
//                    continue;
//                }

//                if (string.IsNullOrEmpty(oldParentName))
//                {
//                    Console.WriteLine($"ERROR: User from tree with Id {oldUser.ParentId.Value} not found in old Auth");
//                    continue;
//                }

//                if (!(UserManager.Users.Any(m => m.UserName.Equals(oldUserName))))
//                {
//                    Console.WriteLine($"ERROR: User from old Auth with Name {oldUserName} not migrated!!!");
//                    continue;
//                }

//                if (!(UserManager.Users.Any(m => m.UserName.Equals(oldParentName))))
//                {
//                    Console.WriteLine($"ERROR: User from old Auth with Name {oldParentName} not migrated!!!");
//                    continue;
//                }

//                var currentUser = await UserManager.Users.FirstOrDefaultAsync(m => m.UserName.Equals(oldUserName));
//                var currentParentId = (await UserManager.Users.FirstOrDefaultAsync(m => m.UserName.Equals(oldParentName))).Id;
//                currentUser.ParentId = currentParentId;
//                var result = await UserManager.UpdateAsync(currentUser);

//                if (result.Succeeded)
//                {
//                    Console.WriteLine($"Add parent fof user {currentUser.UserName}");
//                }
//                else
//                {
//                    Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
//                }

//                var userRole = oldRoles.FirstOrDefault(m => m.Id.Equals(oldUser.TypeId))?.NameEn?.ToLower() ?? string.Empty;
//                if (string.IsNullOrEmpty(userRole))
//                {
//                    Console.WriteLine($"ERROR: UserType from old with Id {oldUser.TypeId} not found!!!");
//                    continue;
//                }

//                result = await UserManager.AddToRoleAsync(currentUser, userRole);

//                if (result.Succeeded)
//                {
//                    Console.WriteLine($"Add role for user {currentUser.UserName}");
//                }
//                else
//                {
//                    Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
//                }
//            }
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
//        //        await MasofaIdentityDbContext.Database.ExecuteSqlRawAsync(sql);
//        //        return true;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"Ошибка выполнения SQL скрипта: {ex.Message}");
//        //        return false;
//        //    }
//        //}

//        #endregion

//    }
//}
