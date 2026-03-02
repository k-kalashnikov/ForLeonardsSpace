using Masofa.Client.EarthExplorer;
using Masofa.Common.Attributes;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Cli.DevopsUtil.Commands.Satellite
{
    public class LandsatSearchProductByDateCommandParameters
    {
        [TaskParameter("Дата начала поиска", true, "2024-01-01")]
        public DateTime StartDate { get; set; }
        
        [TaskParameter("Дата окончания поиска", true, "2024-01-31")]
        public DateTime EndDate { get; set; }

        public static LandsatSearchProductByDateCommandParameters Parse(string[] args)
        {
            if (args.Length < 2 || !DateTime.TryParse(args[0], out var startDate) || !DateTime.TryParse(args[1], out var endDate))
                throw new ArgumentException("Неверные параметры. Используйте: <startDate> <endDate> (формат: yyyy-MM-dd)");

            return new LandsatSearchProductByDateCommandParameters { StartDate = startDate, EndDate = endDate };
        }

        public static LandsatSearchProductByDateCommandParameters GetFromUser()
        {
            Console.WriteLine("Enter start date (yyyy-MM-dd):");
            var startDateStr = Console.ReadLine();
            Console.WriteLine("Enter end date (yyyy-MM-dd):");
            var endDateStr = Console.ReadLine();

            if (!DateTime.TryParse(startDateStr, out var startDate) || !DateTime.TryParse(endDateStr, out var endDate))
                throw new ArgumentException("Invalid date format.");

            return new LandsatSearchProductByDateCommandParameters { StartDate = startDate, EndDate = endDate };
        }
    }

    [BaseCommand("Landsat Search Product By Date", "Поиск продуктов Landsat по дате", typeof(LandsatSearchProductByDateCommandParameters))]
    public class LandsatSearchProductByDateCommand : IBaseCommand
    {
        private ILogger<LandsatSearchProductByDateCommand> Logger { get; set; }
        private LandsatApiUnitOfWork UnitOfWork { get; set; }
        private MasofaLandsatDbContext LandsatDbContext { get; set; }
        private LandsatServiceOptions Options { get; set; }

        public LandsatSearchProductByDateCommand(
            ILogger<LandsatSearchProductByDateCommand> logger,
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
            var parameters = LandsatSearchProductByDateCommandParameters.GetFromUser();
            await ExecuteCore(parameters);
        }

        public async Task Execute(string[] args)
        {
            var parameters = LandsatSearchProductByDateCommandParameters.Parse(args);
            await ExecuteCore(parameters);
        }

        private async Task ExecuteCore(LandsatSearchProductByDateCommandParameters parameters)
        {
            Logger.LogInformation("Start LandsatSearchProductByDateCommand");

            if (!UnitOfWork.IsAuthed)
            {
                await UnitOfWork.LoginAsync(Options);
            }

            var productIds = await UnitOfWork.ProductRepository
                .SearchProductsAsync(Options, parameters.StartDate, parameters.EndDate);

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

            Logger.LogInformation("End LandsatSearchProductByDateCommand");
        }
        public void Dispose()
        {
        }


    }
} 