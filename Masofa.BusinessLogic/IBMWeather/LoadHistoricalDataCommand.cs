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
/// Команда для загрузки исторических данных IBM Weather
/// </summary>
public class LoadHistoricalDataCommand : BaseIBMWeatherLoadCommand
{
    /// <summary>
    /// Идентификатор метеостанции
    /// </summary>
    public Guid IBMMeteoStationId { get; set; }

    /// <summary>
    /// Дата начала периода
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Дата окончания периода
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Загружать ли дневные сводки (30 дней)
    /// </summary>
    public bool LoadDailySummary { get; set; } = true;

    /// <summary>
    /// Загружать ли почасовые данные (1 день)
    /// </summary>
    public bool LoadHourlyData { get; set; } = true;

    public LoadHistoricalDataCommand(
        Guid ibmMeteoStationId,
        DateTime startDate,
        DateTime endDate,
        bool loadDailySummary = true,
        bool loadHourlyData = true)
    {
        IBMMeteoStationId = ibmMeteoStationId;
        StartDate = startDate;
        EndDate = endDate;
        LoadDailySummary = loadDailySummary;
        LoadHourlyData = loadHourlyData;
    }
}

/// <summary>
/// Обработчик команды для загрузки исторических данных
/// </summary>
//public class LoadHistoricalDataHandler : BaseIBMWeatherLoadHandler<LoadHistoricalDataCommand>
//{
//    public LoadHistoricalDataHandler(
//    IBMWeatherApiUnitOfWork unitOfWork,
//    MasofaIBMWeatherDbContext dbContext,
//    IBusinessLogicLogger logger,
//    IConfiguration configuration) : base(unitOfWork, dbContext, logger, configuration)
//    {
//    }

//    public override async Task Handle(LoadHistoricalDataCommand request, CancellationToken cancellationToken)
//    {
//        await LogOperationStartAsync("LoadHistoricalData", 
//            $"StationId: {request.IBMMeteoStationId}, Period: {request.StartDate:yyyy-MM-dd} - {request.EndDate:yyyy-MM-dd}");

//        try
//        {
//            // Получаем информацию о станции
//            var station = await _dbContext.IBMMeteoStations
//                .FirstOrDefaultAsync(s => s.Id == request.IBMMeteoStationId);

//            if (station == null)
//            {
//                throw new ArgumentException($"Метеостанция с ID {request.IBMMeteoStationId} не найдена");
//            }

//            var processedCount = 0;

//            // Загружаем дневные сводки за 30 дней
//            if (request.LoadDailySummary)
//            {
//                var dailyCount = await LoadDailySummaryDataAsync(station, request);
//                processedCount += dailyCount;
//            }

//            // Загружаем почасовые данные за 1 день
//            if (request.LoadHourlyData)
//            {
//                var hourlyCount = await LoadHourlyDataAsync(station, request);
//                processedCount += hourlyCount;
//            }

//            await LogOperationEndAsync("LoadHistoricalData", processedCount, "записей");
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadHistoricalData", ex);
//            throw;
//        }
//    }

//    /// <summary>
//    /// Загрузка дневных сводок за 30 дней
//    /// </summary>
//    private async Task<int> LoadDailySummaryDataAsync(IBMMeteoStation station, LoadHistoricalDataCommand request)
//    {
//        var dailyResponse = await _unitOfWork.IBMWeatherRepository.GetHistoricalDailySummaryAsync(
//            station.Point.Y, station.Point.X);

//        var processedCount = 0;

//        foreach (var item in dailyResponse)
//        {
//            var data = item.Data;
            
//            // Обрабатываем массивы данных (каждый элемент - отдельный день)
//            for (int i = 0; i < data.ValidTimeLocal.Count; i++)
//            {
//                var validTimeLocal = DateTime.Parse(data.ValidTimeLocal[i]);
                
//                // Проверяем, не выходит ли дата за пределы запрошенного периода
//                if (validTimeLocal < request.StartDate || validTimeLocal > request.EndDate)
//                    continue;

//                // Проверяем, существует ли уже такая запись
//                var existingData = await _dbContext.IBMWeatherData
//                    .FirstOrDefaultAsync(w => 
//                        w.IBMMeteoStationId == station.Id && 
//                        w.ValidTimeLocal.Date == validTimeLocal.Date &&
//                        w.TemperatureMax.HasValue); // Признак дневных данных

//                if (existingData != null)
//                    continue; // Данные уже существуют

