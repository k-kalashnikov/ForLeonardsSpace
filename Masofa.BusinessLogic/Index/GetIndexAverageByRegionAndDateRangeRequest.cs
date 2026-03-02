using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Index
{
    /// <summary>
    /// Запрос для получения среднего значения индекса по региону за период времени.
    /// Возвращает Dictionary где ключ - дата, значение - средний индекс по ВСЕМ культурам в регионе.
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetIndexAverageByRegionAndDateRangeRequest<TIndexSharedReport> : IRequest<Dictionary<DateOnly, double>>
        where TIndexSharedReport : IndexReportShared
    {
        /// <summary>
        /// Начальная дата периода
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Конечная дата периода
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Идентификатор региона (nullable - если null, то по всем регионам)
        /// </summary>
        public Guid? RegionId { get; set; }
    }

    public class GetIndexAverageByRegionAndDateRangeRequestHandler<TIndexSharedReport> : IRequestHandler<GetIndexAverageByRegionAndDateRangeRequest<TIndexSharedReport>, Dictionary<DateOnly, double>>
        where TIndexSharedReport : IndexReportShared
    {
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public GetIndexAverageByRegionAndDateRangeRequestHandler(
            ILogger<GetIndexAverageByRegionAndDateRangeRequestHandler<TIndexSharedReport>> logger,
            IBusinessLogicLogger businessLogicLogger,
            MasofaIndicesDbContext masofaIndicesDbContext)
        {
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
            MasofaIndicesDbContext = masofaIndicesDbContext;
        }

        public async Task<Dictionary<DateOnly, double>> Handle(GetIndexAverageByRegionAndDateRangeRequest<TIndexSharedReport> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(
                    $"Start GetIndexAverageByRegionAndDateRangeRequest: StartDate={request.StartDate}, EndDate={request.EndDate}, RegionId={request.RegionId}",
                    requestPath);

                // Базовый запрос к таблице SharedReports
                IQueryable<TIndexSharedReport> query = MasofaIndicesDbContext.Set<TIndexSharedReport>().AsNoTracking();

                // Фильтр по дате
                query = query.Where(r => r.DateOnly >= request.StartDate && r.DateOnly <= request.EndDate);

                // Фильтр по региону (если указан)
                if (request.RegionId.HasValue)
                {
                    query = query.Where(r => r.RegionId == request.RegionId.Value);
                }

                // Группируем по дате и вычисляем среднее значение Average по всем культурам в регионе
                var result = await query
                    .GroupBy(r => r.DateOnly)
                    .Select(g => new
                    {
                        Date = g.Key,
                        AverageIndex = g.Average(r => r.Average)
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync(cancellationToken);

                var dictionary = result.ToDictionary(x => x.Date, x => x.AverageIndex);

                await BusinessLogicLogger.LogInformationAsync(
                    $"Finish GetIndexAverageByRegionAndDateRangeRequest: Found {dictionary.Count} dates",
                    requestPath);

                return dictionary;
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

