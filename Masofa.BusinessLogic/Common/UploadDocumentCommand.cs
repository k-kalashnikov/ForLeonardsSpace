using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Common
{
    /// <summary>
    /// Команда для загрузки файла документа
    /// </summary>
    public class UploadDocumentCommand : IRequest<FileStorageItem>
    {
        /// <summary>
        /// Файл для загрузки
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// Имя владельца (тип документа)
        /// </summary>
        public string OwnerTypeFullName { get; set; }

        /// <summary>
        /// Бакет для хранения файла
        /// </summary>
        public string Bucket { get; set; }
    }

    /// <summary>
    /// Обработчик команды загрузки файла
    /// </summary>
    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, FileStorageItem>
    {
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly ILogger<UploadDocumentCommandHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public UploadDocumentCommandHandler(
            MasofaCommonDbContext commonDbContext,
            IFileStorageProvider fileStorageProvider,
            ILogger<UploadDocumentCommandHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _commonDbContext = commonDbContext;
            _fileStorageProvider = fileStorageProvider;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<FileStorageItem> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (request.File == null || request.File.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                // Определяем тип файла по расширению
                var fileContentType = DetermineFileContentType(request.File.FileName);

                // Сохраняем файл в хранилище
                using var stream = request.File.OpenReadStream();
                string filePath = await _fileStorageProvider.PushFileAsync(stream, request.File.FileName, request.Bucket);

                // Создаем запись в FileStorageItems
                var fileStorageItem = new FileStorageItem
                {
                    Id = Guid.NewGuid(),
                    OwnerId = Guid.Empty, // Будет обновлен при создании документа
                    OwnerTypeFullName = request.OwnerTypeFullName,
                    FileStoragePath = filePath,
                    FileStorageBacket = request.Bucket,
                    FileContentType = fileContentType,
                    FileLength = request.File.Length, // Сохраняем размер файла
                    CreateAt = DateTime.UtcNow,
                    Status = Masofa.Common.Models.StatusType.Active,
                    CreateUser = Guid.Empty,
                    LastUpdateAt = DateTime.UtcNow,
                    LastUpdateUser = Guid.Empty
                };

                await _commonDbContext.FileStorageItems.AddAsync(fileStorageItem, cancellationToken);
                await _commonDbContext.SaveChangesAsync(cancellationToken);

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, fileStorageItem.OwnerTypeFullName), requestPath);
                return fileStorageItem;
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
        /// Определяет тип контента файла по расширению
        /// </summary>
        private FileContentType DetermineFileContentType(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            return fileExtension switch
            {
                ".jpg" or ".jpeg" => FileContentType.ImageJPG,
                ".png" => FileContentType.ImagePNG,
                ".webp" => FileContentType.ImageWEBP,
                ".tif" or ".tiff" => FileContentType.ImageTiff,
                ".zip" => FileContentType.ArchiveZIP,
                ".pdf" => FileContentType.Default, // PDF пока как Default
                _ => FileContentType.Default
            };
        }
    }
}


