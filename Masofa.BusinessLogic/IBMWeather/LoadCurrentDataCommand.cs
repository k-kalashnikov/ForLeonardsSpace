using MediatR;
using Masofa.Common.Models.IBMWeather;
using Microsoft.EntityFrameworkCore;
using Masofa.BusinessLogic.IBMWeather.BaseCommands;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.IBMWeather;
using Masofa.DataAccess;
using Microsoft.Extensions.Configuration;

namespace Masofa.BusinessLogic.IBMWeather.Commands;

/// <summary>
/// Команда для загрузки текущих данных IBM Weather
/// </summary>
public class LoadCurrentDataCommand : BaseIBMWeatherLoadCommand
{
    /// <summary>
    /// Идентификатор метеостанции (если null - для всех активных станций)
    /// </summary>
    public Guid? IBMMeteoStationId { get; set; }

    /// <summary>
    /// Принудительное обновление существующих данных
    /// </summary>
    public bool ForceUpdate { get; set; } = false;

    public LoadCurrentDataCommand(
        Guid? ibmMeteoStationId = null,
        bool forceUpdate = false)
    {
        IBMMeteoStationId = ibmMeteoStationId;
        ForceUpdate = forceUpdate;
    }
}

/// <summary>
/// Обработчик команды для загрузки текущих данных
/// </summary>
//public class LoadCurrentDataHandler : BaseIBMWeatherLoadHandler<LoadCurrentDataCommand>
//{
//    public LoadCurrentDataHandler(
//    IBMWeatherApiUnitOfWork unitOfWork,
//    MasofaIBMWeatherDbContext dbContext,
//    IBusinessLogicLogger logger,
//    IConfiguration configuration) : base(unitOfWork, dbContext, logger, configuration)
//    {
//    }

//    public override async Task Handle(LoadCurrentDataCommand request, CancellationToken cancellationToken)
//    {
//        await LogOperationStartAsync("LoadCurrentData", 
//            $"StationId: {request.IBMMeteoStationId}, ForceUpdate: {request.ForceUpdate}");

//        try
//        {
//            var stations = await GetStationsToProcessAsync(request.IBMMeteoStationId);
//            var processedCount = 0;

//            foreach (var station in stations)
//            {
//                var count = await LoadCurrentDataForStationAsync(station, request.ForceUpdate);
//                processedCount += count;
//            }

//            await LogOperationEndAsync("LoadCurrentData", processedCount, "записей");
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadCurrentData", ex);
//            throw;
//        }
//    }

//    /// <summary>
//    /// Получение списка станций для обработки
//    /// </summary>
//    private async Task<List<IBMMeteoStation>> GetStationsToProcessAsync(Guid? stationId)
//    {
//        if (stationId.HasValue)
//        {
//            var station = await _dbContext.IBMMeteoStations
//                .FirstOrDefaultAsync(s => s.Id == stationId.Value);

//            if (station == null)
//            {
//                throw new ArgumentException($"Метеостанция с ID {stationId.Value} не найдена");
//            }

//            return new List<IBMMeteoStation> { station };
//        }

//        // Получаем все активные станции
//        return await _dbContext.IBMMeteoStations
//            .Where(s => s.IsActive && s.Status == Masofa.Common.Models.StatusType.Active)
//            .ToListAsync();
//    }

//    /// <summary>
//    /// Загрузка текущих данных для конкретной станции
//    /// </summary>
//    private async Task<int> LoadCurrentDataForStationAsync(IBMMeteoStation station, bool forceUpdate)
//    {
//        try
//        {
//            var observationsResponse = await _unitOfWork.IBMWeatherRepository.GetObservationsAsync(
//                station.Point);

//            var observation = observationsResponse.Observation;
//            var metadata = observationsResponse.Metadata;

//            // Проверяем, существует ли уже текущая запись для этой станции
//            var existingData = await _dbContext.IBMWeatherData
//                .FirstOrDefaultAsync(w => 
//                    w.IBMMeteoStationId == station.Id && 
//                    w.ValidTimeLocal.Date == DateTime.Today &&
//                    w.Temperature.HasValue && // Признак текущих данных
//                    !w.TemperatureMax.HasValue); // Исключаем дневные сводки


//            if (existingData != null && !forceUpdate)
//            {
//                // Обновляем существующую запись
//                existingData.Temperature = observation.Temp;
//                existingData.FeelsLike = observation.FeelsLike;
//                existingData.DewPoint = observation.DewPt;
//                existingData.Humidity = observation.Rh;
//                existingData.WindSpeed = observation.Wspd;
//                existingData.WindDirection = observation.Wdir;
//                existingData.WindGust = observation.Gust;
//                existingData.Pressure = observation.Pressure;
//                existingData.WeatherPhrase = observation.WxPhrase;
//                existingData.Precipitation = observation.PrecipTotal;
//                existingData.UvIndex = observation.UvIndex;
//                existingData.UvDescription = observation.UvDesc;
//                existingData.IconCode = observation.IconExtd;
//                existingData.TemperatureMax = observation.MaxTemp;
//                existingData.TemperatureMin = observation.MinTemp;
//                existingData.LastUpdateAt = DateTime.UtcNow;

//                await SaveChangesAsync();
//                return 1; // Обновлена 1 запись
//            }
//            else if (existingData == null)
//            {
//                // Создаем новую запись
//                var weatherData = new IBMWeatherData
//                {
//                    Id = Guid.NewGuid(),
//                    IBMMeteoStationId = station.Id,
//                    ValidTimeUtc = DateTime.UtcNow,
//                    ValidTimeLocal = DateTime.Now,
//                    Temperature = observation.Temp,
//                    FeelsLike = observation.FeelsLike,
//                    DewPoint = observation.DewPt,
//                    Humidity = observation.Rh,
//                    WindSpeed = observation.Wspd,
//                    WindDirection = observation.Wdir,
//                    WindGust = observation.Gust,
//                    Pressure = observation.Pressure,
//                    WeatherPhrase = observation.WxPhrase,
//                    Precipitation = observation.PrecipTotal,
//                    UvIndex = observation.UvIndex,
//                    UvDescription = observation.UvDesc,
//                    IconCode = observation.IconExtd,
//                    TemperatureMax = observation.MaxTemp,
//                    TemperatureMin = observation.MinTemp,
//                    CreateAt = DateTime.UtcNow,
//                    Status = Masofa.Common.Models.StatusType.Active,
//                    LastUpdateAt = DateTime.UtcNow
//                };

//                _dbContext.IBMWeatherData.Add(weatherData);
//                await SaveChangesAsync();
//                return 1; // Создана 1 запись
//            }

//            return 0; // Ничего не изменилось
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadCurrentDataForStation", ex, $"StationId: {station.Id}");
//            return 0; // Продолжаем обработку других станций
//        }
//    }
//}
