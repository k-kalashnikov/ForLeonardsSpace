using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.Ugm;
using Masofa.Client.Ugm;
using Masofa.Client.Ugm.Models;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Ugm;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.Weather
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "UgmWeatherForecastDataLoaderJob", "Ugm")]
    public class UgmWeatherForecastDataLoaderJob : BaseJob<UgmWeatherForecastDataLoaderJobResult>, IJob
    {
        private UgmApiUnitOfWork UgmApiUnitOfWork { get; }
        private MasofaUgmDbContext UgmDbContext { get; }

        public UgmWeatherForecastDataLoaderJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<UgmWeatherForecastDataLoaderJob> logger, UgmApiUnitOfWork ugmApiUnitOfWork, MasofaUgmDbContext ugmDbContext, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            UgmApiUnitOfWork = ugmApiUnitOfWork;
            UgmDbContext = ugmDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Start UgmWeatherForecastDataLoaderJob");

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
            }

            await UgmDbContext.SaveChangesAsync();

            await Task.Delay(TimeSpan.FromSeconds(30));

            var forecasts = await UgmApiUnitOfWork.UgmWeatherDataRepository.GetAllForecastWeatherDataAsync();

            var needToUpdateStations = false;

            List<UgmForecastItem> ugmForecastItems = [];

            foreach (var daysAndNights in forecasts ?? [])
            {
                ugmForecastItems.AddRange(daysAndNights.Day ?? []);
                ugmForecastItems.AddRange(daysAndNights.Night ?? []);

            }

            foreach (var forecast in ugmForecastItems ?? [])
            {
                var regionId = forecast.RegionId;
                if (regionId == null)
                {
                    continue;
                }

                if (!stationsLookup.TryGetValue(regionId.Value, out var station))
                {
                    station = new Common.Models.Ugm.UgmWeatherStation
                    {
                        UgmRegionId = regionId.Value,
                        Name = forecast.RegionCode,
                        Latitude = forecast.Latitude,
                        Longitude = forecast.Longitude,
                        Title = forecast.RegionName
                    };

                    await UgmDbContext.UgmWeatherStations.AddAsync(station);
                    stationsLookup[regionId.Value] = station;
                    needToUpdateStations = true;
                }
                var timeOfDay = forecast.DayPart == "night" ? DayPart.Night : DayPart.Day;

                var newUgmWeatherDataRequest = new UgmWeatherDataUpsertCommand()
                {
                    RegionId = regionId.Value,
                    Date = forecast.Date,
                    DayPart = timeOfDay,
                    Icon = forecast.Icon,
                    AirTMin = forecast.AirTMin,
                    AirTMax = forecast.AirTMax,
                    WindDirection = forecast.WindDirection,
                    WindDirectionChange = forecast.WindDirectionChange,
                    WindSpeedMin = forecast.WindSpeedMin,
                    WindSpeedMax = forecast.WindSpeedMax,
                    WindSpeedMinAfterChange = forecast.WindSpeedMinAfterChange,
                    WindSpeedMaxAfterChange = forecast.WindSpeedMaxAfterChange,
                    CloudAmount = forecast.CloudAmount,
                    TimePeriod = forecast.TimePeriod,
                    Precipitation = forecast.Precipitation,
                    IsOccasional = forecast.IsOccasional,
                    IsPossible = forecast.IsPossible,
                    Thunderstorm = forecast.Thunderstorm,
                    Location = forecast.Location
                };

                var newId = await Mediator.Send(newUgmWeatherDataRequest);
            }

            if (needToUpdateStations)
            {
                await UgmDbContext.SaveChangesAsync();
            }

            Logger.LogInformation("End UgmWeatherForecastDataLoaderJob");
        }
    }

    public class UgmWeatherForecastDataLoaderJobResult : BaseJobResult
    {
        public string Message { get; set; }
    }
}
