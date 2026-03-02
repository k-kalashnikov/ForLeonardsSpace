using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Era
{
    public class Era5MonthWeatherReportCommand : IRequest<int>
    {
        public DateOnly Today { get; set; }
    }

    public class Era5MonthWeatherReportCommandHandler : IRequestHandler<Era5MonthWeatherReportCommand, int>
    {
        private MasofaEraDbContext WeatherReportDbContext { get; set; }

        public Era5MonthWeatherReportCommandHandler(MasofaEraDbContext weatherReportDbContext)
        {
            WeatherReportDbContext = weatherReportDbContext;
        }

        public async Task<int> Handle(Era5MonthWeatherReportCommand request, CancellationToken cancellationToken)
        {
            var month = request.Today.Month;
            var year = request.Today.Year;

            var dailyReports = await WeatherReportDbContext.Era5DayWeatherReports
                .Where(r => r.Date.Month == month && r.Date.Year == year && r.WeatherStation != null)
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

            var existingReports = await WeatherReportDbContext.Era5MonthWeatherReports
                .Where(r => r.Month == month && r.Year == year && r.WeatherStation != null)
                .ToDictionaryAsync(r => r.WeatherStation.Value, cancellationToken);

            foreach (var dailyReport in dailyReports)
            {
                if (!existingReports.TryGetValue(dailyReport.WeatherStationId, out var report))
                {
                    report = new Era5MonthWeatherReport
                    {
                        WeatherStation = dailyReport.WeatherStationId,
                        Month = month,
                        Year = year
                    };

                    WeatherReportDbContext.Era5MonthWeatherReports.Add(report);
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

            return request.Today.Year;
        }
    }
}
