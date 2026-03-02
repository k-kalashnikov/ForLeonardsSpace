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
/// Команда для загрузки прогнозных данных IBM Weather
/// </summary>
public class LoadForecastDataCommand : BaseIBMWeatherLoadCommand
{
    /// <summary>
    /// Идентификатор метеостанции (если null - для всех активных станций)
    /// </summary>
    public Guid? IBMMeteoStationId { get; set; }

    /// <summary>
    /// Принудительное обновление существующих данных
    /// </summary>
    public bool ForceUpdate { get; set; } = false;

    /// <summary>
    /// Загружать ли дневной прогноз (7 дней)
    /// </summary>
    public bool LoadDailyForecast { get; set; } = true;

    /// <summary>
    /// Загружать ли HOD данные
    /// </summary>
    public bool LoadHodData { get; set; } = true;

    public LoadForecastDataCommand(
        Guid? ibmMeteoStationId = null,
        bool forceUpdate = false,
        bool loadDailyForecast = true,
        bool loadHodData = true)
    {
        IBMMeteoStationId = ibmMeteoStationId;
        ForceUpdate = forceUpdate;
        LoadDailyForecast = loadDailyForecast;
        LoadHodData = loadHodData;
    }
}

/// <summary>
/// Обработчик команды для загрузки прогнозных данных
/// </summary>
//public class LoadForecastDataHandler : BaseIBMWeatherLoadHandler<LoadForecastDataCommand>
//{

//    public LoadForecastDataHandler(
//    IBMWeatherApiUnitOfWork unitOfWork,
//    MasofaIBMWeatherDbContext dbContext,
//    IBusinessLogicLogger logger,
//    IConfiguration configuration) : base(unitOfWork, dbContext, logger, configuration)
//    {
//    }

//    public override async Task Handle(LoadForecastDataCommand request, CancellationToken cancellationToken)
//    {
//        await LogOperationStartAsync("LoadForecastData", 
//            $"StationId: {request.IBMMeteoStationId}, ForceUpdate: {request.ForceUpdate}");

//        try
//        {
//            var stations = await GetStationsToProcessAsync(request.IBMMeteoStationId);
//            var processedCount = 0;

//            foreach (var station in stations)
//            {
//                var count = 0;

//                if (request.LoadDailyForecast)
//                {
//                    count += await LoadDailyForecastForStationAsync(station, request.ForceUpdate);
//                }

//                if (request.LoadHodData)
//                {
//                    count += await LoadHodDataForStationAsync(station, request.ForceUpdate);
//                }

//                processedCount += count;
//            }

//            await LogOperationEndAsync("LoadForecastData", processedCount, "записей");
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadForecastData", ex);
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
//    /// Загрузка дневного прогноза для станции
//    /// </summary>
//    private async Task<int> LoadDailyForecastForStationAsync(IBMMeteoStation station, bool forceUpdate)
//    {
//        try
//        {
//            var forecastResponse = await _unitOfWork.IBMWeatherRepository.GetDailyForecastAsync(
//                station.Point);

//            var processedCount = 0;

//            // Обрабатываем основные данные прогноза
//            for (int i = 0; i < forecastResponse.ValidTimeLocal.Count; i++)
//            {
//                var validTimeLocal = DateTime.Parse(forecastResponse.ValidTimeLocal[i]);
                
//                // Проверяем, что это будущая дата (прогноз)
//                if (validTimeLocal.Date <= DateTime.Today.Date)
//                    continue;

//                // Проверяем, существует ли уже такая запись
//                var existingData = await _dbContext.IBMWeatherData
//                    .FirstOrDefaultAsync(w => 
//                        w.IBMMeteoStationId == station.Id && 
//                        w.ValidTimeLocal.Date == validTimeLocal.Date &&
//                        w.TemperatureMax.HasValue && // Признак дневных данных
//                        w.PrecipChance.HasValue); // Признак прогнозных данных

//                if (existingData != null && !forceUpdate)
//                    continue;

//                var weatherData = new IBMWeatherData
//                {
//                    Id = Guid.NewGuid(),
//                    IBMMeteoStationId = station.Id,
//                    ValidTimeUtc = validTimeLocal.ToUniversalTime(),
//                    ValidTimeLocal = validTimeLocal,
//                    TemperatureMax = forecastResponse.CalendarDayTemperatureMax.Count > i ? forecastResponse.CalendarDayTemperatureMax[i] : null,
//                    TemperatureMin = forecastResponse.CalendarDayTemperatureMin.Count > i ? forecastResponse.CalendarDayTemperatureMin[i] : null,
//                    SunriseTimeLocal = forecastResponse.SunriseTimeLocal.Count > i ? DateTime.Parse(forecastResponse.SunriseTimeLocal[i]) : null,
//                    SunsetTimeLocal = forecastResponse.SunsetTimeLocal.Count > i ? DateTime.Parse(forecastResponse.SunsetTimeLocal[i]) : null,
//                    CreateAt = DateTime.UtcNow,
//                    Status = Masofa.Common.Models.StatusType.Active,
//                    LastUpdateAt = DateTime.UtcNow,
//                };

//                if (existingData != null)
//                {
//                    // Обновляем существующую запись
//                    existingData.TemperatureMax = weatherData.TemperatureMax;
//                    existingData.TemperatureMin = weatherData.TemperatureMin;
//                    existingData.SunriseTimeLocal = weatherData.SunriseTimeLocal;
//                    existingData.SunsetTimeLocal = weatherData.SunsetTimeLocal;
//                    existingData.LastUpdateAt = DateTime.UtcNow;
//                }
//                else
//                {
//                    _dbContext.IBMWeatherData.Add(weatherData);
//                }

