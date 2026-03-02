using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using Masofa.Depricated.DataAccess.DepricatedAuthServerOne;
using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;
using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;
using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerOne;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Cli.Migration
{
    public class MigrateIdentityCommand : IRequest
    {

    }

    public class MigrateIdentityCommandHandler : IRequestHandler<MigrateIdentityCommand>
    {
        public MigrateIdentityCommandHandler(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext,
            MasofaCommonDbContext masofaCommonDbContext,
            MasofaDictionariesDbContext masofaDictionariesDbContext,
            UserManager<User> userManager,
            DepricatedUmapiServerOneDbContext depricatedUmapiServerOneDbContext,
            DepricatedAuthServerOneDbContext depricatedAuthServerOneDbContext,
            DepricatedUmapiServerTwoDbContext depricatedUmapiServerTwoDbContext,
            DepricatedAuthServerTwoDbContext depricatedAuthServerTwoDbContext,
            DepricatedUdictServerTwoDbContext depricatedUdictServerTwoDbContext,
            RoleManager<Role> roleManager,
            DepricatedUalertsServerOneDbContext depricatedUalertsServerOneDbContext,
            MasofaIdentityDbContext masofaIdentityDbContext)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaCommonDbContext = masofaCommonDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            UserManager = userManager;
            DepricatedUmapiServerOneDbContext = depricatedUmapiServerOneDbContext;
            DepricatedAuthServerOneDbContext = depricatedAuthServerOneDbContext;
            DepricatedUmapiServerTwoDbContext = depricatedUmapiServerTwoDbContext;
            DepricatedAuthServerTwoDbContext = depricatedAuthServerTwoDbContext;
            DepricatedUdictServerTwoDbContext = depricatedUdictServerTwoDbContext;
            RoleManager = roleManager;
            DepricatedUalertsServerOneDbContext = depricatedUalertsServerOneDbContext;
            MasofaIdentityDbContext = masofaIdentityDbContext;
        }

        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private UserManager<Masofa.Common.Models.Identity.User> UserManager { get; set; }
        private DepricatedUmapiServerOneDbContext DepricatedUmapiServerOneDbContext { get; set; }
        private DepricatedAuthServerOneDbContext DepricatedAuthServerOneDbContext { get; set; }
        private DepricatedUmapiServerTwoDbContext DepricatedUmapiServerTwoDbContext { get; set; }
        private DepricatedAuthServerTwoDbContext DepricatedAuthServerTwoDbContext { get; set; }
        private DepricatedUdictServerTwoDbContext DepricatedUdictServerTwoDbContext { get; set; }
        private RoleManager<Masofa.Common.Models.Identity.Role> RoleManager { get; set; }
        private DepricatedUalertsServerOneDbContext DepricatedUalertsServerOneDbContext { get; set; }
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }

        public async Task Handle(MigrateIdentityCommand request, CancellationToken cancellationToken)
        {
            await MigrateIdentityAsync();
        }

        private async Task MigrateIdentityAsync()
        {
            //Console.WriteLine("Enter pls aplly_all_migration.sql path");
            //var sqlPath = Console.ReadLine();
            //await ExecuteSqlScriptFromFile(sqlPath);
            await MigrateIdentityOneAsync();
            await MigrateIdentityTwoAsync();
        }

        private async Task MigrateIdentityOneAsync()
        {
            var oldUsers = DepricatedAuthServerOneDbContext.AspNetUsers.ToList();
            var oldRoles = DepricatedUmapiServerOneDbContext.UserTypes.ToList();
            var oldUserTree = DepricatedUmapiServerOneDbContext.Users.ToList();

            await MigrateUserOneAsync(oldUsers);
            await MigrateUserTreeOneAsync(oldUsers, oldRoles, oldUserTree);
        }

        private async Task MigrateUserOneAsync(List<Masofa.Depricated.DataAccess.DepricatedAuthServerOne.Models.AspNetUser> oldUsers)
        {
            var logMessages = new List<string>();
            foreach (var oldUser in oldUsers)
            {
                if ((UserManager.Users.Any(m => m.UserName.Equals(oldUser.UserName))))
                {
                    continue;
                }

                var newPassword = PasswordGeneratorHelper.GeneratePassword(8);
                var result = await UserManager.CreateAsync(new User()
                {
                    Id = oldUser.Id, // Сохраняем старый ID для связей с заявками
                    UserName = oldUser.UserName,
                    Email = oldUser.UserName,
                    FirstName = oldUser.FirstName,
                    LastName = oldUser.LastName,
                }, newPassword);

                if (result.Succeeded)
                {
                    Console.WriteLine($"Add new user {oldUser.UserName} from {typeof(Depricated.DataAccess.DepricatedAuthServerOne.DepricatedAuthServerOneDbContext).Name}");
                    logMessages.Add($"Login: {oldUser.UserName}; Password: {newPassword}");
                    continue;
                }

                Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
            }
            File.AppendAllLines("users.txt", logMessages);
        }

        private async Task MigrateUserTreeOneAsync(List<Masofa.Depricated.DataAccess.DepricatedAuthServerOne.Models.AspNetUser> oldUsers,
            List<Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models.UserType> oldRoles,
            List<Masofa.Depricated.DataAccess.DepricatedUmapiServerOne.Models.User> oldUserTree)
        {
            foreach (var oldUser in oldUserTree)
            {
                if (!oldUser.ParentId.HasValue)
                {
                    continue;
                }
                var oldUserName = oldUsers.FirstOrDefault(m => m.Id.Equals(oldUser.Id))?.UserName ?? string.Empty;
                var oldParentName = oldUsers.FirstOrDefault(m => m.Id.Equals(oldUser.ParentId))?.UserName ?? string.Empty;

                if (string.IsNullOrEmpty(oldUserName))
                {
                    Console.WriteLine($"ERROR: User from tree with Id {oldUser.Id} not found in old Auth");
                    continue;
                }

                if (string.IsNullOrEmpty(oldParentName))
                {
                    Console.WriteLine($"ERROR: User from tree with Id {oldUser.ParentId.Value} not found in old Auth");
                    continue;
                }

                if (!(UserManager.Users.Any(m => m.UserName.Equals(oldUserName))))
                {
                    Console.WriteLine($"ERROR: User from old Auth with Name {oldUserName} not migrated!!!");
                    continue;
                }

                if (!(UserManager.Users.Any(m => m.UserName.Equals(oldParentName))))
                {
                    Console.WriteLine($"ERROR: User from old Auth with Name {oldParentName} not migrated!!!");
                    continue;
                }

                var currentUser = await UserManager.Users.FirstOrDefaultAsync(m => m.UserName.Equals(oldUserName));
                var currentParentId = (await UserManager.Users.FirstOrDefaultAsync(m => m.UserName.Equals(oldParentName))).Id;
                currentUser.ParentId = currentParentId;
                var result = await UserManager.UpdateAsync(currentUser);

                if (result.Succeeded)
                {
                    Console.WriteLine($"Add parent for user {currentUser.UserName}");
                }
                else
                {
                    Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
                }

                var userRole = oldRoles.FirstOrDefault(m => m.Id.Equals(oldUser.TypeId))?.NameEn?.ToLower() ?? string.Empty;
                if (string.IsNullOrEmpty(userRole))
                {
                    Console.WriteLine($"ERROR: UserType from old with Id {oldUser.TypeId} not found!!!");
                    continue;
                }

                if (userRole.Contains("worker")) // Исправление ошибки в старой базе
                {
                    userRole = "fieldworker";
                }

                if ((await UserManager.IsInRoleAsync(currentUser, userRole)))
                {
                    continue;
                }

                result = await UserManager.AddToRoleAsync(currentUser, userRole);

                if (result.Succeeded)
                {
                    Console.WriteLine($"Add role for user {currentUser.UserName}");
                }
                else
                {
                    Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
                }
            }
        }

        private async Task MigrateIdentityTwoAsync()
        {
            var oldUsers = DepricatedAuthServerTwoDbContext.AspNetUsers.ToList();
            var oldRoles = DepricatedUmapiServerTwoDbContext.UserTypes.ToList();
            var oldUserTree = DepricatedUmapiServerTwoDbContext.Users.ToList();

            await MigrateUserTwoAsync(oldUsers);
            await MigrateUserTreeTwoAsync(oldUsers, oldRoles, oldUserTree);
        }

        private async Task MigrateUserTwoAsync(List<Masofa.Depricated.DataAccess.DepricatedAuthServerTwo.Models.AspNetUser> oldUsers)
        {
            var logMessages = new List<string>();
            foreach (var oldUser in oldUsers)
            {
                if ((UserManager.Users.Any(m => m.UserName.Equals(oldUser.UserName))))
                {
                    continue;
                }

                var newPassword = PasswordGeneratorHelper.GeneratePassword(8);
                var result = await UserManager.CreateAsync(new User()
                {
                    Id = oldUser.Id, // Сохраняем старый ID для связей с заявками
                    UserName = oldUser.UserName,
                    Email = oldUser.UserName,
                    FirstName = oldUser.FirstName,
                    LastName = oldUser.LastName,
                }, newPassword);

                if (result.Succeeded)
                {
                    Console.WriteLine($"Add new user {oldUser.UserName} from {typeof(Depricated.DataAccess.DepricatedAuthServerTwo.DepricatedAuthServerTwoDbContext).Name}");
                    logMessages.Add($"Login: {oldUser.UserName}; Password: {newPassword}");
                    continue;
                }

                Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
            }
            File.AppendAllLines("users.txt", logMessages);
        }

        private async Task MigrateUserTreeTwoAsync(List<Masofa.Depricated.DataAccess.DepricatedAuthServerTwo.Models.AspNetUser> oldUsers,
            List<Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models.UserType> oldRoles,
            List<Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models.User> oldUserTree)
        {
            foreach (var oldUser in oldUserTree)
            {
                if (!oldUser.ParentId.HasValue)
                {
                    continue;
                }
                var oldUserName = oldUsers.FirstOrDefault(m => m.Id.Equals(oldUser.Id))?.UserName ?? string.Empty;
                var oldParentName = oldUsers.FirstOrDefault(m => m.Id.Equals(oldUser.ParentId))?.UserName ?? string.Empty;

                if (string.IsNullOrEmpty(oldUserName))
                {
                    Console.WriteLine($"ERROR: User from tree with Id {oldUser.Id} not found in old Auth");
                    continue;
                }

                if (string.IsNullOrEmpty(oldParentName))
                {
                    Console.WriteLine($"ERROR: User from tree with Id {oldUser.ParentId.Value} not found in old Auth");
                    continue;
                }

                if (!(UserManager.Users.Any(m => m.UserName.Equals(oldUserName))))
                {
                    Console.WriteLine($"ERROR: User from old Auth with Name {oldUserName} not migrated!!!");
                    continue;
                }

                if (!(UserManager.Users.Any(m => m.UserName.Equals(oldParentName))))
                {
                    Console.WriteLine($"ERROR: User from old Auth with Name {oldParentName} not migrated!!!");
                    continue;
                }

                var currentUser = await UserManager.Users.FirstOrDefaultAsync(m => m.UserName.Equals(oldUserName));
                var currentParentId = (await UserManager.Users.FirstOrDefaultAsync(m => m.UserName.Equals(oldParentName))).Id;
                currentUser.ParentId = currentParentId;
                var result = await UserManager.UpdateAsync(currentUser);

                if (result.Succeeded)
                {
                    Console.WriteLine($"Add parent for user {currentUser.UserName}");
                }
                else
                {
                    Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
                }

                var userRole = oldRoles.FirstOrDefault(m => m.Id.Equals(oldUser.TypeId))?.NameEn?.ToLower() ?? string.Empty;
                if (string.IsNullOrEmpty(userRole))
                {
                    Console.WriteLine($"ERROR: UserType from old with Id {oldUser.TypeId} not found!!!");
                    continue;
                }

                if (userRole.Contains("worker")) // Исправление ошибки в старой базе
                {
                    userRole = "fieldworker";
                }

                if ((await UserManager.IsInRoleAsync(currentUser, userRole)))
                {
                    continue;
                }

                result = await UserManager.AddToRoleAsync(currentUser, userRole);

                if (result.Succeeded)
                {
                    Console.WriteLine($"Add role for user {currentUser.UserName}");
                }
                else
                {
                    Console.WriteLine($"ERROR: {string.Join("\n", result.Errors.Select(m => $"{m.Code}:{m.Description}"))}");
                }
            }
        }

    }
}


