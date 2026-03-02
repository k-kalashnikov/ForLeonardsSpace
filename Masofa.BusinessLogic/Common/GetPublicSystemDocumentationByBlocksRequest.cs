using Masofa.Common.Resources;
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
    /// DTO для публичной документации с информацией о файле
    /// </summary>
    public class PublicSystemDocumentationDto
    {
        /// <summary>
        /// Идентификатор документации
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Многоязычное имя документации
        /// </summary>
        public LocalizationString Names { get; set; }

        /// <summary>
        /// Локализованные идентификаторы файлов в хранилище (по языкам)
        /// </summary>
        public LocalizationFileStorageItem FileStorageIds { get; set; }

        /// <summary>
        /// Размер файла в байтах (для текущего языка)
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// Размер файла в человекочитаемом формате (для текущего языка)
        /// </summary>
        public string? FileSizeString { get; set; }

        /// <summary>
        /// Имя файла (для текущего языка)
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Код сортировки
        /// </summary>
        public string? OrderCode { get; set; }

        /// <summary>
        /// Идентификатор блока документации
        /// </summary>
        public Guid? BlockId { get; set; }
    }

    /// <summary>
    /// DTO для блока документации с документами
    /// </summary>
    public class SystemDocumentationBlockDto
    {
        /// <summary>
        /// Идентификатор блока
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Многоязычное имя блока
        /// </summary>
        public LocalizationString Names { get; set; }

        /// <summary>
        /// Идентификатор родительского блока
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Код сортировки
        /// </summary>
        public int? OrderCode { get; set; }

        /// <summary>
        /// Список дочерних блоков
        /// </summary>
        public List<SystemDocumentationBlockDto> Children { get; set; } = new List<SystemDocumentationBlockDto>();

        /// <summary>
        /// Список документации в этом блоке
        /// </summary>
        public List<PublicSystemDocumentationDto> Documents { get; set; } = new List<PublicSystemDocumentationDto>();
    }

    /// <summary>
    /// Запрос для получения публичной документации, сгруппированной по блокам
    /// </summary>
    public class GetPublicSystemDocumentationByBlocksRequest : IRequest<List<SystemDocumentationBlockDto>>
    {
    }

    /// <summary>
    /// Обработчик запроса на получение публичной документации по блокам
    /// </summary>
    public class GetPublicSystemDocumentationByBlocksRequestHandler : IRequestHandler<GetPublicSystemDocumentationByBlocksRequest, List<SystemDocumentationBlockDto>>
    {
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly ILogger<GetPublicSystemDocumentationByBlocksRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetPublicSystemDocumentationByBlocksRequestHandler(
            MasofaCommonDbContext commonDbContext,
            MasofaDictionariesDbContext dictionariesDbContext,
            ILogger<GetPublicSystemDocumentationByBlocksRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _commonDbContext = commonDbContext;
            _dictionariesDbContext = dictionariesDbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<SystemDocumentationBlockDto>> Handle(GetPublicSystemDocumentationByBlocksRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Получаем все публичные блоки документации
                var blocks = await _dictionariesDbContext.SystemDocumentationBlocks
                    .AsNoTracking()
                    .Where(b => b.Status == StatusType.Active)
                    .ToListAsync(cancellationToken);

                // Сортируем блоки в памяти
                blocks = blocks
                    .OrderBy(b => b.OrderCode ?? int.MaxValue)
                    .ThenBy(b => b.Names != null && !string.IsNullOrEmpty(b.Names["ru-RU"]) ? b.Names["ru-RU"] : 
                                 b.Names != null && !string.IsNullOrEmpty(b.Names["en-US"]) ? b.Names["en-US"] : 
                                 b.Names != null && !string.IsNullOrEmpty(b.Names["uz-Latn-UZ"]) ? b.Names["uz-Latn-UZ"] : "")
                    .ToList();

                // Получаем все публичные документы
                var documents = await _commonDbContext.SystemDocumentations
                    .AsNoTracking()
                    .Where(d => d.Status == StatusType.Active && d.Visible)
                    .ToListAsync(cancellationToken);

                // Сортируем документы в памяти
                documents = documents
                    .OrderBy(d => d.OrderCode ?? "")
                    .ThenBy(d => d.Names != null && !string.IsNullOrEmpty(d.Names["ru-RU"]) ? d.Names["ru-RU"] : 
                                 d.Names != null && !string.IsNullOrEmpty(d.Names["en-US"]) ? d.Names["en-US"] : 
                                 d.Names != null && !string.IsNullOrEmpty(d.Names["uz-Latn-UZ"]) ? d.Names["uz-Latn-UZ"] : "")
                    .ToList();

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

                // Преобразуем блоки в DTO
                var blockDtos = blocks.Select(b => new SystemDocumentationBlockDto
                {
                    Id = b.Id,
                    Names = b.Names,
                    ParentId = b.ParentId,
                    OrderCode = b.OrderCode
                }).ToList();

                // Преобразуем документы в DTO
                // Используем приоритет языков: uz-Latn-UZ, en-US, ru-RU
                var documentDtos = documents.Select(doc =>
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

                    return new PublicSystemDocumentationDto
                    {
                        Id = doc.Id,
                        Names = doc.Names,
                        FileStorageIds = doc.FileStorageIds,
                        FileSize = selectedFile?.FileLength,
                        FileSizeString = FormatFileSize(selectedFile?.FileLength),
                        FileName = selectedFile != null
                            ? ExtractFileName(selectedFile.FileStoragePath)
                            : null,
                        Comment = doc.Comment,
                        OrderCode = doc.OrderCode,
                        BlockId = doc.BlockId
                    };
                }).ToList();

                // Группируем документы по блокам
                foreach (var documentDto in documentDtos)
                {
                    if (documentDto.BlockId.HasValue)
                    {
                        var blockDto = blockDtos.FirstOrDefault(b => b.Id == documentDto.BlockId.Value);
                        if (blockDto != null)
                        {
                            blockDto.Documents.Add(documentDto);
                        }
                    }
                }

                // Строим иерархию блоков
                var rootBlocks = blockDtos.Where(b => !b.ParentId.HasValue).ToList();
                foreach (var rootBlock in rootBlocks)
                {
                    BuildBlockHierarchy(rootBlock, blockDtos);
                }

                // Также добавляем блоки без родителя, которые не были добавлены в иерархию
                var blocksWithoutParent = blockDtos.Where(b => !b.ParentId.HasValue && !rootBlocks.Contains(b)).ToList();
                rootBlocks.AddRange(blocksWithoutParent);

                await _businessLogicLogger.LogInformationAsync(
                    LogMessageResource.RequestFinishedWithResult(requestPath,"result"), requestPath);

                return rootBlocks;
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
        /// Рекурсивно строит иерархию блоков
        /// </summary>
        private void BuildBlockHierarchy(SystemDocumentationBlockDto parentBlock, List<SystemDocumentationBlockDto> allBlocks)
        {
            var children = allBlocks.Where(b => b.ParentId == parentBlock.Id).ToList();
            foreach (var child in children)
            {
                parentBlock.Children.Add(child);
                BuildBlockHierarchy(child, allBlocks);
            }
        }

        /// <summary>
        /// Извлекает имя файла из пути
        /// </summary>
        private static string? ExtractFileName(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var parts = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[^1] : null;
        }

        /// <summary>
        /// Форматирует размер файла в человекочитаемый формат
        /// </summary>
        private static string? FormatFileSize(long? bytes)
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

