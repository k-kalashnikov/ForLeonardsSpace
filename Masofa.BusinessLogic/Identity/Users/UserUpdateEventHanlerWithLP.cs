using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Identity.Users
{
    public class UserUpdateEventHanlerWithLP : INotificationHandler<UserUpdateEvent>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private RoleManager<Role> RoleManager { get; set; }
        private UserManager<User> UserManager { get; set; }
        private ILogger<UserUpdateEventHanlerWithLP> Logger { get; set; }

        public UserUpdateEventHanlerWithLP(IBusinessLogicLogger businessLogicLogger, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext, RoleManager<Role> roleManager, UserManager<User> userManager, ILogger<UserUpdateEventHanlerWithLP> logger)
        {
            BusinessLogicLogger = businessLogicLogger;
            CommonDbContext = commonDbContext;
            IdentityDbContext = identityDbContext;
            RoleManager = roleManager;
            UserManager = userManager;
            Logger = logger;
        }

        public async Task Handle(UserUpdateEvent notification, CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Handling UserUpdateEvent for User ID: {notification.UserId}");

            var oldUser = notification.OldModel;
            var currentUser = notification.CurrentModel;

            var oldUserRoles = new List<string>();
            var currentUserRoles = new List<string>();

            try
            {
                oldUserRoles = (await UserManager.GetRolesAsync(oldUser)).ToList();
                currentUserRoles = (await UserManager.GetRolesAsync(currentUser)).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to get roles for old or current user during update event for User ID: {notification.UserId}. Proceeding with update.");
            }

            var oldRolesSet = new HashSet<string>(oldUserRoles, StringComparer.OrdinalIgnoreCase);
            var currentUserRolesSet = new HashSet<string>(currentUserRoles, StringComparer.OrdinalIgnoreCase);

            if (oldRolesSet.SetEquals(currentUserRolesSet))
            {
                Logger.LogDebug($"User roles have not changed for User ID: {notification.UserId}. Skipping LockPermission update.");
                return;
            }

            Logger.LogInformation($"User roles changed for User ID: {notification.UserId}. Old roles: [{string.Join(", ", oldUserRoles)}], New roles: [{string.Join(", ", currentUserRoles)}]");

            var existingLocksToRemove = IdentityDbContext.LockPermissions.Where(lp => lp.UserId == notification.UserId);
            if (existingLocksToRemove.Any())
            {
                IdentityDbContext.LockPermissions.RemoveRange(existingLocksToRemove);
                Logger.LogDebug($"Marked {existingLocksToRemove.Count()} existing LockPermissions for deletion for User ID: {notification.UserId}");
            }

            var allAccessMapItems = await CommonDbContext.AccessMapItems.ToListAsync(cancellationToken);

            var allowedEntitiesAndActions = new HashSet<(string TypeName, string Action)>();

            foreach (var roleName in currentUserRoles)
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
                        var (typeName, action) = ParseUrlToEntityAndAction(accessMapItem.Url, Logger);
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
                var (typeName, action) = ParseUrlToEntityAndAction(accessMapItem.Url, Logger);
                if (typeName != null && action != null)
                {
                    allPossibleEntitiesAndActions.Add((typeName, action));
                }
            }

            // Нахожу сущности/действия, к которым доступа НЕТ (разность множеств)
            var deniedEntitiesAndActions = allPossibleEntitiesAndActions.Except(allowedEntitiesAndActions);

            var lockPermissionsToCreate = new List<LockPermission>();

            foreach (var (typeName, action) in deniedEntitiesAndActions)
            {
                var lockPermission = new LockPermission
                {
                    UserId = notification.UserId,
                    EntityTypeName = typeName,
                    EntityAction = action,
                    LockPermissionType = LockPermissionType.Action,
                };
                lockPermissionsToCreate.Add(lockPermission);
            }

            if (lockPermissionsToCreate.Any())
            {
                IdentityDbContext.LockPermissions.AddRange(lockPermissionsToCreate);
                Logger.LogDebug($"Prepared {lockPermissionsToCreate.Count} new LockPermissions for User ID: {notification.UserId}");
            }

            await IdentityDbContext.SaveChangesAsync(cancellationToken);
            Logger.LogInformation($"Updated LockPermissions for User ID: {notification.UserId}. Removed {existingLocksToRemove.Count()}, Added {lockPermissionsToCreate.Count()}.");
        }

        private (string typeName, string action) ParseUrlToEntityAndAction(string url, ILogger<UserUpdateEventHanlerWithLP> logger)
        {
            if (string.IsNullOrEmpty(url))
            {
                logger?.LogWarning("URL is null or empty.");
                return (null, null);
            }

            var normalizedUrl = url.TrimStart('/').TrimEnd('/');
            var segments = normalizedUrl.Split('/');

            if (segments.Length < 3)
            {
                logger?.LogWarning($"URL '{url}' has less than 3 segments. Cannot parse.");
                return (null, null);
            }

            var area = segments[0];
            var controllerName = segments[1];
            var actionName = segments[2];

            var cleanActionName = actionName.Split('{')[0].TrimEnd('}').Trim();

            var typeName = $"Masofa.Common.Models.{controllerName}";

            string baseAction = null;
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

            logger?.LogDebug($"Parsed URL '{url}' to TypeName: '{typeName}', Action: '{baseAction}'");

            return (typeName, baseAction);
        }
    }
}
