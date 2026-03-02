using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Era
{
    public class Era5DayWeatherReportUpsertCommand : IRequest
    {
        [Required]
        public Guid WeatherStationId { get; set; }

        [Required]
        public required List<EraWeatherData> EraWeatherData { get; set; }
    }

    public class Era5DayWeatherReportUpsertCommandHandler : IRequestHandler<Era5DayWeatherReportUpsertCommand>
    {
        private MasofaEraDbContext MasofaWeatherReportDbContext { get; set; }

        public Era5DayWeatherReportUpsertCommandHandler(MasofaEraDbContext masofaWeatherReportDbContext)
        {
            MasofaWeatherReportDbContext = masofaWeatherReportDbContext;
        }

        public async Task Handle(Era5DayWeatherReportUpsertCommand command, CancellationToken cancellationToken)
        {
            var dates = command.EraWeatherData
                .Where(e => e.OriginalDateTimeUtc != null && DateOnly.FromDateTime(e.OriginalDateTimeUtc.Value) == DateOnly.FromDateTime(DateTime.UtcNow))
                .Select(e => DateOnly.FromDateTime(e.OriginalDateTimeUtc.Value))
                .Distinct()
                .ToList();

            var existingReportsInDb = await MasofaWeatherReportDbContext.Era5DayWeatherReports
                .Where(r => dates.Contains(r.Date) && r.WeatherStation == command.WeatherStationId)
                .ToListAsync(cancellationToken);

            var reportLookup = existingReportsInDb.ToDictionary(r => r.Date, r => r);

            var neededReports = command.EraWeatherData.Where(d => d.OriginalDateTimeUtc != null
                && DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value) <= DateOnly.FromDateTime(DateTime.UtcNow));

            var neededDates = neededReports.Select(r => r.OriginalDateTimeUtc?.Date).ToList();

            var temperatureByDate = neededReports.Where(d => d.OriginalDateTimeUtc?.Hour >= 8 && d.OriginalDateTimeUtc?.Hour <= 20)
                .GroupBy(d => DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value))
                .Select(d => new
                {
                    Date = d.Key,
                    TemperatureMin = d.Min(x => x.Temperature),
                    TemperatureMax = d.Max(x => x.Temperature),
                })
                .ToList();

            var nightTemperatureByDate = neededReports.Where(d => !(d.OriginalDateTimeUtc?.Hour >= 8 && d.OriginalDateTimeUtc?.Hour <= 20))
                .GroupBy(d => DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value))
                .Select(d => new
                {
                    Date = d.Key,
                    TemperatureMin = d.Min(x => x.Temperature),
                    TemperatureMax = d.Max(x => x.Temperature),
                    TemperatureAverage = d.Average(x => x.Temperature),
                })
                .ToList();

            var valuesByDate = neededReports
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
                    report = new Era5DayWeatherReport
                    {
                        WeatherStation = command.WeatherStationId,
                        Date = date
                    };

                    MasofaWeatherReportDbContext.Era5DayWeatherReports.Add(report);
                    reportLookup[date] = report;
                }

                report.TemperatureMin = temperatureByDate.FirstOrDefault(x => x.Date == date)?.TemperatureMin ?? default!;
                report.TemperatureMax = temperatureByDate.FirstOrDefault(x => x.Date == date)?.TemperatureMax ?? default!;
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
