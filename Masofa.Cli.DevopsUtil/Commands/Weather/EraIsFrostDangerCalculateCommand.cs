using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Masofa.Cli.DevopsUtil.Commands.Weather
{
    [BaseCommand("Fill Is Frost Danger Era Data", "Заполнение данных по морозоопасности")]
    public class EraIsFrostDangerCalculateCommand : IBaseCommand
    {
        private MasofaEraDbContext EraDbContext { get; set; }

        public EraIsFrostDangerCalculateCommand(MasofaEraDbContext eraDbContext)
        {
            EraDbContext = eraDbContext;
        }

        public void Dispose()
        {
            Console.WriteLine($"EraIsFrostDangerCalculateCommand end");
        }

        public async Task Execute()
        {
            Console.WriteLine($"EraIsFrostDangerCalculateCommand start");
            var eraWeatherData = await EraDbContext.EraWeatherData
                .GroupBy(d => d.EraWeatherStationId)
                .ToDictionaryAsync(d => d.Key, d => d.ToList());

            var reports = await EraDbContext.Era5DayWeatherReports.ToListAsync();
            var forecasts = await EraDbContext.Era5DayWeatherForecasts.ToListAsync();

            foreach (var report in reports)
            {
                if (report.WeatherStation == null)
                {
                    continue;
                }

                var coun = reports.IndexOf(report) + 1;

                var currentStationData = eraWeatherData[report.WeatherStation.Value];
                var currentDatData = currentStationData.Where(d => d.OriginalDateTimeUtc != null && DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value) == report.Date).ToList();
                var nightTemperature = currentDatData.Where(d => !(d.OriginalDateTimeUtc?.Hour >= 8 && d.OriginalDateTimeUtc?.Hour <= 20)).ToList();
                var averageNightTemperature = nightTemperature.Average(d => d.Temperature);
                report.IsFrostDanger = averageNightTemperature < -5;

                Console.Write($"\rReports: {coun}/{reports.Count}");
            }
            Console.WriteLine();

            foreach (var forecast in forecasts)
            {
                if (forecast.WeatherStation == null)
                {
                    continue;
                }

                var coun = forecasts.IndexOf(forecast) + 1;

                var currentStationData = eraWeatherData[forecast.WeatherStation.Value];
                var currentDatData = currentStationData.Where(d => d.OriginalDateTimeUtc != null && DateOnly.FromDateTime(d.OriginalDateTimeUtc.Value) == forecast.Date).ToList();
                var nightTemperature = currentDatData.Where(d => !(d.OriginalDateTimeUtc?.Hour >= 8 && d.OriginalDateTimeUtc?.Hour <= 20)).ToList();
                var averageNightTemperature = nightTemperature.Average(d => d.Temperature);
                forecast.IsFrostDanger = averageNightTemperature < -5;
                Console.Write($"\rForecasts: {coun}/{forecasts.Count}");
            }
            Console.WriteLine();

            await EraDbContext.SaveChangesAsync();
        }

        public Task Execute(string[] args)
        {
            return Task.CompletedTask;
        }
    }
}
