using Masofa.Common.Resources;
using System;
using System.IO;
using System.Linq;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Notifications;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Common.UserTickets;

/// <summary>
/// Команда на загрузку вложения для сообщения тикета
/// </summary>
public class UploadUserTicketAttachmentCommand : IRequest<UploadUserTicketAttachmentResult>
{
    /// <summary>
    /// Идентификатор тикета, к которому относится вложение
    /// </summary>
    public Guid UserTicketId { get; set; }

    /// <summary>
    /// Загружаемый файл
    /// </summary>
    public IFormFile? File { get; set; }
}

/// <summary>
/// Результат загрузки вложения
/// </summary>
public class UploadUserTicketAttachmentResult
{
    /// <summary>
    /// Идентификатор файла в хранилище
    /// </summary>
    public Guid FileId { get; set; }

    /// <summary>
    /// Имя исходного файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Тип содержимого
    /// </summary>
    public string ContentType { get; set; } = "application/octet-stream";
}

/// <summary>
/// Обработчик загрузки вложения
/// </summary>
public class UploadUserTicketAttachmentCommandHandler
    : IRequestHandler<UploadUserTicketAttachmentCommand, UploadUserTicketAttachmentResult>
{
    private const string DefaultBucket = "user-ticket-attachments";

    private readonly MasofaCommonDbContext _commonDbContext;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger<UploadUserTicketAttachmentCommandHandler> _logger;
    private readonly IBusinessLogicLogger _businessLogicLogger;

    public UploadUserTicketAttachmentCommandHandler(
        MasofaCommonDbContext commonDbContext,
        IFileStorageProvider fileStorageProvider,
        ILogger<UploadUserTicketAttachmentCommandHandler> logger,
        IBusinessLogicLogger businessLogicLogger)
    {
        _commonDbContext = commonDbContext;
        _fileStorageProvider = fileStorageProvider;
        _logger = logger;
        _businessLogicLogger = businessLogicLogger;
    }

    public async Task<UploadUserTicketAttachmentResult> Handle(
        UploadUserTicketAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

        try
        {
            if (request.UserTicketId == Guid.Empty)
            {
                throw new ArgumentException("UserTicketId is required");
            }

            if (request.File == null || request.File.Length == 0)
            {
                throw new ArgumentException("File is empty or null");
            }

            var ticketExists = await _commonDbContext.UserTickets
                .AsNoTracking()
                .AnyAsync(t => t.Id == request.UserTicketId, cancellationToken);

            if (!ticketExists)
            {
                throw new ArgumentException($"User ticket with id {request.UserTicketId} not found");
            }

            await _businessLogicLogger.LogInformationAsync(
                $"Uploading attachment for ticket {request.UserTicketId}: {request.File.FileName}",
                requestPath);

            var objectName = BuildObjectName(request.UserTicketId, request.File.FileName);

            using var stream = request.File.OpenReadStream();
            var storedPath = await _fileStorageProvider.PushFileAsync(stream, objectName, DefaultBucket);

            var fileStorageItem = new FileStorageItem
            {
                Id = Guid.NewGuid(),
                OwnerId = request.UserTicketId,
                OwnerTypeFullName = typeof(UserTicketMessage).FullName ?? nameof(UserTicketMessage),
                FileStoragePath = storedPath,
                FileStorageBacket = DefaultBucket,
                FileContentType = DetermineFileContentType(request.File.FileName, request.File.ContentType),
                FileLength = request.File.Length,
                CreateUser = Guid.Empty,
                LastUpdateUser = Guid.Empty,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                Status = StatusType.Active
            };

            await _commonDbContext.FileStorageItems.AddAsync(fileStorageItem, cancellationToken);
            await _commonDbContext.SaveChangesAsync(cancellationToken);

            await _businessLogicLogger.LogInformationAsync(
                $"Attachment stored successfully {fileStorageItem.Id}",
                requestPath);

            return new UploadUserTicketAttachmentResult
            {
                FileId = fileStorageItem.Id,
                FileName = request.File.FileName,
                FileSize = request.File.Length,
                ContentType = request.File.ContentType ?? "application/octet-stream"
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

    private static string BuildObjectName(Guid ticketId, string? originalFileName)
    {
        var extension = Path.GetExtension(originalFileName ?? string.Empty);
        var cleanExtension = string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.Trim();
        var sanitizedName = SanitizeFileName(Path.GetFileNameWithoutExtension(originalFileName ?? string.Empty));
        var uniquePart = Guid.NewGuid().ToString("N");

        if (string.IsNullOrWhiteSpace(sanitizedName))
        {
            sanitizedName = "attachment";
        }

        return $"{ticketId}/{sanitizedName}_{uniquePart}{cleanExtension}";
    }

    private static string SanitizeFileName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "file" : cleaned;
    }

    private static FileContentType DetermineFileContentType(string? fileName, string? contentTypeHeader)
    {
        if (!string.IsNullOrWhiteSpace(contentTypeHeader))
        {
            return contentTypeHeader.ToLowerInvariant() switch
            {
                "image/jpg" or "image/jpeg" => FileContentType.ImageJPG,
                "image/png" => FileContentType.ImagePNG,
                "image/webp" => FileContentType.ImageWEBP,
                "image/tif" or "image/tiff" => FileContentType.ImageTiff,
                "application/zip" or "application/x-zip-compressed" => FileContentType.ArchiveZIP,
                _ => FileContentType.Default
            };
        }

        var extension = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" => FileContentType.ImageJPG,
            ".png" => FileContentType.ImagePNG,
            ".webp" => FileContentType.ImageWEBP,
            ".tif" or ".tiff" => FileContentType.ImageTiff,
            ".zip" => FileContentType.ArchiveZIP,
            _ => FileContentType.Default
        };
    }
}

