using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Cli.DbSeeders
{
    public class LockPermissionDbSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LockPermissionDbSeeder> _logger;
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private UserManager<User> UserManager { get ; set; }

        public LockPermissionDbSeeder(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILogger<LockPermissionDbSeeder> logger,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            UserManager<User> userManager)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
            CommonDbContext = commonDbContext;
            IdentityDbContext = identityDbContext;
            UserManager = userManager;
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting LockPermission seeding/updating for existing users...");

            var allAccessMapItems = await CommonDbContext.AccessMapItems.ToListAsync();

            var allPossibleEntitiesAndActions = new HashSet<(string TypeName, string Action)>();

            foreach (var accessMapItem in allAccessMapItems)
            {
                var (typeName, action) = ParseUrlToEntityAndAction(accessMapItem.Url, _logger);
                if (typeName != null && action != null)
                {
                    allPossibleEntitiesAndActions.Add((typeName, action));
                }
            }

            if (!allPossibleEntitiesAndActions.Any())
            {
                _logger.LogWarning("No AccessMapItems found or no valid TypeName/Action pairs parsed. Skipping LockPermission seeding.");
                return;
            }

            _logger.LogInformation($"Found {allPossibleEntitiesAndActions.Count} possible entity-action combinations from AccessMapItems.");

            var allUsers = await UserManager.Users.ToListAsync();

            _logger.LogInformation($"Processing {allUsers.Count} users...");

            foreach (var user in allUsers)
            {
                _logger.LogDebug($"Processing user {user.Id}...");

                var userRoles = await UserManager.GetRolesAsync(user);
                if (!userRoles.Any())
                {
                    _logger.LogDebug($"User {user.Id} has no roles. Skipping LockPermission generation.");
                    continue;
                }

                var allowedEntitiesAndActions = new HashSet<(string TypeName, string Action)>();

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
                            _logger.LogError(ex, $"Failed to deserialize RolesJson for AccessMapItem URL: {accessMapItem.Url} for user {user.Id}. Skipping this AccessMapItem.");
                            continue;
                        }

                        if (rolesDict != null && rolesDict.TryGetValue(roleName, out bool hasAccess) && hasAccess)
                        {
                            var (typeName, action) = ParseUrlToEntityAndAction(accessMapItem.Url, _logger);
                            if (typeName != null && action != null)
                            {
                                allowedEntitiesAndActions.Add((typeName, action));
                            }
                        }
                    }
                }

                // сущности/действия, к которым доступа НЕТ (разность множеств)
                var deniedEntitiesAndActions = allPossibleEntitiesAndActions.Except(allowedEntitiesAndActions);

                var existingLocks = await IdentityDbContext.LockPermissions
                    .Where(lp => lp.UserId == user.Id)
                    .ToListAsync();

                // Создаем словарь для быстрого поиска существующих блокировок
                var existingLockSet = existingLocks.ToLookup(lp => (lp.EntityTypeName, lp.EntityAction));

                var lockPermissionsToCreate = new List<LockPermission>();

                foreach (var (typeName, action) in deniedEntitiesAndActions)
                {
                    if (!existingLockSet.Contains((typeName, action)))
                    {
                        var lockPermission = new LockPermission
                        {
                            UserId = user.Id,
                            EntityTypeName = typeName,
                            EntityAction = action,
                            LockPermissionType = LockPermissionType.Action, // Или LockPermissionType.Entity
                        };
                        lockPermissionsToCreate.Add(lockPermission);
                    }
                    // else: LockPermission уже существует, ничего не делаем
                }

                if (lockPermissionsToCreate.Any())
                {
                    IdentityDbContext.LockPermissions.AddRange(lockPermissionsToCreate);
                    _logger.LogDebug($"Prepared {lockPermissionsToCreate.Count} new LockPermissions for user {user.Id}.");
                }
                else
                {
                    _logger.LogDebug($"No new LockPermissions needed for user {user.Id}.");
                }
            }

            var entriesToSave = IdentityDbContext.ChangeTracker.Entries<LockPermission>()
                .Where(e => e.State == EntityState.Added)
                .ToList();

            if (entriesToSave.Any())
            {
                await IdentityDbContext.SaveChangesAsync();
                _logger.LogInformation($"Saved {entriesToSave.Count} new LockPermissions to the database.");
            }
            else
            {
                _logger.LogInformation("No new LockPermissions were generated during seeding.");
            }

            _logger.LogInformation("LockPermission seeding/updating completed.");
        }

        private static (string TypeName, string Action) ParseUrlToEntityAndAction(string url, ILogger logger)
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
