//using Masofa.Common.Models.Era;
//using Masofa.DataAccess;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;

//namespace Masofa.BusinessLogic.Era
//{
//    public class Era5HourWeatherForecastUpsertCommand : IRequest
//    {
//        [Required]
//        public Guid WeatherStationId { get; set; }

//        [Required]
//        public required List<EraWeatherData> EraWeatherData { get; set; }
//    }

//    public class Era5HourWeatherForecastUpsertCommandHandler : IRequestHandler<Era5HourWeatherForecastUpsertCommand>
//    {
//        private MasofaWeatherReportDbContext MasofaWeatherReportDbContext { get; set; }

//        public Era5HourWeatherForecastUpsertCommandHandler(MasofaWeatherReportDbContext masofaWeatherForecastDbContext)
//        {
//            MasofaWeatherReportDbContext = masofaWeatherForecastDbContext;
//        }

//        public async Task Handle(Era5HourWeatherForecastUpsertCommand command, CancellationToken cancellationToken)
//        {
//            var dates = command.EraWeatherData
//                .Where(e => e.OriginalDateTimeUtc != null && DateOnly.FromDateTime(e.OriginalDateTimeUtc.Value) > DateOnly.FromDateTime(DateTime.UtcNow))
//                .Select(e => DateOnly.FromDateTime(e.OriginalDateTimeUtc.Value))
//                .Distinct()
//                .ToList();

//            var existingForecastsInDb = await MasofaWeatherReportDbContext.Era5HourWeatherForecasts
//                .Where(r => dates.Contains(r.Date) && r.WeatherStation == command.WeatherStationId)
//                .ToListAsync(cancellationToken);

//            var reportLookup = existingForecastsInDb.ToDictionary(r => (r.Date, r.Hour), r => r);

//            foreach (var data in command.EraWeatherData)
//            {
//                if (data.OriginalDateTimeUtc == null || DateOnly.FromDateTime(data.OriginalDateTimeUtc.Value) <= DateOnly.FromDateTime(DateTime.UtcNow))
//                {
//                    continue;
//                }

//                var date = DateOnly.FromDateTime(data.OriginalDateTimeUtc.Value);
//                var hour = data.OriginalDateTimeUtc.Value.Hour;
//                var key = (date, hour);

//                if (!reportLookup.TryGetValue(key, out var report))
//                {
//                    report = new Era5HourWeatherForecast
//                    {
//                        WeatherStation = data.EraWeatherStationId,
//                        Date = date,
//                        Hour = hour
//                    };

//                    MasofaWeatherReportDbContext.Era5HourWeatherForecasts.Add(report);
//                    reportLookup[key] = report;
//                }

//                report.TemperatureMax = data.Temperature ?? default!;
//                report.TemperatureMin = data.Temperature ?? default!;
//                report.TemperatureMaxTotal = data.Temperature ?? default!;
//                report.TemperatureMinTotal = data.Temperature ?? default!;
//                report.SolarRadiationInfluence = data.SolarRadiation ?? default!;
//                report.Fallout = data.Precipitation ?? default!;
//                report.Humidity = data.RelativeHumidity ?? default!;
//                report.WindSpeed = data.WindSpeed ?? default!;
//                report.WindDerection = data.WindDirection ?? default!;
//            }

//            await MasofaWeatherReportDbContext.SaveChangesAsync(cancellationToken);
//        }
//    }
//}