//                var weatherData = new IBMWeatherData
//                {
//                    Id = Guid.NewGuid(),
//                    IBMMeteoStationId = station.Id,
//                    ValidTimeUtc = validTimeLocal.ToUniversalTime(),
//                    ValidTimeLocal = validTimeLocal,
//                    TemperatureMax = data.TemperatureMax.Count > i ? data.TemperatureMax[i] : null,
//                    TemperatureMin = data.TemperatureMin.Count > i ? data.TemperatureMin[i] : null,
//                    Precip24Hour = data.Precip24Hour.Count > i ? data.Precip24Hour[i] : null,
//                    Rain24Hour = data.Rain24Hour.Count > i ? data.Rain24Hour[i] : null,
//                    Snow24Hour = data.Snow24Hour.Count > i ? data.Snow24Hour[i] : null,
//                    IconCodeDay = data.IconCodeDay.Count > i ? data.IconCodeDay[i] : null,
//                    IconCodeNight = data.IconCodeNight.Count > i ? data.IconCodeNight[i] : null,
//                    WxPhraseLongDay = data.WxPhraseLongDay.Count > i ? data.WxPhraseLongDay[i] : null,
//                    WxPhraseLongNight = data.WxPhraseLongNight.Count > i ? data.WxPhraseLongNight[i] : null,
//                    CreateAt = DateTime.UtcNow,
//                    Status = Masofa.Common.Models.StatusType.Active,
//                    LastUpdateAt = DateTime.UtcNow,
//                    CreateUser = Guid.Empty, // TODO: Получить из контекста
//                    LastUpdateUser = Guid.Empty, // TODO: Получить из контекста
//                    Names = new Masofa.Common.Models.LocalizationString { ValuesJson = $"Historical daily data for {validTimeLocal:yyyy-MM-dd}" }
//                };

//                _dbContext.IBMWeatherData.Add(weatherData);
//                processedCount++;
//            }
//        }

//        if (processedCount > 0)
//        {
//            await SaveChangesAsync();
//        }

//        return processedCount;
//    }

//    /// <summary>
//    /// Загрузка почасовых данных за 1 день
//    /// </summary>
//    private async Task<int> LoadHourlyDataAsync(IBMMeteoStation station, LoadHistoricalDataCommand request)
//    {
//        var hourlyResponse = await _unitOfWork.IBMWeatherRepository.GetHistoricalHourlyAsync(
//            station.Point.Y, station.Point.X);

//        var processedCount = 0;

//        foreach (var item in hourlyResponse)
//        {
//            var data = item.Data;
            
//            // Обрабатываем массивы данных (каждый элемент - отдельный час)
//            for (int i = 0; i < data.ValidTimeLocal.Count; i++)
//            {
//                var validTimeLocal = DateTime.Parse(data.ValidTimeLocal[i]);
                
//                // Проверяем, не выходит ли дата за пределы запрошенного периода
//                if (validTimeLocal < request.StartDate || validTimeLocal > request.EndDate)
//                    continue;

//                // Проверяем, существует ли уже такая запись
//                var existingData = await _dbContext.IBMWeatherData
//                    .FirstOrDefaultAsync(w => 
//                        w.IBMMeteoStationId == station.Id && 
//                        w.ValidTimeLocal == validTimeLocal &&
//                        w.Temperature.HasValue); // Признак почасовых данных

//                if (existingData != null)
//                    continue; // Данные уже существуют

//                var weatherData = new IBMWeatherData
//                {
//                    Id = Guid.NewGuid(),
//                    IBMMeteoStationId = station.Id,
//                    ValidTimeUtc = validTimeLocal.ToUniversalTime(),
//                    ValidTimeLocal = validTimeLocal,
//                    Temperature = data.Temperature.Count > i ? data.Temperature[i] : null,
//                    RelativeHumidity = data.RelativeHumidity.Count > i ? data.RelativeHumidity[i] : null,
//                    PressureAltimeter = data.PressureAltimeter.Count > i ? data.PressureAltimeter[i] : null,
//                    WindSpeed = data.WindSpeed.Count > i ? data.WindSpeed[i] : null,
//                    WindDirection = data.WindDirection.Count > i ? data.WindDirection[i] : null,
//                    Precip24Hour = data.Precip24Hour.Count > i ? data.Precip24Hour[i] : null,
//                    Snow24Hour = data.Snow24Hour.Count > i ? data.Snow24Hour[i] : null,
//                    CloudCeiling = data.CloudCeiling.Count > i ? data.CloudCeiling[i] : null,
//                    UvIndex = data.UvIndex.Count > i ? data.UvIndex[i] : null,
//                    UvDescription = data.UvDescription.Count > i ? data.UvDescription[i] : null,
//                    IconCode = data.IconCode.Count > i ? data.IconCode[i] : null,
//                    WxPhraseLong = data.WxPhraseLong.Count > i ? data.WxPhraseLong[i] : null,
//                    DayOrNight = data.DayOrNight.Count > i ? data.DayOrNight[i] : null,
//                    SunriseTimeLocal = data.SunriseTimeLocal.Count > i ? DateTime.Parse(data.SunriseTimeLocal[i]) : null,
//                    SunsetTimeLocal = data.SunsetTimeLocal.Count > i ? DateTime.Parse(data.SunsetTimeLocal[i]) : null,
//                    CreateAt = DateTime.UtcNow,
//                    Status = Masofa.Common.Models.StatusType.Active,
//                    LastUpdateAt = DateTime.UtcNow,
//                    CreateUser = Guid.Empty, // TODO: Получить из контекста
//                    LastUpdateUser = Guid.Empty, // TODO: Получить из контекста
//                    Names = new Masofa.Common.Models.LocalizationString { ValuesJson = $"Historical hourly data for {validTimeLocal:yyyy-MM-dd HH:mm}" }
//                };

//                _dbContext.IBMWeatherData.Add(weatherData);
//                processedCount++;
//            }
//        }

//        if (processedCount > 0)
//        {
//            await SaveChangesAsync();
//        }

//        return processedCount;
//    }
//}
