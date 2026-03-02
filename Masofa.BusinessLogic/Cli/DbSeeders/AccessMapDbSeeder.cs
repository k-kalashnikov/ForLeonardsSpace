using DnsClient.Internal;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Cli.DbSeeders
{
    public class AccessMapDbSeeder
    {
        private readonly MasofaCommonDbContext MasofaCommonDbContext;
        private readonly RoleManager<Role> RoleManager;
        private ILogger<AccessMapDbSeeder> _logger {  get; set; }

        public AccessMapDbSeeder(MasofaCommonDbContext context, RoleManager<Role> roleManager, ILogger<AccessMapDbSeeder> logger)
        {
            MasofaCommonDbContext = context;
            RoleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting AccessMapItem seeding/updating...");

            var webAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Masofa.Web.Monolith");

            if(webAssembly == null)
            {
                _logger.LogWarning("Assembly 'Masofa.Web.Monolith' not found.");
                return;
            }

            var controllerTypes = webAssembly.GetTypes()
                .Where(m => (!string.IsNullOrEmpty(m.Namespace)) && (m.Namespace.Contains("Masofa.Web.Monolith.Controllers")))
                .Where(m => !m.Name.Contains("Base") && m.Name.EndsWith("Controller"))
                .ToList();

            var allRoles = await RoleManager.Roles.ToListAsync();
            var allRoleNames = allRoles.Select(r => r.Name).ToHashSet();

            var targetAccessMapItems = new Dictionary<string, AccessMapItem>(); // Ключ - URL

            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType.GetRuntimeMethods()
                    .Where(m => m.IsPublic)
                    .Where(m =>
                    {
                        return m.GetCustomAttribute<HttpGetAttribute>() != null ||
                               m.GetCustomAttribute<HttpPostAttribute>() != null ||
                               m.GetCustomAttribute<HttpPutAttribute>() != null ||
                               m.GetCustomAttribute<HttpDeleteAttribute>() != null;
                    });

                foreach (var method in methods)
                {
                    var url = $"/{controllerType.Namespace.Replace("Masofa.Web.Monolith.Controllers.", "")}/{controllerType.Name.Replace("Controller", "")}/{method.Name}";

                    var rolesDict = new Dictionary<string, bool>();
                    foreach (var roleName in allRoleNames)
                    {
                        rolesDict[roleName] = false;
                    }

                    foreach (var role in allRoles)
                    {
                        bool hasAccess = CheckAccess(role.Name, method, controllerType);
                        rolesDict[role.Name] = hasAccess;
                    }

                    targetAccessMapItems[url] = new AccessMapItem
                    {
                        Url = url,
                        Roles = rolesDict 
                    };
                }
            }

            var existingAccessMapItems = await MasofaCommonDbContext.AccessMapItems.ToListAsync();
            var existingUrls = existingAccessMapItems.ToDictionary(x => x.Url, x => x);

            var itemsToAdd = new List<AccessMapItem>();
            var itemsToUpdate = new List<AccessMapItem>();

            foreach (var targetItem in targetAccessMapItems.Values)
            {
                if (existingUrls.TryGetValue(targetItem.Url, out var existingItem))
                {
                    bool needsUpdate = false;

                    if (existingItem.Roles == null)
                    {
                        existingItem.Roles = new Dictionary<string, bool>();
                        needsUpdate = true;
                    }

                    foreach (var newRole in targetItem.Roles)
                    {
                        if (!existingItem.Roles.ContainsKey(newRole.Key))
                        {
                            existingItem.Roles[newRole.Key] = newRole.Value;
                            needsUpdate = true;
                        }
                        else if (existingItem.Roles[newRole.Key] != newRole.Value)
                        {
                            existingItem.Roles[newRole.Key] = newRole.Value;
                            needsUpdate = true;
                        }
                    }

                    if (needsUpdate)
                    {
                        itemsToUpdate.Add(existingItem);
                    }
                }
                else
                {
                    itemsToAdd.Add(targetItem);
                }
            }

            if (itemsToAdd.Any())
            {
                MasofaCommonDbContext.AccessMapItems.AddRange(itemsToAdd);
                _logger.LogInformation($"Adding {itemsToAdd.Count} new AccessMapItems.");
            }

            if (itemsToUpdate.Any())
            {
                foreach (var item in itemsToUpdate)
                {
                    MasofaCommonDbContext.Entry(item).State = EntityState.Modified;
                }
                _logger.LogInformation($"Updating {itemsToUpdate.Count} existing AccessMapItems.");
            }

            if (itemsToAdd.Any() || itemsToUpdate.Any())
            {
                await MasofaCommonDbContext.SaveChangesAsync();
                _logger.LogInformation("AccessMapItem seeding/updating completed.");
            }
            else
            {
                _logger.LogInformation("No AccessMapItems needed to be added or updated.");
            }
        }

        private bool CheckAccess(string roleName, MethodInfo method, Type controllerType)
        {
            try
            {
                var controllerAuthAttrs = controllerType.GetCustomAttributes<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();
                var methodAuthAttrs = method.GetCustomAttributes<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();

                var authAttrs = methodAuthAttrs.Any() ? methodAuthAttrs : controllerAuthAttrs;

                if (!authAttrs.Any())
                {
                    return true;
                }

                foreach (var authAttr in authAttrs)
                {
                    if (string.IsNullOrEmpty(authAttr.Roles))
                    {
                        return true;
                    }

                    var allowedRoles = authAttr.Roles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(r => r.Trim())
                                                     .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    if (allowedRoles.Contains(roleName))
                    {
                        return true;
                    }
                }

                return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking access for {roleName} on {controllerType.Name}.{method.Name}: {ex.Message}");
                return true;
            }
        }
    }
}
