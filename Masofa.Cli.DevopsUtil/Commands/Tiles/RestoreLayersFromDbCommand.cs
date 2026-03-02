using Masofa.BusinessLogic.Services;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masofa.Cli.DevopsUtil.Commands.Tiles
{
    [BaseCommand("Restore Layers from DB", "Restore Layers from DB")]
    public class RestoreLayersFromDbCommand : IBaseCommand
    {
        private ILogger<RestoreLayersFromDbCommand> Logger { get; set; }
        private GeoServerService GeoServerService { get; set; }
        private MasofaTileDbContext TileDbContext { get; set; }

        private readonly string _workspace;
        private readonly string _serverPath = "/deploy/prod/data-geoserver-prod/";

        public RestoreLayersFromDbCommand(GeoServerService geoServerService, ILogger<RestoreLayersFromDbCommand> logger, IConfiguration configuration, MasofaTileDbContext tileDbContext)
        {
            GeoServerService = geoServerService;
            Logger = logger;
            _workspace = configuration.GetValue<string>("GeoServerOptions:Workspace") ?? "osm";
            TileDbContext = tileDbContext;
        }

        public void Dispose()
        {
            Console.WriteLine("\nRestoreLayersFromDbCommand END");
        }

        public async Task Execute()
        {
            try
            {
                Console.WriteLine("RestoreLayersFromDbCommand START\n");
                Console.WriteLine($"_workspace: {_workspace}");
                await GeoServerService.CreateWorkspaceAsync(_workspace);

                var tileLayers = await TileDbContext.TileLayers
                    .AsNoTracking()
                    .OrderByDescending(l => l.CreateAt)
                    .ToListAsync();

                foreach (var layer in tileLayers)
                {
                    var storeName = layer.GeoServerName;
                    var path = layer.GeoServerRelationPath.Replace(_serverPath, "");

                    var storeRes = await GeoServerService.RecreateImageMosaicStoreAsync(storeName, path);
                    var layerRes = false;
                    if (storeRes)
                    {
                        layerRes = await GeoServerService.PublishCoverageAsync(storeName, storeName);
                    }
                    Console.WriteLine($"{storeName} - {path} - {storeRes} - {layerRes}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message: {ex.Message}");
                Logger.LogCritical(ex.Message);
            }
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
