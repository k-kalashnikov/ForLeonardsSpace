//using Masofa.Client.Copernicus;
//using Masofa.Common.Attributes;
//using Masofa.Common.Models;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.SystemCrical;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace Masofa.Cli.DevopsUtil.Commands.Satellite
//{
//    public class Sentinel2SearchProductByDateCommandParameters
//    {
//        [TaskParameter("Дата начала поиска", true, "2024-01-01")]
//        public DateTime StartDate { get; set; }
        
//        [TaskParameter("Дата окончания поиска", true, "2024-01-31")]
//        public DateTime EndDate { get; set; }

//        public static Sentinel2SearchProductByDateCommandParameters Parse(string[] args)
//        {
//            if (args.Length < 2 || !DateTime.TryParse(args[0], out var startDate) || !DateTime.TryParse(args[1], out var endDate))
//                throw new ArgumentException("Неверные параметры. Используйте: <startDate> <endDate> (формат: yyyy-MM-dd)");

//            return new Sentinel2SearchProductByDateCommandParameters { StartDate = startDate, EndDate = endDate };
//        }

//        public static Sentinel2SearchProductByDateCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Enter start date (yyyy-MM-dd):");
//            var startDateStr = Console.ReadLine();
//            Console.WriteLine("Enter end date (yyyy-MM-dd):");
//            var endDateStr = Console.ReadLine();

//            if (!DateTime.TryParse(startDateStr, out var startDate) || !DateTime.TryParse(endDateStr, out var endDate))
//                throw new ArgumentException("Invalid date format.");

//            return new Sentinel2SearchProductByDateCommandParameters { StartDate = startDate, EndDate = endDate };
//        }
//    }

//    [BaseCommand("Sentinel2 Search Product By Date", "Поиск продуктов Sentinel2 по дате", typeof(Sentinel2SearchProductByDateCommandParameters))]
//    public class Sentinel2SearchProductByDateCommand : IBaseCommand
//    {
//        ILogger<Sentinel2SearchProductByDateCommand> Logger { get; }
//        private SentinelServiceOptions SentinelServiceOptions { get; }
//        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
//        private MasofaSentinelDbContext SentinelDbContext { get; }

//        public Sentinel2SearchProductByDateCommand(
//            ILogger<Sentinel2SearchProductByDateCommand> logger,
//            CopernicusApiUnitOfWork copernicusApiUnitOfWork,
//            MasofaCommonDbContext commonDbContext,
//            MasofaSentinelDbContext sentinelDbContext,
//            IOptions<SentinelServiceOptions> options)
//        {
//            SentinelServiceOptions = options.Value;
//            Logger = logger;
//            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
//            SentinelDbContext = sentinelDbContext;
//        }

//        public async Task Execute()
//        {
//            var parameters = Sentinel2SearchProductByDateCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = Sentinel2SearchProductByDateCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(Sentinel2SearchProductByDateCommandParameters parameters)
//        {
//            Logger.LogInformation($"Start Sentinel2SearchProductByDateCommand");

//            if (!CopernicusApiUnitOfWork.IsAuthed)
//            {
//                await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);
//            }

//            for (var currentDate = parameters.StartDate; currentDate <= parameters.EndDate; currentDate = currentDate.AddDays(1))
//            {
//                try
//                {
//                    Console.WriteLine($"Try search product from {currentDate.ToString("yyyy-MM-dd")} to {currentDate.AddDays(1).ToString("yyyy-MM-dd")}");
//                    var productIds = await CopernicusApiUnitOfWork.ProductRepository.SearchProductAsync(SentinelServiceOptions, currentDate, currentDate.AddDays(1));

//                    if (productIds == null || !productIds.Any())
//                    {
//                        Logger.LogInformation("No suitable products found for AOI.");
//                        return;
//                    }

//                    var allQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>().ToListAsync();

//                    var existsIds = allQueue
//                        .Where(m => Guid.TryParse(m.ProductId, out var id) && productIds.Contains(id))
//                        .Select(m => m.ProductId)
//                        .ToList();

//                    foreach (var item in productIds.Where(m => !existsIds.Contains(m.ToString())))
//                    {
//                        var tempPQ = new Sentinel2ProductQueue()
//                        {
//                            CreateAt = DateTime.UtcNow,
//                            Id = Guid.NewGuid(),
//                            ProductId = item.ToString(),
//                            Status = ProductQueueStatusType.New,
//                            CreateUser = Guid.Empty
//                        };

//                        await SentinelDbContext.Set<Sentinel2ProductQueue>().AddAsync(tempPQ);
//                    }

//                    await SentinelDbContext.SaveChangesAsync();
//                    await Task.Delay(5000);
//                    Console.WriteLine($"Complite search product from {currentDate.ToString("yyyy-MM-dd")} to {currentDate.AddDays(1).ToString("yyyy-MM-dd")}");

//                }
//                catch (Exception ex) 
//                {
//                    Console.WriteLine($"Exception with search product from {currentDate.ToString("yyyy-MM-dd")} to {currentDate.AddDays(1).ToString("yyyy-MM-dd")} with exception {ex.Message}");
//                }
//            }

//            Logger.LogInformation($"End Sentinel2SearchProductByDateCommand");
//        }
//        public void Dispose()
//        {
//        }


//    }
//}