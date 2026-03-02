using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Dictionaries
{
    /// <summary>
    /// Расширенная модель нормативной документации с размером файла
    /// </summary>
    public class RegulatoryDocumentationWithFileSize : RegulatoryDocumentation
    {
        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// Размер файла в человекочитаемом формате (например, "375 Bytes", "1.5 MB")
        /// </summary>
        public string? FileSizeString { get; set; }
    }

    /// <summary>
    /// Запрос для получения нормативной документации с информацией о размере файла
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetRegulatoryDocumentationCustomRequest : IRequest<List<RegulatoryDocumentationWithFileSize>>
    {
        /// <summary>
        /// Базовый запрос с фильтрами, сортировкой и пагинацией
        /// </summary>
        public BaseGetQuery<RegulatoryDocumentation> Query { get; set; }
    }

    /// <summary>
    /// Обработчик запроса на получение нормативной документации с размером файла
    /// </summary>
    public class GetRegulatoryDocumentationCustomRequestHandler : IRequestHandler<GetRegulatoryDocumentationCustomRequest, List<RegulatoryDocumentationWithFileSize>>
    {
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly ILogger<GetRegulatoryDocumentationCustomRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetRegulatoryDocumentationCustomRequestHandler(
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaCommonDbContext commonDbContext,
            ILogger<GetRegulatoryDocumentationCustomRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dictionariesDbContext = dictionariesDbContext;
            _commonDbContext = commonDbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<RegulatoryDocumentationWithFileSize>> Handle(GetRegulatoryDocumentationCustomRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Получаем базовые данные из RegulatoryDocumentation
                IQueryable<RegulatoryDocumentation> resultQuery = _dictionariesDbContext
                    .Set<RegulatoryDocumentation>()
                    .AsNoTracking()
                    .Where(m => m.Status == StatusType.Active);

                // Применяем фильтры
                if (request.Query.Filters.Any())
                {
                    foreach (var item in request.Query.Filters)
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

                // Получаем список документации
                var documents = await resultQuery.ToListAsync(cancellationToken);

                // Получаем все FileStorageId для загрузки информации о файлах
                var fileStorageIds = documents
                    .Where(d => d.FileStorageId.HasValue)
                    .Select(d => d.FileStorageId.Value)
                    .Distinct()
                    .ToList();

                // Загружаем информацию о файлах одним запросом
                var fileStorageItems = await _commonDbContext.FileStorageItems
                    .Where(f => fileStorageIds.Contains(f.Id))
                    .ToDictionaryAsync(f => f.Id, cancellationToken);

                // Объединяем данные
                var result = documents.Select(doc => new RegulatoryDocumentationWithFileSize
                {
                    Id = doc.Id,
                    CreateAt = doc.CreateAt,
                    Status = doc.Status,
                    LastUpdateAt = doc.LastUpdateAt,
                    CreateUser = doc.CreateUser,
                    LastUpdateUser = doc.LastUpdateUser,
                    IsPublic = doc.IsPublic,
                    Visible = doc.Visible,
                    OrderCode = doc.OrderCode,
                    ExtData = doc.ExtData,
                    Comment = doc.Comment,
                    Names = doc.Names,
                    FileStorageId = doc.FileStorageId,
                    FileSize = doc.FileStorageId.HasValue && fileStorageItems.ContainsKey(doc.FileStorageId.Value)
                        ? fileStorageItems[doc.FileStorageId.Value].FileLength
                        : null,
                    FileSizeString = FormatFileSize(
                        doc.FileStorageId.HasValue && fileStorageItems.ContainsKey(doc.FileStorageId.Value)
                            ? fileStorageItems[doc.FileStorageId.Value].FileLength
                            : null
                    )
                }).ToList();

                await _businessLogicLogger.LogInformationAsync(
                    LogMessageResource.RequestFinishedWithResult(requestPath,result.Count.ToString()), requestPath);

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
        /// Форматирует размер файла в человекочитаемый формат
        /// </summary>
        private static string FormatFileSize(long? bytes)
        {
            if (!bytes.HasValue || bytes.Value == 0)
                return "0 Bytes";

            var size = bytes.Value;
            var k = 1024;
            var sizes = new[] { "Bytes", "KB", "MB", "GB" };
            var i = (int)Math.Floor(Math.Log(size) / Math.Log(k));

            return Math.Round(size / Math.Pow(k, i), 2) + " " + sizes[i];
        }
    }
}

