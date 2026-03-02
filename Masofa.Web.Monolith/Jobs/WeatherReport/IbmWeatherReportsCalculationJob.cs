using Masofa.BusinessLogic.IBMWeather;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.WeatherReport
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "IbmWeatherReportsCalculationJob", "IBMWeather")]
    public class IbmWeatherReportsCalculationJob : BaseJob<IbmWeatherReportsCalculationJobResult>, IJob
    {
        private MasofaIBMWeatherDbContext IBMWeatherDbContext { get; set; }

        public IbmWeatherReportsCalculationJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<IbmWeatherReportsCalculationJob> logger, MasofaIBMWeatherDbContext iBMWeatherDbContext, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            IBMWeatherDbContext = iBMWeatherDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ibmWeatherData = await IBMWeatherDbContext.IBMWeatherData
                .Where(d => DateOnly.FromDateTime(d.ValidTimeUtc) >= DateOnly.FromDateTime(DateTime.UtcNow))
                .GroupBy(d => d.IBMMeteoStationId)
                .ToDictionaryAsync(d => d.Key, d => d.ToList());

            var tasks = ibmWeatherData.Select(async data =>
            {
                try
                {
                    await Mediator.Send(new IbmDayWeatherForecastUpsertCommand()
                    {
                        WeatherStationId = data.Key,
                        IbmWeatherData = data.Value
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
        }
    }

    public class IbmWeatherReportsCalculationJobResult : BaseJobResult
    {
        public Dictionary<string, List<string>> ReportsCreated { get; set; } = new Dictionary<string, List<string>>();
    }
}
