//using Masofa.Common.Models.Era;
//using Masofa.DataAccess;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;

//namespace Masofa.BusinessLogic.Era
//{
//    public class Era5HourWeatherReportUpsertCommand : IRequest
//    {
//        [Required]
//        public Guid WeatherStationId { get; set; }

//        [Required]
//        public required List<EraWeatherData> EraWeatherData { get; set; }
//    }

//    public class Era5HourWeatherReportUpsertCommandHandler : IRequestHandler<Era5HourWeatherReportUpsertCommand>
//    {
//        private MasofaWeatherReportDbContext MasofaWeatherReportDbContext { get; set; }

//        public Era5HourWeatherReportUpsertCommandHandler(MasofaWeatherReportDbContext masofaWeatherReportDbContext)
//        {
//            MasofaWeatherReportDbContext = masofaWeatherReportDbContext;
//        }

//        public async Task Handle(Era5HourWeatherReportUpsertCommand command, CancellationToken cancellationToken)
//        {
//            var dates = command.EraWeatherData
//                .Where(e => e.OriginalDateTimeUtc != null && DateOnly.FromDateTime(e.OriginalDateTimeUtc.Value) == DateOnly.FromDateTime(DateTime.UtcNow))
//                .Select(e => DateOnly.FromDateTime(e.OriginalDateTimeUtc.Value))
//                .Distinct()
//                .ToList();

//            var existingReportsInDb = await MasofaWeatherReportDbContext.Era5HourWeatherReports
//                .Where(r => dates.Contains(r.Date) && r.WeatherStation == command.WeatherStationId)
//                .ToListAsync(cancellationToken);

//            var reportLookup = existingReportsInDb.ToDictionary(r => (r.Date, r.Hour), r => r);

//            foreach (var data in command.EraWeatherData)
//            {
//                if (data.OriginalDateTimeUtc == null || DateOnly.FromDateTime(data.OriginalDateTimeUtc.Value) > DateOnly.FromDateTime(DateTime.UtcNow))
//                {
//                    continue;
//                }

//                var date = DateOnly.FromDateTime(data.OriginalDateTimeUtc.Value);
//                var hour = data.OriginalDateTimeUtc.Value.Hour;
//                var key = (date, hour);

//                if (!reportLookup.TryGetValue(key, out var report))
//                {
//                    report = new Era5HourWeatherReport
//                    {
//                        WeatherStation = data.EraWeatherStationId,
//                        Date = date,
//                        Hour = hour
//                    };

//                    MasofaWeatherReportDbContext.Era5HourWeatherReports.Add(report);
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
