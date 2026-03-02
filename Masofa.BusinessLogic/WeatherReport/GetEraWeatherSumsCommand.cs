using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.WeatherReport
{
    public class GetEraWeatherSumsCommand : IRequest<WeatherReportWithSums>
    {
        [Required]
        public required DateOnly Date { get; set; }

        [Required]
        public required List<Guid> StationIds { get; set; }
    }

    public class GetEraWeatherSumsCommandHandler : IRequestHandler<GetEraWeatherSumsCommand, WeatherReportWithSums>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<GetEraWeatherSumsCommandHandler> Logger { get; set; }

        private MasofaEraDbContext EraDbContext { get; set; }

        public GetEraWeatherSumsCommandHandler(IBusinessLogicLogger businessLogicLogger, ILogger<GetEraWeatherSumsCommandHandler> logger, MasofaEraDbContext eraDbContext)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            EraDbContext = eraDbContext;
        }

        public async Task<WeatherReportWithSums> Handle(GetEraWeatherSumsCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var startDate = new DateOnly(request.Date.Year, 1, 1);

                var previousReports = await EraDbContext.Era5DayWeatherReports
                    .Where(x => x.WeatherStation != null && request.StationIds.Contains(x.WeatherStation.Value) && x.Date >= startDate && x.Date <= request.Date)
                    .GroupBy(x => x.WeatherStation.Value)
                    .ToDictionaryAsync(x => x.Key, x => x.OrderByDescending(x => x.Date).ToList(), cancellationToken);

                Dictionary<Guid, List<BaseEra5WeatherReport>> reportsByStation = [];
                foreach (var (stId, rl) in previousReports)
                {
                    var reportsList = reportsByStation.GetValueOrDefault(stId, []);
                    reportsList.AddRange(rl);
                    reportsByStation[stId] = reportsList;
                }

                var lastReportDate = startDate;
                if (previousReports.Count != 0)
                {
                    lastReportDate = previousReports.Values.ToList()[0][0].Date;
                }

                if (Math.Abs(request.Date.DayNumber - lastReportDate.DayNumber) > 0)
                {
                    var previousForecasts = await EraDbContext.Era5DayWeatherForecasts
                        .Where(x => x.WeatherStation != null && request.StationIds.Contains(x.WeatherStation.Value) && x.Date > lastReportDate && x.Date <= request.Date)
                        .GroupBy(x => x.WeatherStation.Value)
                        .ToDictionaryAsync(x => x.Key, x => x.OrderByDescending(x => x.Date).ToList());

                    foreach (var (stId, rl) in previousForecasts)
                    {
                        var reportsList = reportsByStation.GetValueOrDefault(stId, []);
                        reportsList.AddRange(rl);
                        reportsByStation[stId] = reportsList;
                    }
                }

                Dictionary<Guid, WeatherReportWithSums> valuesDict = [];

                foreach (var (stId, rl) in reportsByStation)
                {
                    var tempReport = new WeatherReportWithSums()
                    {
                        SumOfActiveTemperaturesBase5 = rl.Where(r => r.TemperatureAverage > 5).Sum(r => r.TemperatureAverage - 5),
                        SumOfActiveTemperaturesBase7 = rl.Where(r => r.TemperatureAverage > 7).Sum(r => r.TemperatureAverage - 7),
                        SumOfActiveTemperaturesBase10 = rl.Where(r => r.TemperatureAverage > 10).Sum(r => r.TemperatureAverage - 10),
                        SumOfActiveTemperaturesBase12 = rl.Where(r => r.TemperatureAverage > 12).Sum(r => r.TemperatureAverage - 12),
                        SumOfActiveTemperaturesBase15 = rl.Where(r => r.TemperatureAverage > 15).Sum(r => r.TemperatureAverage - 15),
                        SumOfSolarRadiation = rl.Sum(r => r.SolarRadiationInfluence),
                        SumOfFallout = rl.Sum(r => r.Fallout)
                    };
                    valuesDict[stId] = tempReport;
                }

                var result = new WeatherReportWithSums()
                {
                    SumOfActiveTemperaturesBase5 = valuesDict.Values.Average(v => v.SumOfActiveTemperaturesBase5),
                    SumOfActiveTemperaturesBase7 = valuesDict.Values.Average(v => v.SumOfActiveTemperaturesBase7),
                    SumOfActiveTemperaturesBase10 = valuesDict.Values.Average(v => v.SumOfActiveTemperaturesBase10),
                    SumOfActiveTemperaturesBase12 = valuesDict.Values.Average(v => v.SumOfActiveTemperaturesBase12),
                    SumOfActiveTemperaturesBase15 = valuesDict.Values.Average(v => v.SumOfActiveTemperaturesBase15),
                    SumOfSolarRadiation = valuesDict.Values.Average(v => v.SumOfSolarRadiation),
                    SumOfFallout = valuesDict.Values.Average(v => v.SumOfFallout)
                };

                return result;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}
