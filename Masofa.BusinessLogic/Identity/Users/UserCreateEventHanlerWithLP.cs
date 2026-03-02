using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RazorLight.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Identity.Users
{
    public class UserCreateEventHanlerWithLP : INotificationHandler<UserCreateEvent>
    {
        private IBusinessLogicLogger BusinessLogicLogger {  get; set; }
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private RoleManager<Role> RoleManager { get; set; }
        private UserManager<User> UserManager { get; set; }
        private ILogger<UserCreateEventHanlerWithLP> Logger { get; set; }
        public UserCreateEventHanlerWithLP(IBusinessLogicLogger businessLogicLogger,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            RoleManager<Role> roleManager, UserManager<User> userManager, ILogger<UserCreateEventHanlerWithLP> logger)
        {
            BusinessLogicLogger = businessLogicLogger;
            CommonDbContext = commonDbContext;
            IdentityDbContext = identityDbContext;
            Logger = logger;
            RoleManager = roleManager;
            UserManager = userManager;
        }
        public async Task Handle(UserCreateEvent notification, CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Handling UserCreateEvent for User ID: {notification.User.Id}");
            var userRoles = await UserManager.GetRolesAsync(notification.User);
            if (!userRoles.Any())
            {
                Logger.LogWarning($"User {notification.User.Id} has no roles assigned. No LockPermissions will be created.");
                return;
            }
            var allowedEntitiesAndActions = new HashSet<(string TypeName, string Action)>();
            var allAccessMapItems = await CommonDbContext.AccessMapItems.ToListAsync(cancellationToken);

            foreach (var roleName in userRoles)
            {
                foreach (var accessMapItem in allAccessMapItems)
                {
                    Dictionary<string, bool> rolesDict;
                    try
                    {
                        rolesDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(accessMapItem.RolesJson);
                    }
                    catch (JsonException ex)
                    {
                        Logger.LogError(ex, $"Failed to deserialize RolesJson for AccessMapItem URL: {accessMapItem.Url}. Skipping.");
                        continue;
                    }

                    if (rolesDict != null && rolesDict.TryGetValue(roleName, out bool hasAccess) && hasAccess)
                    {

                        var (typeName, action) = ParseUrlToEntityAndAction(accessMapItem.Url);
                        if (typeName != null && action != null)
                        {
                            allowedEntitiesAndActions.Add((typeName, action));
                        }
                    }
                }
            }

            var allPossibleEntitiesAndActions = new HashSet<(string TypeName, string Action)>();

            foreach (var accessMapItem in allAccessMapItems)
            {
                var (typeName, action) = ParseUrlToEntityAndAction(accessMapItem.Url);
                if (typeName != null && action != null)
                {
                    allPossibleEntitiesAndActions.Add((typeName, action));
                }
            }

            var deniedEntitiesAndActions = allPossibleEntitiesAndActions.Except(allowedEntitiesAndActions);

            var lockPermissionsToCreate = new List<LockPermission>();

            foreach (var (typeName, action) in deniedEntitiesAndActions)
            {
                var lockPermission = new LockPermission
                {
                    UserId = notification.User.Id,
                    EntityTypeName = typeName,
                    EntityAction = action,
                    LockPermissionType = LockPermissionType.Action, // Или LockPermissionType.Entity
                    // EntityId = null; 
                };
                lockPermissionsToCreate.Add(lockPermission);
            }

            if (lockPermissionsToCreate.Any())
            {
                IdentityDbContext.LockPermissions.AddRange(lockPermissionsToCreate);
                await IdentityDbContext.SaveChangesAsync(cancellationToken);
                Logger.LogInformation($"Created {lockPermissionsToCreate.Count} LockPermissions for User ID: {notification.User.Id}");
            }
            else
            {
                Logger.LogInformation($"No LockPermissions needed for User ID: {notification.User.Id}");
            }

        }

        private (string TypeName, string Action) ParseUrlToEntityAndAction(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Logger.LogWarning("URL is null or empty.");
                return (null, null);
            }

            var normalizedUrl = url.TrimStart('/').TrimEnd('/');
            var segments = normalizedUrl.Split('/');

            if (segments.Length < 3)
            {
                Logger.LogWarning($"URL '{url}' has less than 3 segments. Cannot parse.");
                return (null, null);
            }

            var area = segments[0];
            var controllerName = segments[1];
            var actionName = segments[2];

            var cleanActionName = actionName.Split('{')[0].TrimEnd('}').Trim();

            // Определяем TypeName
            var typeName = $"Masofa.Common.Models.{controllerName}";

            // Определяем BaseActionType
            string baseAction = null;

            // Сопоставление actionName -> BaseActionType
            if (cleanActionName.StartsWith("Get") || cleanActionName.Contains("Read"))
            {
                baseAction = nameof(BaseActionType.Read);
            }
            else if (cleanActionName.StartsWith("Create") || cleanActionName.StartsWith("Post"))
            {
                baseAction = nameof(BaseActionType.Create);
            }
            else if (cleanActionName.StartsWith("Update") || cleanActionName.StartsWith("Put") || cleanActionName.StartsWith("Patch"))
            {
                baseAction = nameof(BaseActionType.Update);
            }
            else if (cleanActionName.StartsWith("Delete") || cleanActionName.StartsWith("Remove"))
            {
                baseAction = nameof(BaseActionType.Delete);
            }
            else
            {
                baseAction = cleanActionName;
            }

            Logger.LogDebug($"Parsed URL '{url}' to Area: '{area}', Controller: '{controllerName}', Action: '{cleanActionName}' -> TypeName: '{typeName}', BaseAction: '{baseAction}'");

            return (typeName, baseAction);
        }
    }
}
