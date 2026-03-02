
//using Masofa.Common.Models;
//﻿using Masofa.Common.Models.CropMonitoring;
//using Masofa.Common.Models.Dictionaries;
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
//using Microsoft.IdentityModel.Tokens;
//using NetTopologySuite.Index.HPRtree;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using NodaTime;
//using Masofa.Common.Models.System;
//using Masofa.Cli.DevopsUtil.Converters;

//namespace Masofa.Cli.DevopsUtil.Commands.Migration
//{
//    [BaseCommand("Compare Data", "Сравнение и миграция данных между различными контекстами базы данных")]
//    public class CompareDataCommand : IBaseCommand
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


//        public CompareDataCommand(MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext, MasofaCommonDbContext masofaCommonDbContext,
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

//        public void Dispose()
//        {

//        }

//        public async Task Execute()
//        {
//            //await MigrateIdentityAsync();
//            //await MigrateDictionaries();
//            //await MigrateTemplatesAsync();
//            await MigrateBidsAsync();
//        }

//        #region MigrateIdentity
//        private async Task MigrateIdentityAsync()
//        {
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


//        #endregion

//        #region MigrateDictionaries
//        public async Task MigrateDictionaries()
//        {
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

//                MasofaCropMonitoringDbContext.BidTemplates.Add(new BidTemplate()
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
//                });

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



//                var currentBidTypeId = MasofaDictionariesDbContext.BidTypes.Any(m => m.Id.Equals(oldBid.BidTypeId))
//                    ? oldBid.BidTypeId
//                    : MasofaDictionariesDbContext.BidTypes.First(m => m.NameEn.ToLower().Equals(DepricatedUmapiServerOneDbContext.BidTypes.First(m => m.Id.Equals(oldBid.BidTypeId)).NameEn.ToLower())).Id;
//                var currentRegionId = MasofaDictionariesDbContext.Regions.Any(m => m.Id.Equals(oldBid.RegionId))
//                    ? oldBid.RegionId
//                    : Guid.Empty;
//                var currentVarietyId = MasofaDictionariesDbContext.Varieties.Any(m => m.Id.Equals(oldBid.RegionId))
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



//                var currentBidTypeId = (oldBid.BidTypeId == Guid.Empty) ? Guid.Empty
//                    : (MasofaDictionariesDbContext.BidTypes.Any(m => m.Id.Equals(oldBid.BidTypeId))
//                    ? oldBid.BidTypeId
//                    : MasofaDictionariesDbContext.BidTypes.First(m => m.NameEn.ToLower().Equals(DepricatedUmapiServerTwoDbContext.BidTypes.First(m => m.Id.Equals(oldBid.BidTypeId)).NameEn.ToLower())).Id);
//                var currentRegionId = MasofaDictionariesDbContext.Regions.Any(m => m.Id.Equals(oldBid.RegionId))
//                    ? oldBid.RegionId
//                    : Guid.Empty;
//                var currentVarietyId = MasofaDictionariesDbContext.Varieties.Any(m => m.Id.Equals(oldBid.RegionId))
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
//           return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day)
//                .AddHours(localDateTime.Hour)
//                .AddMinutes(localDateTime.Minute)
//                .ToUniversalTime();
//        }

//        private DateTime ResolveDateTime(LocalDate localDateTime)
//        {
//            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day).ToUniversalTime();
//        }

//        public Task Execute(string[] args)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public static class PasswordGenerator
//    {
//        private static readonly Random Random = new Random();
//        // Буквы: строчные и заглавные
//        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
//        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
//        private const string Digits = "0123456789";
//        private const string SpecialChars = "!@#$%^&*()_-+=<>?";

//        // Общий алфавит (можно расширить)
//        private const string AllChars = Lowercase + Uppercase;

//        /// <summary>
//        /// Генерирует пароль по заданным правилам Identity.
//        /// Длина >= 6, содержит минимум 1 строчную и 1 заглавную букву.
//        /// </summary>
//        /// <param name="length">Длина пароля (минимум 6)</param>
//        /// <returns>Сгенерированный пароль</returns>
//        public static string GeneratePassword(int length = 8)
//        {
//            if (length < 6)
//                throw new ArgumentException("Длина должна быть не менее 6 символов.", nameof(length));

//            var password = new char[length];
//            int position;

//            // Шаг 1: гарантируем наличие строчной буквы
//            position = Random.Next(0, length);
//            password[position] = Lowercase[Random.Next(Lowercase.Length)];

//            // Шаг 2: гарантируем наличие заглавной буквы
//            do
//            {
//                position = Random.Next(0, length);
//            } while (password[position] != '\0'); // чтобы не перезаписать предыдущую

//            password[position] = Uppercase[Random.Next(Uppercase.Length)];

//            // Шаг 3: заполняем оставшиеся позиции любыми символами (буквы, цифры)
//            for (int i = 0; i < length; i++)
//            {
//                if (password[i] == '\0') // свободная позиция
//                {
//                    password[i] = AllChars[Random.Next(AllChars.Length)];
//                }
//            }

//            return new string(password);
//        }
//    }
//}
