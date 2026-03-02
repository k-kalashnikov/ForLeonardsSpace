using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.WeatherReport
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "Era5WeatherReportsCalculationJob", "Era")]
    public class Era5WeatherReportsCalculationJob : BaseJob<Era5WeatherReportsCalculationJobResult>, IJob
    {
        private MasofaEraDbContext EraDbContext { get; set; }
        private IMediator Mediator { get; set; }

        public Era5WeatherReportsCalculationJob(ILogger<Era5WeatherReportsCalculationJob> logger, IBusinessLogicLogger businessLogicLogger, IMediator mediator, IConfiguration configuration, MasofaEraDbContext eraDbContext, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            EraDbContext = eraDbContext;
            Mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var eraWeatherData = await EraDbContext.EraWeatherData
                .Where(d => d.OriginalDateTimeUtc != null &&
                            DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value) >= DateOnly.FromDateTime(DateTime.UtcNow))
                .GroupBy(d => d.EraWeatherStationId)
                .ToDictionaryAsync(d => d.Key, d => d.ToList());

            var tasks = eraWeatherData.Select(async data =>
            {
                try
                {
                    //await Mediator.Send(new Era5HourWeatherReportUpsertCommand()
                    //{
                    //    WeatherStationId = data.Key,
                    //    EraWeatherData = data.Value
                    //});

                    //await Mediator.Send(new Era5HourWeatherForecastUpsertCommand()
                    //{
                    //    WeatherStationId = data.Key,
                    //    EraWeatherData = data.Value
                    //});

                    await Mediator.Send(new Era5DayWeatherReportUpsertCommand()
                    {
                        WeatherStationId = data.Key,
                        EraWeatherData = data.Value
                    });

                    await Mediator.Send(new Era5DayWeatherForecastUpsertCommand()
                    {
                        WeatherStationId = data.Key,
                        EraWeatherData = data.Value
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            foreach (var task in tasks)
            {
                await task;
            }

            var today = await Mediator.Send(new Era5WeekWeatherReportCommand());

            var year = await Mediator.Send(new Era5MonthWeatherReportCommand()
            {
                Today = today
            });

            await Mediator.Send(new Era5YearWeatherReportCommand()
            {
                Year = year
            });
        }
    }

    public class Era5WeatherReportsCalculationJobResult : BaseJobResult
    {
        public Dictionary<string, List<string>> ReportsCreated { get; set; } = new Dictionary<string, List<string>>();
    }
}