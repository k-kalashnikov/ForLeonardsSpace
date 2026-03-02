using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Era
{
    public class Era5DayWeatherForecastUpsertCommand : IRequest
    {
        [Required]
        public Guid WeatherStationId { get; set; }

        [Required]
        public required List<EraWeatherData> EraWeatherData { get; set; }
    }

    public class Era5DayWeatherForecastUpsertCommandHandler : IRequestHandler<Era5DayWeatherForecastUpsertCommand>
    {
        private MasofaEraDbContext MasofaWeatherReportDbContext { get; set; }

        public Era5DayWeatherForecastUpsertCommandHandler(MasofaEraDbContext masofaWeatherForecastDbContext)
        {
            MasofaWeatherReportDbContext = masofaWeatherForecastDbContext;
        }

        public async Task Handle(Era5DayWeatherForecastUpsertCommand command, CancellationToken cancellationToken)
        {
            var dates = command.EraWeatherData
                .Where(e => e.OriginalDateTimeUtc != null && DateOnly.FromDateTime(e.OriginalDateTimeUtc.Value) > DateOnly.FromDateTime(DateTime.UtcNow))
                .Select(e => DateOnly.FromDateTime(e.OriginalDateTimeUtc.Value))
                .Distinct()
                .ToList();

            var existingForecastsInDb = await MasofaWeatherReportDbContext.Era5DayWeatherForecasts
                .Where(r => dates.Contains(r.Date) && r.WeatherStation == command.WeatherStationId)
                .ToListAsync(cancellationToken);

            var reportLookup = existingForecastsInDb.ToDictionary(r => r.Date, r => r);

            var neededForecasts = command.EraWeatherData.Where(d => d.OriginalDateTimeUtc != null
                && DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value) > DateOnly.FromDateTime(DateTime.UtcNow));

            var neededDates = neededForecasts.Select(r => r.OriginalDateTimeUtc?.Date).ToList();

            var temperatureByDate = neededForecasts.Where(d => d.OriginalDateTimeUtc?.Hour >= 8 && d.OriginalDateTimeUtc?.Hour <= 20)
                .GroupBy(d => DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value))
                .Select(d => new
                {
                    Date = d.Key,
                    TemperatureMax = d.Max(x => x.Temperature),
                    TemperatureMin = d.Min(x => x.Temperature)
                })
                .ToList();

            var nightTemperatureByDate = neededForecasts.Where(d => !(d.OriginalDateTimeUtc?.Hour >= 8 && d.OriginalDateTimeUtc?.Hour <= 20))
                .GroupBy(d => DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value))
                .Select(d => new
                {
                    Date = d.Key,
                    TemperatureMax = d.Max(x => x.Temperature),
                    TemperatureMin = d.Min(x => x.Temperature),
                    TemperatureAverage = d.Average(x => x.Temperature)
                })
                .ToList();

            var valuesByDate = neededForecasts
                .GroupBy(d => DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value))
                .Select(d => new
                {
                    Date = d.Key,
                    RadiationSum = d.Sum(x => x.SolarRadiation),
                    PrecipitationSum = d.Sum(x => x.Precipitation),
                    AverageHumidity = d.Average(x => x.RelativeHumidity),
                    AverageWindSpeed = d.Average(x => x.WindSpeed),
                    AverageWindDirection = d.Average(x => x.WindDirection)
                })
                .ToList();

            foreach (var dt in neededDates)
            {
                if (dt == null) continue;

                var date = DateOnly.FromDateTime(dt.Value);
                if (!reportLookup.TryGetValue(date, out var report))
                {
                    report = new Era5DayWeatherForecast
                    {
                        WeatherStation = command.WeatherStationId,
                        Date = date
                    };

                    MasofaWeatherReportDbContext.Era5DayWeatherForecasts.Add(report);
                    reportLookup[date] = report;
                }

                report.TemperatureMax = temperatureByDate.FirstOrDefault(x => x.Date == date)?.TemperatureMax ?? default!;
                report.TemperatureMin = temperatureByDate.FirstOrDefault(x => x.Date == date)?.TemperatureMin ?? default!;
                report.TemperatureMaxTotal = temperatureByDate.FirstOrDefault(x => x.Date == date)?.TemperatureMax ?? default!;
                report.TemperatureMinTotal = temperatureByDate.FirstOrDefault(x => x.Date == date)?.TemperatureMin ?? default!;
                report.SolarRadiationInfluence = valuesByDate.FirstOrDefault(x => x.Date == date)?.RadiationSum ?? default!;
                report.Fallout = valuesByDate.FirstOrDefault(x => x.Date == date)?.PrecipitationSum ?? default!;
                report.Humidity = valuesByDate.FirstOrDefault(x => x.Date == date)?.AverageHumidity ?? default!;
                report.WindSpeed = valuesByDate.FirstOrDefault(x => x.Date == date)?.AverageWindSpeed ?? default!;
                report.WindDerection = valuesByDate.FirstOrDefault(x => x.Date == date)?.AverageWindDirection ?? default!;
                report.IsFrostDanger = nightTemperatureByDate.FirstOrDefault(x => x.Date == date)?.TemperatureAverage < -5;
            }

            await MasofaWeatherReportDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
