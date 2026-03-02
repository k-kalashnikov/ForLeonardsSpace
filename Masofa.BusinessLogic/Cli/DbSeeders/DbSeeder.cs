using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Cli.DbSeeders
{
    public class DbSeeder
    {
        private IServiceProvider ServiceProvider { get; set; }
        private IConfiguration Configuration { get; set; }
        private ILogger<AccessMapDbSeeder> Logger { get; set; }

        public DbSeeder(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<AccessMapDbSeeder> logger)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            Logger = logger;
        }

        public async Task SeedAllAsync()
        {
            using var scope = ServiceProvider.CreateScope();

            if (Configuration.GetValue<bool>("DbSeeders:StartIdentity"))
            {
                Logger.LogInformation("Starting IdentityDbSeeder...");
                var identityDbSeeder = ActivatorUtilities.CreateInstance<IdentityDbSeeder>(scope.ServiceProvider);
                await identityDbSeeder.SeedAsync();
                Logger.LogInformation("IdentityDbSeeder completed.");
            }

            if (Configuration.GetValue<bool>("DbSeeders:StartDictionaries"))
            {
                Logger.LogInformation("Starting DictionariesDbSeeder...");
                var dictionariesDbSeeder = ActivatorUtilities.CreateInstance<DictionariesDbSeeder>(scope.ServiceProvider);
                await dictionariesDbSeeder.SeedAsync();
                Logger.LogInformation("DictionariesDbSeeder completed.");
            }

            if (Configuration.GetValue<bool>("DbSeeders:StartCropMonitoring"))
            {
                Logger.LogInformation("Starting FieldAgroOperationDbSeeder...");
                var fieldAgroOperationDbSeeder = ActivatorUtilities.CreateInstance<FieldAgroOperationDbSeeder>(scope.ServiceProvider);
                await fieldAgroOperationDbSeeder.SeedAsync();
                Logger.LogInformation("FieldAgroOperationDbSeeder completed.");
            }

            if (Configuration.GetValue<bool>("DbSeeders:StartAccessMap"))
            {
                Logger.LogInformation("Starting AccessMapDbSeeder...");
                var accessMapDbSeeder = ActivatorUtilities.CreateInstance<AccessMapDbSeeder>(scope.ServiceProvider);
                await accessMapDbSeeder.SeedAsync();
                Logger.LogInformation("AccessMapDbSeeder completed.");
            }

            if (Configuration.GetValue<bool>("DbSeeders:StartLockPermission"))
            {
                Logger.LogInformation("Starting LockPermissionDbSeeder...");
                var accessMapDbSeeder = ActivatorUtilities.CreateInstance<LockPermissionDbSeeder>(scope.ServiceProvider);
                await accessMapDbSeeder.SeedAsync();
                Logger.LogInformation("LockPermissionDbSeeder completed.");
            }

            if (Configuration.GetValue<bool>("MonolithConfiguration:StartJobs"))
            {
                var systemBackgroundTaskDbSeeder = ActivatorUtilities.CreateInstance<SystemBackgroundTaskDbSeeder>(scope.ServiceProvider);
                await systemBackgroundTaskDbSeeder.SeedAsync();
            }
        }
    }
}
