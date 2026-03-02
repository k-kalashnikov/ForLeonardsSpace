using DnsClient.Internal;
using Masofa.BusinessLogic.Services;
using Masofa.Common;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Models.Tiles;
using Masofa.DataAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Index
{
    public class BaseDeleteTileHandler : INotificationHandler<BaseDeleteEvent<TileLayer, MasofaTileDbContext>>
    {
        private readonly string _archiveRootPath;
        private MasofaTileDbContext MasofaTileDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private GeoServerService GeoServerService { get; set; }
        private GeoServerOptions GeoServerOptions { get; set; }
        private ILogger<BaseDeleteTileHandler> Logger { get; set; }
        public BaseDeleteTileHandler(MasofaTileDbContext masofaTileDbContext, GeoServerService geoServerService, ILogger<BaseDeleteTileHandler> logger, IOptions<GeoServerOptions> options, MasofaIdentityDbContext identityDbContext)
        {
            MasofaTileDbContext = masofaTileDbContext;
            GeoServerService = geoServerService;
            Logger = logger;
            GeoServerOptions = options.Value;
            _archiveRootPath = Path.Combine(GeoServerOptions.Volume, "Archives");
            Directory.CreateDirectory(_archiveRootPath);
            IdentityDbContext = identityDbContext;
        }

        public async Task Handle(BaseDeleteEvent<TileLayer, MasofaTileDbContext> notification, CancellationToken cancellationToken)
        {
            var tileLayer = notification.Model;

            if (tileLayer == null)
            {
                Logger.LogWarning("A delete event was received with a null TileLayer object.");
                return;
            }

            var storeName = tileLayer.GeoServerName;
            if (string.IsNullOrWhiteSpace(storeName))
            {
                Logger.LogWarning("TileLayer с ID={Id} does not have a GeoServerName - skip deletion from GeoServer.", tileLayer.Id);
                return;
            }

            var deletedFromGeoServer = await GeoServerService.DeleteCoverageStoreAsync(GeoServerOptions.Workspace, storeName);

            if (!deletedFromGeoServer)
            {
                Logger.LogWarning("Failed to remove coverage store '{StoreName}' from GeoServer.", storeName);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(tileLayer.GeoServerRelationPath))
                {
                    var sourcePath = Path.Combine(GeoServerOptions.Volume, tileLayer.GeoServerRelationPath);

                    if (Directory.Exists(sourcePath))
                    {
                        var user = IdentityDbContext.Users.FirstOrDefault(x => x.Id == notification.UserId);
                        var deletedBy = "System";
                        if(user != null)
                        {
                            deletedBy = user.UserName;
                        }

                        await ArchiveAndDeleteDirectoryAsync(sourcePath, tileLayer.GeoServerName, deletedBy, cancellationToken);
                    }
                    else
                    {
                        Logger.LogDebug("Data folder not found: {FullPath}", sourcePath);
                    }
                }
                else
                {
                    Logger.LogWarning("TileLayer с ID={Id} dont have GeoServerRelationPath — skip deleting folder.", tileLayer.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting data folder for TileLayer ID={Id}, path={Path}",
                    tileLayer.Id,
                    Path.Combine(GeoServerOptions.Volume, tileLayer.GeoServerRelationPath));
            }

        }

        private async Task ArchiveAndDeleteDirectoryAsync(string sourcePath, string layerName, string deletedBy, CancellationToken ct)
        {
            if (!Directory.Exists(sourcePath))
            {
                Logger.LogDebug("Source folder does not exist: {SourcePath}", sourcePath);
                return;
            }

            var now = DateTime.UtcNow;
            var archiveFileName = $"Deleted_{layerName}_{now:yyyyMMdd_HHmmss}_{deletedBy}.zip";
            var archiveFullPath = Path.Combine(_archiveRootPath, archiveFileName);

            try
            {
                ZipFile.CreateFromDirectory(sourcePath, archiveFullPath, CompressionLevel.Fastest, includeBaseDirectory: true);
                Logger.LogInformation("The folder was successfully archived: {ArchivePath}", archiveFullPath);

                Directory.Delete(sourcePath, recursive: true);
                Logger.LogInformation("Original folder deleted: {SourcePath}", sourcePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error while archiving and deleting folder {SourcePath}", sourcePath);
                throw; 
            }
        }
    }
}
