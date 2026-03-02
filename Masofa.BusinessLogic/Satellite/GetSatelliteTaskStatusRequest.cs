using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Satellite
{
    /// <summary>
    /// Запрос для получения статуса задач обработки спутниковых снимков
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetSatelliteTaskStatusRequest : IRequest<GetSatelliteTaskStatusResponse>
    {
        /// <summary>
        /// Количество записей для возврата
        /// </summary>
        public int? Take { get; set; } = 20;

        /// <summary>
        /// Смещение для пагинации
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Поле для сортировки
        /// </summary>
        public string? SortBy { get; set; } = "CreateAt";

        /// <summary>
        /// Направление сортировки (-1 = desc, 1 = asc)
        /// </summary>
        public int Sort { get; set; } = -1;

        /// <summary>
        /// Фильтр по статусу задачи
        /// </summary>
        public ProductQueueStatusType? Status { get; set; }

        /// <summary>
        /// Фильтр по типу спутника
        /// </summary>
        public ProductSourceType? SatelliteType { get; set; }

        /// <summary>
        /// Фильтр по дате создания задачи (начало периода)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Фильтр по дате создания задачи (конец периода)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Фильтр по ProductId
        /// </summary>
        public string? ProductId { get; set; }
    }

    /// <summary>
    /// Ответ со статусом задач и метаданными
    /// </summary>
    public class GetSatelliteTaskStatusResponse
    {
        /// <summary>
        /// Список задач
        /// </summary>
        public List<SatelliteTaskStatusDto> Data { get; set; } = new();

        /// <summary>
        /// Общее количество записей (для пагинации)
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// DTO для статуса задачи обработки спутникового снимка
    /// </summary>
    public class SatelliteTaskStatusDto
    {
        /// <summary>
        /// ID задачи в очереди
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID продукта в источнике данных
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Название продукта
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// Тип спутника
        /// </summary>
        public ProductSourceType SatelliteType { get; set; }

        /// <summary>
        /// Текущий статус обработки
        /// </summary>
        public ProductQueueStatusType Status { get; set; }

        /// <summary>
        /// Название статуса для отображения
        /// </summary>
        public string StatusName { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания задачи
        /// </summary>
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// Дата снимка
        /// </summary>
        public DateTime? OriginDate { get; set; }

        /// <summary>
        /// Название региона
        /// </summary>
        public string? RegionName { get; set; }

        /// <summary>
        /// Прогресс обработки (в процентах)
        /// </summary>
        public int ProgressPercent { get; set; }

        /// <summary>
        /// Детализированный статус индексов
        /// </summary>
        public SatelliteIndexStatusDto? IndexStatus { get; set; }

        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Ошибки обработки (если есть)
        /// </summary>
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// DTO для детализированного статуса индексов
    /// </summary>
    public class SatelliteIndexStatusDto
    {
        /// <summary>
        /// Статус генерации TIFF файлов
        /// </summary>
        public bool IsTiffComplete { get; set; }

        /// <summary>
        /// Статус загрузки в базу данных
        /// </summary>
        public bool IsDbComplete { get; set; }

        /// <summary>
        /// Детали по каждому индексу
        /// </summary>
        public List<IndexDetailDto> IndexDetails { get; set; } = new();
    }

    /// <summary>
    /// DTO для деталей конкретного индекса
    /// </summary>
    public class IndexDetailDto
    {
        /// <summary>
        /// Название индекса
        /// </summary>
        public string IndexName { get; set; } = string.Empty;

        /// <summary>
        /// Статус генерации TIFF
        /// </summary>
        public bool TiffGenerated { get; set; }

        /// <summary>
        /// Статус загрузки в БД
        /// </summary>
        public bool DbLoaded { get; set; }
    }

    /// <summary>
    /// Handler для GetSatelliteTaskStatusRequest
    /// </summary>
    public class GetSatelliteTaskStatusRequestHandler : IRequestHandler<GetSatelliteTaskStatusRequest, GetSatelliteTaskStatusResponse>
    {
        private readonly MasofaSentinelDbContext _dbSentinelContext;
        private readonly MasofaCommonDbContext _dbCommonContext;
        private readonly ILogger<GetSatelliteTaskStatusRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetSatelliteTaskStatusRequestHandler(
            MasofaSentinelDbContext dbSentinelContext,
            MasofaCommonDbContext dbCommonContext,
            ILogger<GetSatelliteTaskStatusRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dbSentinelContext = dbSentinelContext;
            _dbCommonContext = dbCommonContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<GetSatelliteTaskStatusResponse> Handle(GetSatelliteTaskStatusRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Базовый запрос для Sentinel2ProductQueue
                var query = _dbSentinelContext.Set<Sentinel2ProductQueue>()
                    .AsNoTracking();

                // Фильтр по статусу
                if (request.Status.HasValue)
                {
                    query = query.Where(q => q.QueueStatus == request.Status.Value);
                }

                // Фильтр по дате создания
                // Даты уже нормализованы в контроллере, используем как есть
                if (request.StartDate.HasValue)
                {
                    query = query.Where(q => q.CreateAt >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    query = query.Where(q => q.CreateAt <= request.EndDate.Value);
                }

                // Фильтр по ProductId
                if (!string.IsNullOrEmpty(request.ProductId))
                {
                    query = query.Where(q => q.ProductId.Contains(request.ProductId));
                }

                // Получаем общее количество до применения пагинации
                var totalCount = await query.CountAsync(cancellationToken);

                // Применяем сортировку
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = request.Sort == -1
                        ? query.OrderByDescending(q => EF.Property<object>(q, request.SortBy))
                        : query.OrderBy(q => EF.Property<object>(q, request.SortBy));
                }

                // Применяем пагинацию
                if (request.Take.HasValue)
                {
                    query = query.Skip(request.Offset).Take(request.Take.Value);
                }

                var tasks = await query.ToListAsync(cancellationToken);

                // Получаем детализированные статусы индексов
                var taskIds = tasks.Select(t => t.Id).ToList();
                var indexStatuses = await _dbSentinelContext.Set<Sentinel2GenerateIndexStatus>()
                    .AsNoTracking()
                    .Where(s => taskIds.Contains(s.Sentinel2ProductQueue))
                    .ToListAsync(cancellationToken);

                // Получаем информацию о продуктах
                var productIds = tasks.Select(t => t.ProductId).ToList();
                var products = await _dbCommonContext.Set<SatelliteProduct>()
                    .AsNoTracking()
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToListAsync(cancellationToken);

                // Формируем DTO
                var result = tasks.Select(task =>
                {
                    var indexStatus = indexStatuses.FirstOrDefault(s => s.Sentinel2ProductQueue == task.Id);
                    var product = products.FirstOrDefault(p => p.ProductId == task.ProductId);

                    var progressPercent = CalculateProgressPercent(task.QueueStatus, indexStatus);
                    var statusName = GetStatusDisplayName(task.QueueStatus);

                    return new SatelliteTaskStatusDto
                    {
                        Id = task.Id,
                        ProductId = task.ProductId,
                        ProductName = product?.ProductId ?? task.ProductId,
                        SatelliteType = ProductSourceType.Sentinel2, // Пока только Sentinel2
                        Status = task.QueueStatus,
                        StatusName = statusName,
                        CreateAt = task.CreateAt,
                        OriginDate = task.OriginDate,
                        ProgressPercent = progressPercent,
                        IndexStatus = indexStatus != null ? new SatelliteIndexStatusDto
                        {
                            IsTiffComplete = indexStatus.IsTiffComplite,
                            IsDbComplete = indexStatus.IsDbComplite,
                            IndexDetails = GetIndexDetails(indexStatus)
                        } : null,
                        LastUpdated = task.LastUpdateAt,
                        ErrorMessage = null // TODO: Добавить логику для ошибок
                    };
                }).ToList();

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

                return new GetSatelliteTaskStatusResponse
                {
                    Data = result,
                    Total = totalCount
                };
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        private int CalculateProgressPercent(ProductQueueStatusType status, Sentinel2GenerateIndexStatus? indexStatus)
        {
            // Базовый прогресс по статусу
            var baseProgress = status switch
            {
                ProductQueueStatusType.New => 0,
                ProductQueueStatusType.MetadataLoaded => 10,
                ProductQueueStatusType.MediaLoaded => 20,
                ProductQueueStatusType.Parsed => 30,
                ProductQueueStatusType.IndicesComplite => 40,
                ProductQueueStatusType.PreviewGenerated => 80,
                ProductQueueStatusType.GeoserverImported => 90,
                ProductQueueStatusType.GeoserverImportedIndex => 100,
                _ => 0
            };

            // Дополнительный прогресс по индексам
            if (indexStatus != null)
            {
                var tiffProgress = CalculateIndexProgress(indexStatus, true) * 0.3; // 30% за TIFF
                var dbProgress = CalculateIndexProgress(indexStatus, false) * 0.2; // 20% за DB
                return Math.Min(100, baseProgress + (int)(tiffProgress + dbProgress));
            }

            return baseProgress;
        }

        private int CalculateIndexProgress(Sentinel2GenerateIndexStatus indexStatus, bool isTiff)
        {
            var completed = 0;
            var total = 8; // Всего 8 индексов

            if (isTiff)
            {
                if (indexStatus.ArviTiff) completed++;
                if (indexStatus.EviTiff) completed++;
                if (indexStatus.GndviTiff) completed++;
                if (indexStatus.MndwiTiff) completed++;
                if (indexStatus.NdmiTiff) completed++;
                if (indexStatus.NdviTiff) completed++;
                if (indexStatus.OrviTiff) completed++;
                if (indexStatus.OsaviTiff) completed++;
            }
            else
            {
                if (indexStatus.ArviDb) completed++;
                if (indexStatus.EviDb) completed++;
                if (indexStatus.GndviDb) completed++;
                if (indexStatus.MndwiDb) completed++;
                if (indexStatus.NdmiDb) completed++;
                if (indexStatus.NdviDb) completed++;
                if (indexStatus.OrviDb) completed++;
                if (indexStatus.OsaviDb) completed++;
            }

            return (completed * 100) / total;
        }

        private string GetStatusDisplayName(ProductQueueStatusType status)
        {
            return status switch
            {
                ProductQueueStatusType.New => "Новая задача",
                ProductQueueStatusType.MetadataLoaded => "Метаданные загружены",
                ProductQueueStatusType.MediaLoaded => "Медиа загружены",
                ProductQueueStatusType.Parsed => "Обработано",
                ProductQueueStatusType.PreviewGenerated => "Превью создано",
                ProductQueueStatusType.IndicesComplite => "Индексы завершены",
                ProductQueueStatusType.GeoserverImported => "Импорт в GeoServer",
                ProductQueueStatusType.GeoserverImportedIndex => "Индексы в GeoServer",
                _ => "Неизвестный статус"
            };
        }

        private List<IndexDetailDto> GetIndexDetails(Sentinel2GenerateIndexStatus indexStatus)
        {
            return new List<IndexDetailDto>
            {
                new() { IndexName = "ARVI", TiffGenerated = indexStatus.ArviTiff, DbLoaded = indexStatus.ArviDb },
                new() { IndexName = "EVI", TiffGenerated = indexStatus.EviTiff, DbLoaded = indexStatus.EviDb },
                new() { IndexName = "GNDVI", TiffGenerated = indexStatus.GndviTiff, DbLoaded = indexStatus.GndviDb },
                new() { IndexName = "MNDWI", TiffGenerated = indexStatus.MndwiTiff, DbLoaded = indexStatus.MndwiDb },
                new() { IndexName = "NDMI", TiffGenerated = indexStatus.NdmiTiff, DbLoaded = indexStatus.NdmiDb },
                new() { IndexName = "NDVI", TiffGenerated = indexStatus.NdviTiff, DbLoaded = indexStatus.NdviDb },
                new() { IndexName = "ORVI", TiffGenerated = indexStatus.OrviTiff, DbLoaded = indexStatus.OrviDb },
                new() { IndexName = "OSAVI", TiffGenerated = indexStatus.OsaviTiff, DbLoaded = indexStatus.OsaviDb }
            };
        }
    }
}
