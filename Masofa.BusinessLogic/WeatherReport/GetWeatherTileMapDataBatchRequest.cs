using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Era;
using Masofa.Common.Models.IBMWeather;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.WeatherReport
{
    /// <summary>
    /// Запрос для массового получения данных плиточной карты погоды для нескольких регионов
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetWeatherTileMapDataBatchRequest : IRequest<List<WeatherTileMapDataResponse>>
    {
        /// <summary>
        /// Список идентификаторов RegionMap (плиток)
        /// </summary>
        public List<Guid> RegionMapIds { get; set; } = new();

        /// <summary>
        /// Тип данных:
        /// 1 - Уведомления от УГМ о стихийных бедствиях
        /// 2 - Отклонения от температурных норм
        /// 3 - Отклонения от осадочных норм
        /// 4 - Карта заморозков
        /// 5 - Карта неблагоприятных условий для культур
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Начальная дата (для типа 1 - период)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Конечная дата (для типа 1 - период)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Дата (для типов 2-5)
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Идентификатор культуры (для типа 5)
        /// </summary>
        public Guid? CropId { get; set; }
    }

    /// <summary>
    /// Ответ с данными для плиточки
    /// </summary>
    public class WeatherTileMapDataResponse
    {
        /// <summary>
        /// Идентификатор RegionMap
        /// </summary>
        public Guid RegionMapId { get; set; }

        /// <summary>
        /// Цвет плитки: green, yellow, red, blue, gray
        /// </summary>
        public string Color { get; set; } = "gray";

        /// <summary>
        /// Данные в зависимости от типа
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Сообщение об ошибке или дополнительная информация
        /// </summary>
        public string? Message { get; set; }
    }

    /// <summary>
    /// Данные для типа 1 - Уведомления от УГМ
    /// </summary>
    public class DisasterAlertsData
    {
        public int AlertCount { get; set; }
        public bool HasAlerts { get; set; }
        public string AlertLevel { get; set; } = "none"; // none, low, medium, high
    }

    /// <summary>
    /// Данные для типа 2 - Отклонения от температурных норм
    /// </summary>
    public class TemperatureDeviationData
    {
        public double Actual { get; set; }
        public double Average { get; set; }
        public double Deviation { get; set; }
    }

    /// <summary>
    /// Данные для типа 3 - Отклонения от осадочных норм
    /// </summary>
    public class PrecipitationDeviationData
    {
        public double Actual { get; set; }
        public double Average { get; set; }
        public double Deviation { get; set; }
    }

    /// <summary>
    /// Данные для типа 4 - Карта заморозков
    /// </summary>
    public class FrostData
    {
        public double MinTemperature { get; set; }
        public bool HasFrost { get; set; }
    }

    /// <summary>
    /// Данные для типа 5 - Карта неблагоприятных условий для культур
    /// </summary>
    public class CropConditionData
    {
        public double ActualTemperature { get; set; }
        public double OptimalTemperature { get; set; }
        public double Deviation { get; set; }
        public bool IsOptimal { get; set; }
    }

    public class GetWeatherTileMapDataBatchRequestHandler : IRequestHandler<GetWeatherTileMapDataBatchRequest, List<WeatherTileMapDataResponse>>
    {
        private readonly MasofaEraDbContext _eraDbContext;
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly MasofaIBMWeatherDbContext _ibmWeatherDbContext;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly ILogger<GetWeatherTileMapDataBatchRequestHandler> _logger;

        public GetWeatherTileMapDataBatchRequestHandler(
            MasofaEraDbContext eraDbContext,
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaIBMWeatherDbContext ibmWeatherDbContext,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<GetWeatherTileMapDataBatchRequestHandler> logger)
        {
            _eraDbContext = eraDbContext;
            _dictionariesDbContext = dictionariesDbContext;
            _ibmWeatherDbContext = ibmWeatherDbContext;
            _businessLogicLogger = businessLogicLogger;
            _logger = logger;
        }

        public async Task<List<WeatherTileMapDataResponse>> Handle(GetWeatherTileMapDataBatchRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(
                    $"Start GetWeatherTileMapDataBatchRequest: {request.RegionMapIds.Count} regions, DataType={request.DataType}",
                    requestPath);

                if (request.RegionMapIds == null || request.RegionMapIds.Count == 0)
                {
                    await _businessLogicLogger.LogErrorAsync("RegionMapIds list is empty", requestPath);
                    return new List<WeatherTileMapDataResponse>();
                }

                // Получаем все RegionMaps за один запрос
                var regionMaps = await _dictionariesDbContext.RegionMaps
                    .AsNoTracking()
                    .Where(rm => request.RegionMapIds.Contains(rm.Id) && rm.Status == StatusType.Active)
                    .ToListAsync(cancellationToken);

                if (regionMaps.Count == 0)
                {
                    await _businessLogicLogger.LogWarningAsync("No valid RegionMaps found", requestPath);
                    return request.RegionMapIds.Select(id => new WeatherTileMapDataResponse
                    {
                        RegionMapId = id,
                        Color = "gray",
                        Message = "RegionMap not found"
                    }).ToList();
                }

                var results = new List<WeatherTileMapDataResponse>();

                // Обрабатываем каждый регион
                foreach (var regionMap in regionMaps.Where(rm => rm.Polygon != null))
                {
                    try
                    {
                        var regionPolygonWkt = regionMap.Polygon!.AsText();

                        // Получаем все EraWeatherStations внутри полигона RegionMap
                        var stationIds = await _eraDbContext.EraWeatherStations
                            .FromSqlRaw(@"
                                SELECT DISTINCT s.*
                                FROM ""EraWeatherStations"" s
                                WHERE ST_Contains(
                                    ST_SetSRID(ST_GeomFromText(@polygonWkt), 4326),
                                    ST_SetSRID(s.""Point""::geometry, 4326)
                                )",
                                new NpgsqlParameter("@polygonWkt", regionPolygonWkt))
                            .Select(s => s.Id)
                            .ToListAsync(cancellationToken);

                        if (stationIds.Count == 0)
                        {
                            // Если станций нет внутри полигона, ищем ближайшие
                            var centerPoint = regionMap.Polygon.Centroid;
                            var nearestStations = await _eraDbContext.EraWeatherStations
                                .OrderBy(s => s.Point.Distance(centerPoint))
                                .Take(5)
                                .Select(s => s.Id)
                                .ToListAsync(cancellationToken);

                            stationIds = nearestStations;
                        }

                        // Обрабатываем в зависимости от типа данных
                        WeatherTileMapDataResponse result = request.DataType switch
                        {
                            1 => await GetDisasterAlertsData(request, regionMap.Id, stationIds, regionPolygonWkt, requestPath, cancellationToken),
                            2 => await GetTemperatureDeviationData(request, regionMap.Id, stationIds, requestPath, cancellationToken),
                            3 => await GetPrecipitationDeviationData(request, regionMap.Id, stationIds, requestPath, cancellationToken),
                            4 => await GetFrostData(request, regionMap.Id, stationIds, requestPath, cancellationToken),
                            5 => await GetCropConditionData(request, regionMap.Id, stationIds, requestPath, cancellationToken),
                            _ => new WeatherTileMapDataResponse
                            {
                                RegionMapId = regionMap.Id,
                                Color = "gray",
                                Message = "Unknown data type"
                            }
                        };

                        results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        await _businessLogicLogger.LogErrorAsync(
                            $"Error processing region {regionMap.Id}: {ex.Message}",
                            requestPath);
                        
                        results.Add(new WeatherTileMapDataResponse
                        {
                            RegionMapId = regionMap.Id,
                            Color = "gray",
                            Message = $"Error: {ex.Message}"
                        });
                    }
                }

                // Добавляем записи для регионов без полигонов или не найденных
                var processedIds = results.Select(r => r.RegionMapId).ToHashSet();
                foreach (var regionMapId in request.RegionMapIds)
                {
                    if (!processedIds.Contains(regionMapId))
                    {
                        results.Add(new WeatherTileMapDataResponse
                        {
                            RegionMapId = regionMapId,
                            Color = "gray",
                            Message = "RegionMap not found or has no polygon"
                        });
                    }
                }

                await _businessLogicLogger.LogInformationAsync(
                    $"Completed GetWeatherTileMapDataBatchRequest: {results.Count} results",
                    requestPath);

                return results;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        private async Task<WeatherTileMapDataResponse> GetDisasterAlertsData(
            GetWeatherTileMapDataBatchRequest request,
            Guid regionMapId,
            List<Guid> stationIds,
            string regionPolygonWkt,
            string requestPath,
            CancellationToken cancellationToken)
        {
            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "StartDate and EndDate are required for disaster alerts"
                };
            }

            var startDate = DateOnly.FromDateTime(request.StartDate.Value);
            var endDate = DateOnly.FromDateTime(request.EndDate.Value);

            // Ищем алерты IBM Weather в этом регионе за период
            var alertCount = await _ibmWeatherDbContext.IBMWeatherAlerts
                .FromSqlRaw(@"
                    SELECT a.*
                    FROM ""IBMWeatherAlerts"" a
                    WHERE a.""Latitude"" IS NOT NULL 
                      AND a.""Longitude"" IS NOT NULL
                      AND ST_Contains(
                          ST_SetSRID(ST_GeomFromText(@polygonWkt), 4326),
                          ST_SetSRID(ST_MakePoint(a.""Longitude"", a.""Latitude""), 4326)
                      )
                      AND a.""IssueTimeLocal"" >= @startDate
                      AND a.""ExpireTimeLocal"" <= @endDate
                      AND a.""MessageType"" != 'Cancel'",
                    new NpgsqlParameter("@polygonWkt", regionPolygonWkt),
                    new NpgsqlParameter("@startDate", request.StartDate.Value),
                    new NpgsqlParameter("@endDate", request.EndDate.Value))
                .CountAsync(cancellationToken);

            var alertLevel = alertCount switch
            {
                0 => "none",
                1 => "low",
                2 => "medium",
                _ => "high"
            };

            var color = alertLevel switch
            {
                "none" => "green",
                "low" => "yellow",
                "medium" => "red",
                "high" => "red",
                _ => "gray"
            };

            return new WeatherTileMapDataResponse
            {
                RegionMapId = regionMapId,
                Color = color,
                Data = new DisasterAlertsData
                {
                    AlertCount = alertCount,
                    HasAlerts = alertCount > 0,
                    AlertLevel = alertLevel
                }
            };
        }

        private async Task<WeatherTileMapDataResponse> GetTemperatureDeviationData(
            GetWeatherTileMapDataBatchRequest request,
            Guid regionMapId,
            List<Guid> stationIds,
            string requestPath,
            CancellationToken cancellationToken)
        {
            if (!request.Date.HasValue)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "Date is required for temperature deviation"
                };
            }

            var targetDate = DateOnly.FromDateTime(request.Date.Value);

            // Получаем текущие данные
            var currentData = await _eraDbContext.Era5DayWeatherReports
                .Where(r => stationIds.Contains(r.WeatherStation!.Value) && r.Date == targetDate)
                .ToListAsync(cancellationToken);

            if (currentData.Count == 0)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "No weather data available for this date"
                };
            }

            var currentTemp = currentData.Average(r => r.TemperatureAverage);

            // Вычисляем среднюю температуру за тот же месяц за последние 5 лет
            var historicalData = await _eraDbContext.Era5DayWeatherReports
                .Where(r => stationIds.Contains(r.WeatherStation!.Value) &&
                           r.Date.Month == targetDate.Month &&
                           r.Date.Year >= targetDate.Year - 5 &&
                           r.Date.Year < targetDate.Year)
                .ToListAsync(cancellationToken);

            var averageTemp = historicalData.Count > 0
                ? historicalData.Average(r => r.TemperatureAverage)
                : currentTemp; // Если нет исторических данных, используем текущую

            var deviation = currentTemp - averageTemp;

            var color = Math.Abs(deviation) <= 3
                ? "green"
                : deviation > 3
                    ? "red"
                    : "blue"; // Ниже нормы

            return new WeatherTileMapDataResponse
            {
                RegionMapId = regionMapId,
                Color = color,
                Data = new TemperatureDeviationData
                {
                    Actual = Math.Round(currentTemp, 2),
                    Average = Math.Round(averageTemp, 2),
                    Deviation = Math.Round(deviation, 2)
                }
            };
        }

        private async Task<WeatherTileMapDataResponse> GetPrecipitationDeviationData(
            GetWeatherTileMapDataBatchRequest request,
            Guid regionMapId,
            List<Guid> stationIds,
            string requestPath,
            CancellationToken cancellationToken)
        {
            if (!request.Date.HasValue)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "Date is required for precipitation deviation"
                };
            }

            var targetDate = DateOnly.FromDateTime(request.Date.Value);

            // Получаем текущие данные
            var currentData = await _eraDbContext.Era5DayWeatherReports
                .Where(r => stationIds.Contains(r.WeatherStation!.Value) && r.Date == targetDate)
                .ToListAsync(cancellationToken);

            if (currentData.Count == 0)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "No weather data available for this date"
                };
            }

            var currentPrecip = currentData.Average(r => r.Fallout);

            // Вычисляем средние осадки за тот же месяц за последние 5 лет
            var historicalData = await _eraDbContext.Era5DayWeatherReports
                .Where(r => stationIds.Contains(r.WeatherStation!.Value) &&
                           r.Date.Month == targetDate.Month &&
                           r.Date.Year >= targetDate.Year - 5 &&
                           r.Date.Year < targetDate.Year)
                .ToListAsync(cancellationToken);

            var averagePrecip = historicalData.Count > 0
                ? historicalData.Average(r => r.Fallout)
                : currentPrecip;

            var deviation = currentPrecip - averagePrecip;

            var color = Math.Abs(deviation) <= 5
                ? "green"
                : deviation > 5
                    ? "red"
                    : "blue"; // Ниже нормы

            return new WeatherTileMapDataResponse
            {
                RegionMapId = regionMapId,
                Color = color,
                Data = new PrecipitationDeviationData
                {
                    Actual = Math.Round(currentPrecip, 2),
                    Average = Math.Round(averagePrecip, 2),
                    Deviation = Math.Round(deviation, 2)
                }
            };
        }

        private async Task<WeatherTileMapDataResponse> GetFrostData(
            GetWeatherTileMapDataBatchRequest request,
            Guid regionMapId,
            List<Guid> stationIds,
            string requestPath,
            CancellationToken cancellationToken)
        {
            if (!request.Date.HasValue)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "Date is required for frost data"
                };
            }

            var targetDate = DateOnly.FromDateTime(request.Date.Value);

            // Получаем данные
            var weatherData = await _eraDbContext.Era5DayWeatherReports
                .Where(r => stationIds.Contains(r.WeatherStation!.Value) && r.Date == targetDate)
                .ToListAsync(cancellationToken);

            if (weatherData.Count == 0)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "No weather data available for this date"
                };
            }

            var minTemp = weatherData.Min(r => r.TemperatureMinTotal);
            // Бизнес-требование: заморозки начинаются при -5°C
            var hasFrost = minTemp < -5;

            return new WeatherTileMapDataResponse
            {
                RegionMapId = regionMapId,
                Color = hasFrost ? "blue" : "green",
                Data = new FrostData
                {
                    MinTemperature = Math.Round(minTemp, 2),
                    HasFrost = hasFrost
                }
            };
        }

        private async Task<WeatherTileMapDataResponse> GetCropConditionData(
            GetWeatherTileMapDataBatchRequest request,
            Guid regionMapId,
            List<Guid> stationIds,
            string requestPath,
            CancellationToken cancellationToken)
        {
            if (!request.Date.HasValue || !request.CropId.HasValue)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "Date and CropId are required for crop condition data"
                };
            }

            var targetDate = DateOnly.FromDateTime(request.Date.Value);
            var cropId = request.CropId.Value;

            // Получаем текущие данные
            var currentData = await _eraDbContext.Era5DayWeatherReports
                .Where(r => stationIds.Contains(r.WeatherStation!.Value) && r.Date == targetDate)
                .ToListAsync(cancellationToken);

            if (currentData.Count == 0)
            {
                return new WeatherTileMapDataResponse
                {
                    RegionMapId = regionMapId,
                    Color = "gray",
                    Message = "No weather data available for this date"
                };
            }

            var currentTemp = currentData.Average(r => r.TemperatureAverage);

            // Получаем оптимальную температуру для культуры (можно добавить в справочник культур)
            var optimalTemp = GetOptimalTemperatureForCrop(cropId);

            var deviation = Math.Abs(currentTemp - optimalTemp);

            var color = deviation <= 3
                ? "green"
                : deviation <= 7
                    ? "blue"
                    : "red";

            return new WeatherTileMapDataResponse
            {
                RegionMapId = regionMapId,
                Color = color,
                Data = new CropConditionData
                {
                    ActualTemperature = Math.Round(currentTemp, 2),
                    OptimalTemperature = optimalTemp,
                    Deviation = Math.Round(deviation, 2),
                    IsOptimal = deviation <= 3
                }
            };
        }

        /// <summary>
        /// Получает оптимальную температуру для культуры (можно вынести в справочник)
        /// </summary>
        private double GetOptimalTemperatureForCrop(Guid cropId)
        {
            // TODO: Получать из справочника культур
            // Пока возвращаем значения по умолчанию
            return 25.0; // Средняя оптимальная температура по умолчанию
        }
    }
}

