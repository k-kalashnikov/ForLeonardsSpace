using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Era
{
    public class Era5YearWeatherReportCommand : IRequest
    {
        public int Year { get; set; }
    }

    public class Era5YearWeatherReportCommandHandler : IRequestHandler<Era5YearWeatherReportCommand>
    {
        private MasofaEraDbContext WeatherReportDbContext { get; set; }

        public Era5YearWeatherReportCommandHandler(MasofaEraDbContext weatherReportDbContext)
        {
            WeatherReportDbContext = weatherReportDbContext;
        }

        public async Task Handle(Era5YearWeatherReportCommand request, CancellationToken cancellationToken)
        {
            var monthReports = await WeatherReportDbContext.Era5MonthWeatherReports
                .Where(r => r.Year == request.Year && r.WeatherStation != null)
                .GroupBy(r => r.WeatherStation)
                .Select(d => new
                {
                    WeatherStationId = d.Key.Value,
                    TemperatureMin = d.Average(x => x.TemperatureMin),
                    TemperatureMax = d.Average(x => x.TemperatureMax),
                    TemperatureMinTotal = d.Average(x => x.TemperatureMinTotal),
                    TemperatureMaxTotal = d.Average(x => x.TemperatureMaxTotal),
                    RadiationSum = d.Sum(x => x.SolarRadiationInfluence),
                    PrecipitationSum = d.Sum(x => x.Fallout),
                    AverageHumidity = d.Average(x => x.Humidity),
                    AverageWindSpeed = d.Average(x => x.WindSpeed),
                    AverageWindDirection = d.Average(x => x.WindDerection)
                })
                .ToListAsync(cancellationToken);

            var existingReports = await WeatherReportDbContext.Era5YearWeatherReports
                .Where(r => r.Year == request.Year && r.WeatherStation != null)
                .ToDictionaryAsync(r => r.WeatherStation.Value, cancellationToken);

            foreach (var monthReport in monthReports)
            {
                if (!existingReports.TryGetValue(monthReport.WeatherStationId, out var report))
                {
                    report = new Era5YearWeatherReport
                    {
                        WeatherStation = monthReport.WeatherStationId,
                        Year = request.Year
                    };

                    WeatherReportDbContext.Era5YearWeatherReports.Add(report);
                }

                report.TemperatureMin = monthReport.TemperatureMin;
                report.TemperatureMax = monthReport.TemperatureMax;
                report.TemperatureMinTotal = monthReport.TemperatureMinTotal;
                report.TemperatureMaxTotal = monthReport.TemperatureMaxTotal;
                report.SolarRadiationInfluence = monthReport.RadiationSum;
                report.Fallout = monthReport.PrecipitationSum;
                report.Humidity = monthReport.AverageHumidity;
                report.WindSpeed = monthReport.AverageWindSpeed;
                report.WindDerection = monthReport.AverageWindDirection;
            }

            await WeatherReportDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
