using Masofa.Client.EarthExplorer;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Cli.DevopsUtil.Commands.Satellite
{
    [BaseCommand("Landsat Search Product", "Поиск продуктов Landsat за последние 5 дней")]
    public class LandsatSearchProductCommand : IBaseCommand
    {
        private ILogger<LandsatSearchProductCommand> Logger { get; set; }
        private LandsatApiUnitOfWork UnitOfWork { get; set; }
        private MasofaLandsatDbContext LandsatDbContext { get; set; }
        private LandsatServiceOptions Options { get; set; }

        public LandsatSearchProductCommand(
            ILogger<LandsatSearchProductCommand> logger,
            LandsatApiUnitOfWork unitOfWork,
            MasofaLandsatDbContext landsatDbContext,
            MasofaCommonDbContext commonDbContext,
            IOptions<LandsatServiceOptions> options)
        {
            Logger = logger;
            UnitOfWork = unitOfWork;
            LandsatDbContext = landsatDbContext;
            Options = options.Value;
        }

        public async Task Execute()
        {
            Logger.LogInformation("Start LandsatSearchProductJob");

            if (!UnitOfWork.IsAuthed)
            {
                await UnitOfWork.LoginAsync(Options);
            }

            var productIds = await UnitOfWork.ProductRepository
                .SearchProductsAsync(Options, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow);

            var existingIds = await LandsatDbContext.Set<LandsatProductQueue>()
                .Select(x => x.ProductId)
                .ToListAsync();

            var newIds = productIds.Except(existingIds).ToList();

            foreach (var id in newIds)
            {
                var queueItem = new LandsatProductQueue
                {
                    Id = Guid.NewGuid(),
                    ProductId = id,
                    Status = ProductQueueStatusType.New,
                    CreateAt = DateTime.UtcNow
                };
                await LandsatDbContext.Set<LandsatProductQueue>().AddAsync(queueItem);
            }

            await LandsatDbContext.SaveChangesAsync();

            Logger.LogInformation("End LandsatSearchProductJob");
        }
        public void Dispose()
        {
        }

        public Task Execute(string[] args)
        {
            return Execute();
        }
    }
}