/*
   [
	{
		"Email" : "admin@uz.uz",
		"Name" : "Administrator"
	},
	{
		"Email" : "user@uz.uz",
		"Name" : "User"
	},
	{
		"Email" : "user_cc@6grain.com",
		"Name" : "User"
	},
	{
		"Email" : "admin_cc@6grain.com",
		"Name" : "Administrator"
	},
	{
		"Email" : "m.rusanov@6grain.com",
		"Name" : "Administrator"
	},
	{
		"Email" : "user3@masofa-yer.uz",
		"Name" : "User"
	},
	{
		"Email" : "user4@masofa-yer.uz",
		"Name" : "User"
	},
	{
		"Email" : "user6@masofa-yer.uz",
		"Name" : "User"
	},
	{
		"Email" : "user5@masofa-yer.uz",
		"Name" : "User"
	},
	{
		"Email" : "user_operator@6grain.com",
		"Name" : "Operator"
	},
	{
		"Email" : "user_foreman@6grain.com",
		"Name" : "Foreman"
	},
	{
		"Email" : "test@test.oper",
		"Name" : "Operator"
	},
	{
		"Email" : "user_fw@6grain.com",
		"Name" : "Administrator"
	},
	{
		"Email" : "user_fw1@6grain.com",
		"Name" : "Worker"
	},
	{
		"Email" : "user_fw2@6grain.com",
		"Name" : "Worker"
	},
	{
		"Email" : "user_fw3@6grain.com",
		"Name" : "Worker"
	},
	{
		"Email" : "foreman_cc@uz.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "no_use@mail.ru",
		"Name" : "Operator"
	},
	{
		"Email" : "fw4_cc@uz.uz",
		"Name" : "Administrator"
	},
	{
		"Email" : "fw5_cc@uz.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "fw6_cc@uz.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "OPER@uz.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "user1@masofa-yer.uz",
		"Name" : "Administrator"
	},
	{
		"Email" : "test4@test.tst",
		"Name" : "Worker"
	},
	{
		"Email" : "test@test.asdf",
		"Name" : "Operator"
	},
	{
		"Email" : "user2@masofa-yer.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "operator@test.ru",
		"Name" : "Operator"
	},
	{
		"Email" : "root@uz.uz",
		"Name" : "SuperAdministrator"
	},
	{
		"Email" : "worker@uz.com",
		"Name" : "Worker"
	},
	{
		"Email" : "operator@test.asdf",
		"Name" : "Operator"
	},
	{
		"Email" : "system@6grain.com",
		"Name" : "System"
	},
	{
		"Email" : "f.ortikov@agro.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "nurlymuratov-na@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "dauletmuratova-ga@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "toreeva-zhya@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kulcharov-rm@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "nasretdinov-ka@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "adilov-ab@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "tursinov-ab@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "saparov-khb@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "yuldasheva-gr@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "markova-tp@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "devonaev-ma@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "sodikov-im@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "melikuziev-shsh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mamadzhonova-mi@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "toshtemirov-aa@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kuzieva-gn@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kurbonov-ov@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "akhmedov-du@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "bakaeva-db@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "mukhiddinov-ss@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "farmonov-az@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "toshkinov-khn@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "zubaev-fr@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "normatov-at@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kholyigitov-em@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "saliev-ri@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "korabekov-af@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "sidorova-kv@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "dzhuraeva-as@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "khasanova-rv@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "davronov-in@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "chuliev-au@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "zhuravleva-sa@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "choriev-sb@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mirzaev-sf@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ruzmanova-msh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdurakhmonova-me@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "yadgarov-nzh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "nizamova-nn@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "ashurova-mb@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ergasheva-gn@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "turaeva-zs@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdukarimov-aa@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "turakhanov-kk@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdikodirov-a@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdullaev-fkh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ulukov-mm@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdullaeva-gf@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kholova-nya@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "amirov-okh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kholmukhamedova-dn@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kayumova-gkh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "boybekova-nm@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "makhsimov-zb@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "shokirov-mf@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "zhumanov-go@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kamolov-ta@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kurbonsaidov-oy@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "gaipova-dkh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "murotov-zht@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ergashev-skh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mengnarov-sha@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "zulkhaydarov-im@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "sharofiddinov-aa@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "eshmurzaev-nd@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ismailova-ee@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdaliev-ka@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "murotov-bb@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "khaydarova-sha@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "sharafutdinova-nr@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "khamraeva-rb@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kodirova-fr@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "orlova-as@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "sazonova-li@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "pasynkova-na@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "nigay-lkh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "tozhiev-akh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ismoilova-ms@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "tilyakhodzhaeva-mz@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "dzhumabaev-tt@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "bakhtierkhonov-kb@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "saidzhonova-lsh@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "kalandarova-fk@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "khamraev-sho@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "dosumova-nm@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "erkabaev-tr@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "rakhimbaev-zhg@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "orunov-ba@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "lev-ni@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "tutieva-mk@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "feldman-lv@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "testpassword@test.ru",
		"Name" : "Administrator"
	},
	{
		"Email" : "butaeva-gkh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "askarova-em@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdullayeva-in@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "xayitov-ub@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "test@password.ru",
		"Name" : "Administrator"
	},
	{
		"Email" : "temp@test",
		"Name" : "User"
	},
	{
		"Email" : "fields_request@6grain.com",
		"Name" : "Operator"
	},
	{
		"Email" : "rbigildin6@gmail.com",
		"Name" : "Foreman"
	},
	{
		"Email" : "boboqul77@mail.ru",
		"Name" : "Foreman"
	},
	{
		"Email" : "b.akromov@mail.ru",
		"Name" : "Foreman"
	},
	{
		"Email" : "lev-ni-b@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "lev-ni-w@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "tutieva-mk-w@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "tutieva-mk-b@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "feldman-lv-w@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "m@m.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "fw_cc@uz.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "evz37@yandex.ru",
		"Name" : "Administrator"
	},
	{
		"Email" : "maks@maks.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "maxshnn@yandex.com",
		"Name" : "Foreman"
	},
	{
		"Email" : "lev-ni_b@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "lev-ni_w@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ozodbekabdurashidov13@gmail.com",
		"Name" : "Worker"
	},
	{
		"Email" : "inomjonnabiyev3477@gmail.com",
		"Name" : "Worker"
	},
	{
		"Email" : "ruzieva-fa@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mamaev-do@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kholova-di@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "khokhlova-di@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "mengnarova-ba@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "saidov-fa@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "saidov-fa-b@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "azamov-khr@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "feldman-lv-b@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "feldman-lv-br@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "khazratova-ze@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "numonzhanov-ru@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "omonullayev-af-b@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "omonullayev-af@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "mavlyanov-ar@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "isamutdinova-umr@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "xaydarova-fo@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "mominova-di@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "sadriddinov-sho@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mavlyanov-ar-o@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "ortikov-f@agro.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "mustarov-ab@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "shokirboyev-zi@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "veliulova-gy@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "xaydarova-fot@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "xaydarova-fo-w@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "saidov-fa-w@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mominova-di-o@meteo.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "mominova-di-b@meteo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "yunusov-sa@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "suyarkulov-ru@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "suyarkulov-ry@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "eshonqulov-sh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "usmonov-ab@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abduraupov-sh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "odilov-yo@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mihailova-an@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mihajlova-an@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "sharapova-vi@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mirzahanova-za@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mirzayeva-um@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "buvrayeva-la@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "igamberdiyeva-na@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "raximjonova-sho@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "k-kalashnikov73@yandex.ru",
		"Name" : "Operator"
	},
	{
		"Email" : "azimova-zu@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "fayzullayeva-di@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abduqodirov-ak@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "nadirkulova-da@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "norov-na@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ibragimxodzayeva-ma@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdieva-ma@meteo.uz",
		"Name" : "Administrator"
	},
	{
		"Email" : "nunayev-al@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "avazov-ja@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdieva-mar@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "normamatova-sa@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "baxronov-ax@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "majidov-ra@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "nematova-ni@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "boboev-ja@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "dosbekov-ul@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "qalandarova-mu@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "ergasheva-gu@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abdullaeva-ma@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "khazratova-pa@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mamadolimova-ki@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "mamajonova-me@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "muxtarov-hu@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "divanaev-ma@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "sodiqov-ik@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kuzieva-gu@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "tashlanov-as@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "nomongonov-ru@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "gostisheva-sv@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "jurayev-te@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "maxanbetov-ku@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "aimbetova-ul@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "kabulov-ai@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "doshimova-ay@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "nasratdinov-qu@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "egamberdieva-ra@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "aliev-mu@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "eazhimuratova-zi@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "tazhimuratova-zi@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "begmuratova-di@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "sarsenbaeva-ar@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "nazarbaev-ad@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "yuldasheva-va@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "imomova-zi@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "abduvahobov-sh@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "uldoshova-gu@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "eshmurzaeva-li@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "yuldasheva-mo@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "yuldasheva-mox@meteo.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Makhsudov-Bu@argo.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Makhsudov-Bu@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Zhuraev-Zh@agro.uz",
		"Name" : "Operator"
	},
	{
		"Email" : "Zhuraev-Zh-b@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Niyozov-Fa@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Mamajonov-Ab@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Tursunov-Tu@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Rezhavaliev-Ab@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Karaboev-Ah@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Nasirdinov-Yu@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Osarov-Al@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Alikulov-Ga@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Alikulov-Kh@agro.uz",
		"Name" : "Administrator"
	},
	{
		"Email" : "Dekhkonov-Av@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Abukhamidov-Ab@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Kamolova-Ta@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Shokirov-La@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Mamajonov-So@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Kurbonov-Ra@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Bozorov-Nu@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Toshkuziev-Zh@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Dosmatov-Do@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Shomakhmudov-Zh@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Abdurakhmonov-Ni@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Madaminov-Sh@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Abdujalilov-Mu@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Bokiev-Hi@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Ergasheva-Ka@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Kuziboev-Sh@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Abdullaev-Ot@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Tuychiev-Mu@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Tursunov-Ma@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Orinboyev-So@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Vakhobov-Mu@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Muhammadaliyev-Ab@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Abdurakhimov-Er@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Ismailov-Ad@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Khaliljonov-Ad@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Kholmurojonov-Ja@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Abdulkhamidov-Ka@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Khaidarov-Er@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Sobirov-Mu@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Yakubov-Is@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Rakhmonlaliev-Ib@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Salaev-Or@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Isaboev-Bu@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Muminov-Is@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "Burkhonov-Ra@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Ermatov-An@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Alikulov-Xu@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "Khoshimov-Ab@agro.uz",
		"Name" : "Worker"
	},
	{
		"Email" : "myminov-is@agro.uz",
		"Name" : "Foreman"
	},
	{
		"Email" : "qutbiddinov-sh@meteo.uz",
		"Name" : "Worker"
	}
]

 */