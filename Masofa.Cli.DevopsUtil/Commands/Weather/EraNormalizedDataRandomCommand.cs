using Masofa.Common.Models.Era;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Masofa.Cli.DevopsUtil.Commands.Weather
{
    [BaseCommand("Fill Normalizer Era Data with random values", "Заполнение нормализованных данных (рандом)")]
    public class EraNormalizedDataRandomCommand : IBaseCommand
    {
        private MasofaEraDbContext EraDbContext { get; set; }

        public EraNormalizedDataRandomCommand(MasofaEraDbContext eraDbContext)
        {
            EraDbContext = eraDbContext;
        }

        public void Dispose()
        {
            Console.WriteLine($"EraNormalizedDataRandomCommand end");
        }

        public async Task Execute()
        {
            Console.WriteLine($"EraNormalizedDataRandomCommand start");
            var before10Day = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
            var after10Day = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));

            var reports = await EraDbContext.Era5DayWeatherReports.Where(r => r.Date <= after10Day && r.Date >= before10Day).ToListAsync();
            var forecasts = await EraDbContext.Era5DayWeatherForecasts.Where(r => r.Date <= after10Day && r.Date >= before10Day).ToListAsync();

            HashSet<string> processed = [];
            List<Era5DayNormalizedWeather> era5DayNormalizedWeather = [];

            var rand = new Random();
            foreach (var report in reports)
            {
                var key = $"{report.WeatherStation}_{report.Date:yyyyMMdd}";
                processed.Add(key);
                era5DayNormalizedWeather.Add(new Era5DayNormalizedWeather()
                {
                    Month = report.Date.Month,
                    Day = report.Date.Day,
                    TemperatureMin = report.TemperatureMin + rand.Next(-2, 3),
                    TemperatureMax = report.TemperatureMax + rand.Next(-2, 3),
                    TemperatureMinTotal = report.TemperatureMinTotal + rand.Next(-2, 3),
                    TemperatureMaxTotal = report.TemperatureMaxTotal + rand.Next(-2, 3),
                    SolarRadiationInfluence = report.SolarRadiationInfluence + rand.Next(-50, 51),
                    Fallout = CalcFallout(report.Fallout),
                    Humidity = report.Humidity + rand.Next(-50, 51) / 10.0,
                    WindSpeed = CalcWindSpeed(report.WindSpeed),
                    WindDerection = CalcWindDir(report.WindDerection),
                    WeatherStation = report.WeatherStation
                });
            }

            foreach (var forecast in forecasts)
            {
                var key = $"{forecast.WeatherStation}_{forecast.Date:yyyyMMdd}";

                if (processed.Contains(key))
                {
                    continue;
                }

                processed.Add(key);
                era5DayNormalizedWeather.Add(new Era5DayNormalizedWeather()
                {
                    Month = forecast.Date.Month,
                    Day = forecast.Date.Day,
                    TemperatureMin = forecast.TemperatureMin + rand.Next(-2, 3),
                    TemperatureMax = forecast.TemperatureMax + rand.Next(-2, 3),
                    TemperatureMinTotal = forecast.TemperatureMinTotal + rand.Next(-2, 3),
                    TemperatureMaxTotal = forecast.TemperatureMaxTotal + rand.Next(-2, 3),
                    SolarRadiationInfluence = forecast.SolarRadiationInfluence + rand.Next(-50, 51),
                    Fallout = CalcFallout(forecast.Fallout),
                    Humidity = forecast.Humidity + rand.Next(-50, 51) / 10.0,
                    WindSpeed = CalcWindSpeed(forecast.WindSpeed),
                    WindDerection = CalcWindDir(forecast.WindDerection),
                    WeatherStation = forecast.WeatherStation
                });
            }

            await EraDbContext.Era5DayNormalizedWeather.AddRangeAsync(era5DayNormalizedWeather);
            await EraDbContext.SaveChangesAsync();
        }

        public Task Execute(string[] args)
        {
            Console.WriteLine($"Execute with args");
            return Task.CompletedTask;
        }

        private static double CalcFallout(double? fallout)
        {
            if (fallout == null)
            {
                return 0;
            }

            var result = fallout + new Random().Next(-1, 2) / 10.0;
            if (result < 0)
            {
                return 0;
            }

            return result ?? 0;
        }

        private static double CalcWindSpeed(double? windSpeed)
        {
            if (windSpeed == null)
            {
                return 0;
            }

            var result = windSpeed + new Random().Next(-50, 50) / 10.0;
            if (result < 0)
            {
                return 0;
            }

            return result ?? 0;
        }

        private static double CalcWindDir(double? windDir)
        {
            var result = windDir + new Random().Next(-5, 6);
            if (result < 0)
            {
                result = 360 + result;
            }
            else if (result > 360)
            {
                result -= 360;
            }

            if (result == null)
            {
                return 0;
            }

            return (double)result;
        }
    }
}
