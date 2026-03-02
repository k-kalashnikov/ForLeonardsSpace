using Masofa.Client.EarthExplorer;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Cli.DevopsUtil.Commands.Satellite
{
    [BaseCommand("Landsat Metadata Loader", "Загрузка метаданных Landsat из очереди")]
    public class LandsatMetadataLoaderCommand : IBaseCommand
    {
        private ILogger<LandsatMetadataLoaderCommand> Logger { get; set; }
        private LandsatApiUnitOfWork UnitOfWork { get; set; }
        private MasofaLandsatDbContext LandsatDbContext { get; set; }
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private LandsatServiceOptions Options { get; set; } 

        public LandsatMetadataLoaderCommand(
            ILogger<LandsatMetadataLoaderCommand> logger,
            LandsatApiUnitOfWork unitOfWork,
            IOptions<LandsatServiceOptions> options,
            MasofaCommonDbContext commonDbContext,
            MasofaLandsatDbContext landsatDbContext)
        {
            Logger = logger;
            UnitOfWork = unitOfWork;
            Options = options.Value;
            CommonDbContext = commonDbContext;
            LandsatDbContext = landsatDbContext;
        }

        public async Task Execute()
        {
            Logger.LogInformation("Start LandsatMetadataLoaderJob");

            if (!UnitOfWork.IsAuthed)
                await UnitOfWork.LoginAsync(Options);

            var queue = await LandsatDbContext.Set<LandsatProductQueue>()
                .Where(x => x.Status == ProductQueueStatusType.New)
                .Take(100)
                .ToListAsync();

            foreach (var item in queue)
            {
                var product = await CommonDbContext.Set<SatelliteProduct>().FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                if (product == null)
                {
                    product = new SatelliteProduct
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        ProductSourceType = ProductSourceType.Landsat,
                        CreateAt = DateTime.UtcNow,
                        CreateUser = Guid.Empty,
                        MediadataPath = Guid.Empty
                    };
                    await CommonDbContext.Set<SatelliteProduct>().AddAsync(product);
                }

                var metadata = await UnitOfWork.ProductRepository.GetProductMetadataAsync(
                    Options,
                    "landsat_ot_c2_l2",
                    item.ProductId,
                    product.Id.ToString());

                await LandsatDbContext.Set<LandsatProductMetadata>().AddAsync(metadata);

                item.Status = ProductQueueStatusType.MetadataLoaded;
                LandsatDbContext.Set<LandsatProductQueue>().Update(item);

                await CommonDbContext.SaveChangesAsync();
                await LandsatDbContext.SaveChangesAsync();
            }

            Logger.LogInformation("End LandsatMetadataLoaderJob");
        }
        public void Dispose()
        {
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
