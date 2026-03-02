using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Era
{
    public class Era5WeekWeatherReportCommand : IRequest<DateOnly>
    {
    }

    public class Era5WeekWeatherReportCommandHandler : IRequestHandler<Era5WeekWeatherReportCommand, DateOnly>
    {
        private MasofaEraDbContext WeatherReportDbContext { get; set; }

        public Era5WeekWeatherReportCommandHandler(MasofaEraDbContext weatherReportDbContext)
        {
            WeatherReportDbContext = weatherReportDbContext;
        }

        public async Task<DateOnly> Handle(Era5WeekWeatherReportCommand request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var (weekStart, weekEnd) = GetWeekRange(today);

            var dailyReports = await WeatherReportDbContext.Era5DayWeatherReports
                .Where(r => r.Date >= weekStart && r.Date <= weekEnd && r.WeatherStation != null)
                .GroupBy(r => r.WeatherStation)
                .Select(d => new
                {
                    WeatherStationId = d.Key.Value,
                    TemperatureMin = d.Average(x => x.TemperatureMin),
                    TemperatureMax = d.Average(x => x.TemperatureMax),
                    TemperatureMinTotal = d.Min(x => x.TemperatureMin),
                    TemperatureMaxTotal = d.Max(x => x.TemperatureMax),
                    RadiationSum = d.Sum(x => x.SolarRadiationInfluence),
                    PrecipitationSum = d.Sum(x => x.Fallout),
                    AverageHumidity = d.Average(x => x.Humidity),
                    AverageWindSpeed = d.Average(x => x.WindSpeed),
                    AverageWindDirection = d.Average(x => x.WindDerection)
                })
                .ToListAsync(cancellationToken);

            var existingReports = await WeatherReportDbContext.Era5WeekWeatherReports
                .Where(r => r.WeekStart == weekStart && r.WeekEnd == weekEnd && r.WeatherStation != null)
                .ToDictionaryAsync(r => r.WeatherStation.Value, cancellationToken);

            foreach (var dailyReport in dailyReports)
            {
                if (!existingReports.TryGetValue(dailyReport.WeatherStationId, out var report))
                {
                    report = new Era5WeekWeatherReport
                    {
                        WeatherStation = dailyReport.WeatherStationId,
                        WeekStart = weekStart,
                        WeekEnd = weekEnd
                    };

                    WeatherReportDbContext.Era5WeekWeatherReports.Add(report);
                }

                report.TemperatureMin = dailyReport.TemperatureMin;
                report.TemperatureMax = dailyReport.TemperatureMax;
                report.TemperatureMinTotal = dailyReport.TemperatureMinTotal;
                report.TemperatureMaxTotal = dailyReport.TemperatureMaxTotal;
                report.SolarRadiationInfluence = dailyReport.RadiationSum;
                report.Fallout = dailyReport.PrecipitationSum;
                report.Humidity = dailyReport.AverageHumidity;
                report.WindSpeed = dailyReport.AverageWindSpeed;
                report.WindDerection = dailyReport.AverageWindDirection;
            }

            await WeatherReportDbContext.SaveChangesAsync(cancellationToken);

            return today;
        }

        private (DateOnly StartOfWeek, DateOnly EndOfWeek) GetWeekRange(DateOnly date)
        {
            var dayOfWeek = date.DayOfWeek;

            var daysToMonday = (int)dayOfWeek - (int)DayOfWeek.Monday;

            if (dayOfWeek == DayOfWeek.Sunday)
            {
                daysToMonday = -1;
            }

            var startOfWeek = date.AddDays(-daysToMonday);
            var endOfWeek = startOfWeek.AddDays(6);

            return (startOfWeek, endOfWeek);
        }
    }
}