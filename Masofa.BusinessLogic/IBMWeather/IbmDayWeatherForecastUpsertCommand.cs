using Masofa.Common.Models.IBMWeather;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.IBMWeather
{
    public class IbmDayWeatherForecastUpsertCommand : IRequest
    {
        [Required]
        public Guid WeatherStationId { get; set; }

        [Required]
        public required List<IBMWeatherData> IbmWeatherData { get; set; }
    }

    public class IbmDayWeatherForecastUpsertCommandHandler : IRequestHandler<IbmDayWeatherForecastUpsertCommand>
    {
        private MasofaIBMWeatherDbContext IBMWeatherDbContext { get; set; }
        public IbmDayWeatherForecastUpsertCommandHandler(MasofaIBMWeatherDbContext iBMWeatherDbContext)
        {
            IBMWeatherDbContext = iBMWeatherDbContext;
        }

        public async Task Handle(IbmDayWeatherForecastUpsertCommand request, CancellationToken cancellationToken)
        {
            var dates = request.IbmWeatherData
                .Where(e => DateOnly.FromDateTime(e.ValidTimeUtc) >= DateOnly.FromDateTime(DateTime.UtcNow))
                .Select(e => DateOnly.FromDateTime(e.ValidTimeUtc))
                .Distinct()
                .ToList();

            var existingForecastsInDb = await IBMWeatherDbContext.IbmDayWeatherForecasts
                .Where(r => dates.Contains(r.Date) && r.WeatherStation == request.WeatherStationId)
                .ToListAsync(cancellationToken);

            var reportLookup = existingForecastsInDb.ToDictionary(r => r.Date, r => r);

            var neededForecasts = request.IbmWeatherData.Where(d => DateOnly.FromDateTime(d.ValidTimeUtc) >= DateOnly.FromDateTime(DateTime.UtcNow));

            var neededDates = neededForecasts.Select(r => r.ValidTimeUtc.Date).ToList();

            foreach (var dt in neededDates)
            {
                var date = DateOnly.FromDateTime(dt);
                if (!reportLookup.TryGetValue(date, out var report))
                {
                    report = new IbmDayWeatherForecast
                    {
                        WeatherStation = request.WeatherStationId,
                        Date = date
                    };

                    IBMWeatherDbContext.IbmDayWeatherForecasts.Add(report);
                    reportLookup[date] = report;
                }

                var currentForecasts = neededForecasts.Where(x => x.DayOrNight != null && DateOnly.FromDateTime(x.ValidTimeUtc) == date)
                    .ToDictionary(x => x.DayOrNight!, x => x);

                IBMWeatherData? dayForecast = currentForecasts.GetValueOrDefault("D", null!);
                IBMWeatherData? nightForecast = currentForecasts.GetValueOrDefault("N", null!);

                if (dayForecast == null || nightForecast == null) continue;

                report.TemperatureMax = dayForecast.TemperatureMax ?? default!;
                report.TemperatureMin = dayForecast.TemperatureMin ?? default!;
                report.TemperatureMaxTotal = Math.Max(dayForecast.TemperatureMax ?? default!, nightForecast.TemperatureMax ?? default!);
                report.TemperatureMinTotal = Math.Min(dayForecast.TemperatureMin ?? default!, nightForecast.TemperatureMin ?? default!);
                report.Fallout = (dayForecast.Precipitation ?? default!) + (nightForecast.Precipitation ?? default!);
                report.Humidity = ((dayForecast.RelativeHumidity ?? default!) + (nightForecast.RelativeHumidity ?? default!)) / 2;
                report.WindSpeed = ((dayForecast.WindSpeed ?? default!) + (nightForecast.WindSpeed ?? default!)) / 2;
                report.WindDerection = ((dayForecast.WindDirection ?? default!) + (nightForecast.WindDirection ?? default!)) / 2;
            }

            await IBMWeatherDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