//                processedCount++;
//            }

//            // Обрабатываем данные daypart (дневные/ночные периоды)
//            foreach (var daypart in forecastResponse.Daypart)
//            {
//                for (int i = 0; i < daypart.DayOrNight.Count; i++)
//                {
//                    var dayOrNight = daypart.DayOrNight[i];
//                    var daypartName = daypart.DaypartName.Count > i ? daypart.DaypartName[i] : null;
//                    var temperature = daypart.Temperature.Count > i ? daypart.Temperature[i] : 0;
//                    var precipChance = daypart.PrecipChance.Count > i ? daypart.PrecipChance[i] : 0;
//                    var qpf = daypart.Qpf.Count > i ? daypart.Qpf[i] : 0;
//                    var qpfSnow = daypart.QpfSnow.Count > i ? daypart.QpfSnow[i] : 0;
//                    var windSpeed = daypart.WindSpeed.Count > i ? daypart.WindSpeed[i] : 0;
//                    var wxPhraseLong = daypart.WxPhraseLong.Count > i ? daypart.WxPhraseLong[i] : null;

//                    // Создаем запись для дневного/ночного периода
//                    var daypartData = new IBMWeatherData
//                    {
//                        Id = Guid.NewGuid(),
//                        IBMMeteoStationId = station.Id,
//                        ValidTimeUtc = DateTime.UtcNow.AddDays(i), // Примерное время
//                        ValidTimeLocal = DateTime.Now.AddDays(i),
//                        Temperature = temperature,
//                        PrecipChance = precipChance,
//                        Qpf = qpf,
//                        QpfSnow = qpfSnow,
//                        WindSpeed = windSpeed,
//                        WxPhraseLong = wxPhraseLong,
//                        DayOrNight = dayOrNight,
//                        CreateAt = DateTime.UtcNow,
//                        Status = Masofa.Common.Models.StatusType.Active,
//                        LastUpdateAt = DateTime.UtcNow
//                    };

//                    _dbContext.IBMWeatherData.Add(daypartData);
//                    processedCount++;
//                }
//            }

//            if (processedCount > 0)
//            {
//                await SaveChangesAsync();
//            }

//            return processedCount;
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadDailyForecastForStation", ex, $"StationId: {station.Id}");
//            return 0;
//        }
//    }

//    /// <summary>
//    /// Загрузка HOD данных для станции
//    /// </summary>
//    private async Task<int> LoadHodDataForStationAsync(IBMMeteoStation station, bool forceUpdate)
//    {
//        try
//        {
//            var hodResponse = await _unitOfWork.IBMWeatherRepository.GetHodDirectWithDefaultProductsAsync(
//                station.Point);

//            var processedCount = 0;

//            foreach (var hodItem in hodResponse)
//            {
//                var validTimeUtc = DateTime.Parse(hodItem.ValidTimeUtc);
                
//                // Проверяем, что это будущее время (прогноз)
//                if (validTimeUtc <= DateTime.UtcNow)
//                    continue;

//                // Проверяем, существует ли уже такая запись
//                var existingData = await _dbContext.IBMWeatherData
//                    .FirstOrDefaultAsync(w => 
//                        w.IBMMeteoStationId == station.Id && 
//                        w.ValidTimeUtc == validTimeUtc &&
//                        w.GridpointId == hodItem.GridpointId);

//                if (existingData != null && !forceUpdate)
//                    continue;

//                var weatherData = new IBMWeatherData
//                {
//                    Id = Guid.NewGuid(),
//                    IBMMeteoStationId = station.Id,
//                    ValidTimeUtc = validTimeUtc,
//                    ValidTimeLocal = validTimeUtc.ToLocalTime(),
//                    Temperature = (int)hodItem.Temperature,
//                    TemperatureDewPoint = hodItem.TemperatureDewPoint,
//                    Evapotranspiration = hodItem.Evapotranspiration,
//                    UvIndex = hodItem.UvIndex,
//                    Precip1Hour = hodItem.Precip1Hour,
//                    WindSpeed = (int)hodItem.WindSpeed,
//                    WindDirection = (int)hodItem.WindDirection,
//                    RequestedLatitude = hodItem.RequestedLatitude,
//                    RequestedLongitude = hodItem.RequestedLongitude,
//                    GridpointId = hodItem.GridpointId,
//                    CreateAt = DateTime.UtcNow,
//                    Status = Masofa.Common.Models.StatusType.Active,
//                    LastUpdateAt = DateTime.UtcNow
//                };

//                if (existingData != null)
//                {
//                    // Обновляем существующую запись
//                    existingData.Temperature = weatherData.Temperature;
//                    existingData.TemperatureDewPoint = weatherData.TemperatureDewPoint;
//                    existingData.Evapotranspiration = weatherData.Evapotranspiration;
//                    existingData.UvIndex = weatherData.UvIndex;
//                    existingData.Precip1Hour = weatherData.Precip1Hour;
//                    existingData.WindSpeed = weatherData.WindSpeed;
//                    existingData.WindDirection = weatherData.WindDirection;
//                    existingData.LastUpdateAt = DateTime.UtcNow;
//                }
//                else
//                {
//                    _dbContext.IBMWeatherData.Add(weatherData);
//                }

//                processedCount++;
//            }

//            if (processedCount > 0)
//            {
//                await SaveChangesAsync();
//            }

//            return processedCount;
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadHodDataForStation", ex, $"StationId: {station.Id}");
//            return 0;
//        }
//    }
//}
