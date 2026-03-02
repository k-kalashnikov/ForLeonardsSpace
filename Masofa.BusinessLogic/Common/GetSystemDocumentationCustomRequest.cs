using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemDocumentation;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Masofa.Common.Models.SystemCrical;

namespace Masofa.BusinessLogic.Common
{
    /// <summary>
    /// Расширенная модель системной документации с размером файла
    /// </summary>
    public class SystemDocumentationWithFileSize : SystemDocumentation
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
    /// Запрос для получения системной документации с информацией о размере файла
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetSystemDocumentationCustomRequest : IRequest<List<SystemDocumentationWithFileSize>>
    {
        /// <summary>
        /// Базовый запрос с фильтрами, сортировкой и пагинацией
        /// </summary>
        public BaseGetQuery<SystemDocumentation> Query { get; set; }
    }

    /// <summary>
    /// Обработчик запроса на получение системной документации с размером файла
    /// </summary>
    public class GetSystemDocumentationCustomRequestHandler : IRequestHandler<GetSystemDocumentationCustomRequest, List<SystemDocumentationWithFileSize>>
    {
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly ILogger<GetSystemDocumentationCustomRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetSystemDocumentationCustomRequestHandler(
            MasofaCommonDbContext commonDbContext,
            ILogger<GetSystemDocumentationCustomRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _commonDbContext = commonDbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<SystemDocumentationWithFileSize>> Handle(GetSystemDocumentationCustomRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Получаем базовые данные из SystemDocumentation
                IQueryable<SystemDocumentation> resultQuery = _commonDbContext
                    .Set<SystemDocumentation>()
                    .AsNoTracking()
                    .Where(m => m.Status == StatusType.Active);

                // Применяем фильтры
                if (request.Query.Filters != null && request.Query.Filters.Count > 0)
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

                // Получаем все FileStorageId для загрузки информации о файлах (из всех языков)
                var fileStorageIds = documents
                    .SelectMany(doc =>
                    {
                        var ids = new List<Guid>();
                        var supportedLanguages = new[] { "ru-RU", "en-US", "uz-Latn-UZ", "uz-Cyrl-UZ", "ar-LB" };
                        foreach (var lang in supportedLanguages)
                        {
                            var fileId = doc.FileStorageIds[lang];
                            if (fileId.HasValue)
                            {
                                ids.Add(fileId.Value);
                            }
                        }
                        return ids;
                    })
                    .Distinct()
                    .ToList();

                // Загружаем информацию о файлах одним запросом
                var fileStorageItems = await _commonDbContext.FileStorageItems
                    .Where(f => fileStorageIds.Contains(f.Id))
                    .ToDictionaryAsync(f => f.Id, cancellationToken);

                // Объединяем данные
                // Используем приоритет языков: uz-Latn-UZ, en-US, ru-RU
                var result = documents.Select(doc =>
                {
                    var priorityLanguages = new[] { "uz-Latn-UZ", "en-US", "ru-RU", "uz-Cyrl-UZ", "ar-LB" };
                    Guid? selectedFileId = null;
                    FileStorageItem? selectedFile = null;

                    foreach (var lang in priorityLanguages)
                    {
                        var fileId = doc.FileStorageIds[lang];
                        if (fileId.HasValue && fileStorageItems.ContainsKey(fileId.Value))
                        {
                            selectedFileId = fileId.Value;
                            selectedFile = fileStorageItems[fileId.Value];
                            break;
                        }
                    }

                    return new SystemDocumentationWithFileSize
                    {
                        Id = doc.Id,
                        CreateAt = doc.CreateAt,
                        BlockId = doc.BlockId,
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
                        FileStorageIds = doc.FileStorageIds,
                        FileSize = selectedFile?.FileLength,
                        FileSizeString = FormatFileSize(selectedFile?.FileLength)
                    };
                }).ToList();

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

