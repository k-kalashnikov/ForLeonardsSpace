using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Era;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Dictionaries
{
    /// <summary>
    /// DTO для агроклиматической зоны с расчетными полями
    /// </summary>
    public class AgroclimaticZoneDto : NamedDictionaryItem
    {
        /// <summary>
        /// Осадки за год (мм)
        /// </summary>
        public decimal? AnnualPrecipitation { get; set; }

        /// <summary>
        /// Средняя скорость ветра за год (м/с)
        /// </summary>
        public decimal? AverageWindSpeed { get; set; }

        /// <summary>
        /// Средняя температура за год (°C)
        /// </summary>
        public decimal? AverageTemperature { get; set; }
    }

    /// <summary>
    /// Запрос для API получения агроклиматических зон с расчетными полями
    /// </summary>
    public class AgroclimaticZoneQueryRequest : BaseGetQuery<AgroclimaticZone>
    {
        /// <summary>
        /// Год для расчета данных (если не указан, используется текущий год)
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Месяц для фильтрации по дню в году (используется вместе с Day)
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// День для фильтрации по дню в году (используется вместе с Month)
        /// </summary>
        public int? Day { get; set; }
    }

    /// <summary>
    /// Запрос для получения агроклиматических зон с расчетными полями (для MediatR)
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class AgroclimaticZoneGetByQueryRequest : IRequest<List<AgroclimaticZoneDto>>
    {
        /// <summary>
        /// Базовый запрос с фильтрами, сортировкой и пагинацией
        /// </summary>
        public BaseGetQuery<AgroclimaticZone> Query { get; set; }

        /// <summary>
        /// Год для расчета данных (если не указан, используется текущий год)
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Месяц для фильтрации по дню в году (используется вместе с Day)
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// День для фильтрации по дню в году (используется вместе с Month)
        /// </summary>
        public int? Day { get; set; }
    }

    /// <summary>
    /// Обработчик запроса на получение агроклиматических зон с расчетными полями
    /// </summary>
    public class AgroclimaticZoneGetByQueryRequestHandler : IRequestHandler<AgroclimaticZoneGetByQueryRequest, List<AgroclimaticZoneDto>>
    {
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly MasofaEraDbContext _eraDbContext;
        private readonly ILogger<AgroclimaticZoneGetByQueryRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public AgroclimaticZoneGetByQueryRequestHandler(
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaEraDbContext eraDbContext,
            ILogger<AgroclimaticZoneGetByQueryRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dictionariesDbContext = dictionariesDbContext;
            _eraDbContext = eraDbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<AgroclimaticZoneDto>> Handle(AgroclimaticZoneGetByQueryRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Извлекаем month и day из фильтров, если есть фильтр dayOfYear
                int? month = request.Month;
                int? day = request.Day;

                var dayOfYearFilter = request.Query.Filters?.FirstOrDefault(f =>
                    f.FilterField?.Equals("dayOfYear", StringComparison.OrdinalIgnoreCase) == true);

                if (dayOfYearFilter != null && dayOfYearFilter.FilterValue != null)
                {
                    // Пытаемся извлечь дату из фильтра
                    if (dayOfYearFilter.FilterValue is DateTime dateTime)
                    {
                        month = dateTime.Month;
                        day = dateTime.Day;
                    }
                    else if (dayOfYearFilter.FilterValue is string dateString &&
                             DateTime.TryParse(dateString, out var parsedDate))
                    {
                        month = parsedDate.Month;
                        day = parsedDate.Day;
                    }
                }

                // Удаляем фильтр dayOfYear из запроса, так как он не относится к модели AgroclimaticZone
                var filtersWithoutDayOfYear = request.Query.Filters?
                    .Where(f => !f.FilterField?.Equals("dayOfYear", StringComparison.OrdinalIgnoreCase) == true)
                    .ToList() ?? new List<FieldFilter>();

                // Получаем базовые зоны
                IQueryable<AgroclimaticZone> resultQuery = _dictionariesDbContext
                    .Set<AgroclimaticZone>()
                    .AsNoTracking()
                    .Where(m => m.Status == StatusType.Active);

                // Применяем фильтры
                if (filtersWithoutDayOfYear.Any())
                {
                    foreach (var item in filtersWithoutDayOfYear)
                    {
                        resultQuery = resultQuery.ApplyFiltering(item);
                    }
                }

                // Применяем сортировку
                if (!string.IsNullOrEmpty(request.Query.SortBy))
                {
                    resultQuery = resultQuery.ApplyOrdering(request.Query.SortBy, request.Query.Sort);
                }

                // Применяем пагинацию
                if (request.Query.Take.HasValue)
                {
                    resultQuery = resultQuery
                        .Skip(request.Query.Offset)
                        .Take(request.Query.Take.Value);
                }

                // Получаем список зон
                var zones = await resultQuery.ToListAsync(cancellationToken);

                // Рассчитываем средние значения для каждой зоны
                var result = new List<AgroclimaticZoneDto>();
                foreach (var zone in zones)
                {
                    var dto = await CalculateWeatherData(zone, request.Year, month, day, cancellationToken);
                    result.Add(dto);
                }

                await _businessLogicLogger.LogInformationAsync(
                    LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        /// <summary>
        /// Рассчитывает средние значения метеоданных для агроклиматической зоны
        /// </summary>
        private async Task<AgroclimaticZoneDto> CalculateWeatherData(
            AgroclimaticZone zone,
            int? year,
            int? month,
            int? day,
            CancellationToken cancellationToken)
        {
            // 1. Найти все EraWeatherStation для зоны
            var stationIds = await _eraDbContext.EraWeatherStations
                .Where(s => s.AgroclimaticZoneId == zone.Id)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            var dto = new AgroclimaticZoneDto
            {
                Id = zone.Id,
                Names = zone.Names,
                ExtData = zone.ExtData,
                Comment = zone.Comment,
                Visible = zone.Visible,
                IsPublic = zone.IsPublic,
                OrderCode = zone.OrderCode,
                CreateAt = zone.CreateAt,
                LastUpdateAt = zone.LastUpdateAt,
                CreateUser = zone.CreateUser,
                LastUpdateUser = zone.LastUpdateUser,
                Status = zone.Status
            };

            if (!stationIds.Any())
            {
                return dto;
            }

            // 2. Выбрать источник данных
            IQueryable<BaseEra5WeatherReport> reports;

            if (month.HasValue && day.HasValue)
            {
                // Использовать Era5DayNormalizedWeather
                reports = _eraDbContext.Era5DayNormalizedWeather
                    .Where(r => r.WeatherStation.HasValue &&
                               stationIds.Contains(r.WeatherStation.Value) &&
                               r.Month == month.Value &&
                               r.Day == day.Value)
                    .Cast<BaseEra5WeatherReport>();
            }
            else
            {
                // Использовать Era5YearWeatherReport
                var targetYear = year ?? DateTime.UtcNow.Year;
                reports = _eraDbContext.Era5YearWeatherReports
                    .Where(r => r.WeatherStation.HasValue &&
                               stationIds.Contains(r.WeatherStation.Value) &&
                               r.Year == targetYear)
                    .Cast<BaseEra5WeatherReport>();
            }

            // 3. Рассчитать средние значения
            var reportsList = await reports.ToListAsync(cancellationToken);

            if (reportsList.Any())
            {
                // Для температуры - среднее по всем станциям
                var avgTemp = reportsList.Average(r => r.TemperatureAverage);
                dto.AverageTemperature = (decimal)avgTemp;

                // Для скорости ветра - среднее по всем станциям
                var avgWindSpeed = reportsList.Average(r => r.WindSpeed);
                dto.AverageWindSpeed = (decimal)avgWindSpeed;

                // Для осадок - сумма по всем станциям
                var totalPrecipitation = reportsList.Sum(r => r.Fallout);
                dto.AnnualPrecipitation = (decimal)totalPrecipitation;
            }

            return dto;
        }
    }
}

