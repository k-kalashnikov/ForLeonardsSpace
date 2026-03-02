using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.Ugm;
using Masofa.Client.Ugm;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Ugm;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.Weather
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "UgmWeatherCurrentDataLoaderJob", "Ugm")]
    public class UgmWeatherCurrentDataLoaderJob : BaseJob<UgmWeatherCurrentDataLoaderJobResult>, IJob
    {
        private UgmApiUnitOfWork UgmApiUnitOfWork { get; }
        private MasofaUgmDbContext UgmDbContext { get; }

        public UgmWeatherCurrentDataLoaderJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<UgmWeatherCurrentDataLoaderJob> logger, UgmApiUnitOfWork ugmApiUnitOfWork, MasofaUgmDbContext ugmDbContext, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            UgmApiUnitOfWork = ugmApiUnitOfWork;
            UgmDbContext = ugmDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Start UgmWeatherCurrentDataLoaderJob");

            var current = await UgmApiUnitOfWork.UgmWeatherDataRepository.GetAllCurrentWeatherDataAsync();

            var weatherStations = await UgmDbContext.UgmWeatherStations.ToListAsync();
            var stationsLookup = weatherStations.ToDictionary(s => s.UgmRegionId, s => s);

            foreach (var currentWeather in current ?? [])
            {
                if (currentWeather.RegionId == null)
                {
                    continue;
                }

                if (!stationsLookup.TryGetValue(currentWeather.RegionId.Value, out var station))
                {
                    station = new Common.Models.Ugm.UgmWeatherStation
                    {
                        UgmRegionId = currentWeather.RegionId.Value
                    };

                    await UgmDbContext.UgmWeatherStations.AddAsync(station);
                    stationsLookup[currentWeather.RegionId.Value] = station;
                }

                station.Name = currentWeather.City?.Name;
                station.IsRegionalCenter = currentWeather.City?.IsRegionalCenter;
                station.Latitude = currentWeather.City?.Latitude;
                station.Longitude = currentWeather.City?.Longitude;
                station.Title = currentWeather.City?.Title;

                var timeOfDay = currentWeather.TimeOfDay == "night" ? DayPart.Night : DayPart.Day;

                var newUgmWeatherDataRequest = new UgmWeatherDataUpsertCommand()
                {
                    RegionId = currentWeather.RegionId.Value,
                    Date = DateOnly.FromDateTime(currentWeather.DateTime ?? DateTime.UtcNow),
                    DateTime = currentWeather.DateTime ?? DateTime.UtcNow,
                    DayPart = timeOfDay,
                    AirTMin = currentWeather.AirT,
                    AirTMax = currentWeather.AirT,
                    CloudAmount = currentWeather.CloudAmount,
                    WeatherCode = currentWeather.WeatherCode
                };

                var newId = await Mediator.Send(newUgmWeatherDataRequest);
            }

            await UgmDbContext.SaveChangesAsync();

            Logger.LogInformation("End UgmWeatherCurrentDataLoaderJob");
        }
    }

    public class UgmWeatherCurrentDataLoaderJobResult : BaseJobResult
    {
        public string Message { get; set; }
    }
}
