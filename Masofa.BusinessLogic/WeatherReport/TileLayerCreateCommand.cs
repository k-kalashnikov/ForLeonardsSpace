using Masofa.Common.Models;
using Masofa.Common.Models.Tiles;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.WeatherReport
{
    public class TileLayerCreateCommand : IRequest
    {
        public required string Indicator { get; set; }
        public required string LayerName { get; set; }
        public required string RelativePath { get; set; }

        public class TileLayerCreateCommandHandler : IRequestHandler<TileLayerCreateCommand>
        {
            private ILogger Logger { get; set; }
            private MasofaTileDbContext TileDbContext { get; set; }
            private readonly string _serverPath;
            public TileLayerCreateCommandHandler(MasofaTileDbContext tileDbContext, ILogger<TileLayerCreateCommandHandler> logger, IConfiguration configuration)
            {
                TileDbContext = tileDbContext;
                Logger = logger;
                _serverPath = configuration.GetValue<string>("GeoServerOptions:GeoserverTilesDir") ?? "/deploy/prod/data-geoserver-prod/";
            }


            public async Task Handle(TileLayerCreateCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var config = TileLayerConfig.Layers.FirstOrDefault(x => x.ApiName == request.Indicator)
                        ?? throw new NullReferenceException($"There is no localization for {request.Indicator}");

                    var path = Path.Combine(_serverPath, request.RelativePath);

                    var existing = await TileDbContext.TileLayers.FirstOrDefaultAsync(t => t.GeoServerRelationPath == path || t.GeoServerName == request.LayerName, cancellationToken);
                    if (existing != null)
                    {
                        existing.Names = new LocalizationString
                        {
                            ["en-US"] = config.EngName,
                            ["ru-RU"] = config.RuName,
                            ["uz-Latn-UZ"] = config.UzLatName
                        };
                        existing.GeoServerName = request.LayerName;
                        existing.GeoServerRelationPath = path;
                        existing.LastUpdateAt = DateTime.UtcNow;

                        TileDbContext.Update(existing);
                    }
                    else
                    {
                        var tileLayer = new TileLayer
                        {
                            Names = new LocalizationString
                            {
                                ["en-US"] = config.EngName,
                                ["ru-RU"] = config.RuName,
                                ["uz-Latn-UZ"] = config.UzLatName
                            },
                            GeoServerName = request.LayerName,
                            GeoServerRelationPath = path,
                            CreateAt = DateTime.UtcNow,
                            LastUpdateAt = DateTime.UtcNow
                        };

                        TileDbContext.TileLayers.Add(tileLayer);
                    }

                    await TileDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Something went wrong in generation of TileLayerConfig: {ex.Message}");
                }
            }
        }
    }
}
