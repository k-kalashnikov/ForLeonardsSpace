using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemDocumentation;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Common
{
    /// <summary>
    /// Команда для создания или обновления системной документации с опциональным файлом
    /// </summary>
    public class SystemDocumentationCreateWithFileCommand : IRequest<Guid>
    {
        public SystemDocumentation Model { get; set; }

        public IFormFile? File { get; set; }

        /// <summary>
        /// Язык для файла (например, "ru-RU", "en-US", "uz-Latn-UZ")
        /// </summary>
        public string? Language { get; set; }

        public string Author { get; set; }
    }

    /// <summary>
    /// Обработчик команды создания системной документации с файлом
    /// </summary>
    public class SystemDocumentationCreateWithFileCommandHandler : IRequestHandler<SystemDocumentationCreateWithFileCommand, Guid>
    {
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly IMediator _mediator;
        private readonly ILogger<SystemDocumentationCreateWithFileCommandHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public SystemDocumentationCreateWithFileCommandHandler(
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            IMediator mediator,
            ILogger<SystemDocumentationCreateWithFileCommandHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _commonDbContext = commonDbContext;
            _identityDbContext = identityDbContext;
            _mediator = mediator;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<Guid> Handle(SystemDocumentationCreateWithFileCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var incomingModel = request.Model;

                var lastUpdateUser = await _identityDbContext.Set<User>().FirstAsync(m => m.UserName.ToLower().Equals(request.Author.ToLower()), cancellationToken);

                // Determine create or update by Id
                var isCreate = incomingModel.Id == Guid.Empty || !await _commonDbContext.Set<SystemDocumentation>().AnyAsync(e => e.Id == incomingModel.Id, cancellationToken);

                if (isCreate)
                {
                    // Create new entity
                    incomingModel.CreateAt = DateTime.UtcNow;
                    incomingModel.LastUpdateAt = DateTime.UtcNow;
                    incomingModel.CreateUser = lastUpdateUser.Id;
                    incomingModel.LastUpdateUser = lastUpdateUser.Id;
                    incomingModel.Status = StatusType.Active;

                    // Initialize FileStorageIds if not set
                    if (incomingModel.FileStorageIds.ValuesJson == null || string.IsNullOrWhiteSpace(incomingModel.FileStorageIds.ValuesJson))
                    {
                        incomingModel.FileStorageIds = new LocalizationFileStorageItem();
                    }

                    var addResult = await _commonDbContext.Set<SystemDocumentation>().AddAsync(incomingModel, cancellationToken);
                    await _commonDbContext.SaveChangesAsync(cancellationToken);

                    var created = addResult.Entity;
                    var entityId = created.Id;
                    
                    // If file uploaded, attach new file for the specified language
                    if (request.File != null && request.File.Length > 0 && !string.IsNullOrWhiteSpace(request.Language))
                    {
                        var uploadCommand = new UploadDocumentCommand
                        {
                            File = request.File,
                            OwnerTypeFullName = typeof(SystemDocumentation).FullName,
                            Bucket = "system-documents"
                        };

                        var fileStorageItem = await _mediator.Send(uploadCommand, cancellationToken);

                        fileStorageItem.OwnerId = entityId;
                        fileStorageItem.LastUpdateAt = DateTime.UtcNow;
                        _commonDbContext.FileStorageItems.Update(fileStorageItem);
                        await _commonDbContext.SaveChangesAsync(cancellationToken);

                        // Set file for the specified language
                        var fileStorageIds = created.FileStorageIds;
                        fileStorageIds[request.Language] = fileStorageItem.Id;
                        created.FileStorageIds = fileStorageIds;
                        _commonDbContext.Set<SystemDocumentation>().Update(created);
                        await _commonDbContext.SaveChangesAsync(cancellationToken);
                    }

                    await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, entityId.ToString()), requestPath);
                    return entityId;
                }
                else
                {
                    // Update existing entity
                    var existing = await _commonDbContext.Set<SystemDocumentation>().FirstAsync(e => e.Id == incomingModel.Id, cancellationToken);

                    // Update basic fields
                    existing.Names = incomingModel.Names;
                    existing.Visible = incomingModel.Visible;
                    existing.OrderCode = incomingModel.OrderCode;
                    existing.ExtData = incomingModel.ExtData;
                    existing.Comment = incomingModel.Comment;
                    existing.BlockId = incomingModel.BlockId;
                    existing.LastUpdateAt = DateTime.UtcNow;
                    existing.LastUpdateUser = lastUpdateUser.Id;

                    // Initialize FileStorageIds if not set
                    if (existing.FileStorageIds.ValuesJson == null || string.IsNullOrWhiteSpace(existing.FileStorageIds.ValuesJson))
                    {
                        existing.FileStorageIds = new LocalizationFileStorageItem();
                    }

                    // Handle file logic
                    var hasUploadedNewFile = request.File != null && request.File.Length > 0 && !string.IsNullOrWhiteSpace(request.Language);

                    if (hasUploadedNewFile)
                    {
                        // Capture previous file id for this language
                        var previousFileId = existing.FileStorageIds[request.Language];

                        // Upload new file and bind
                        var uploadCommand = new UploadDocumentCommand
                        {
                            File = request.File!,
                            OwnerTypeFullName = typeof(SystemDocumentation).FullName,
                            Bucket = "system-documents"
                        };
                        var newFile = await _mediator.Send(uploadCommand, cancellationToken);

                        newFile.OwnerId = existing.Id;
                        newFile.LastUpdateAt = DateTime.UtcNow;
                        _commonDbContext.FileStorageItems.Update(newFile);
                        await _commonDbContext.SaveChangesAsync(cancellationToken);

                        // Set file for the specified language
                        var fileStorageIds = existing.FileStorageIds;
                        fileStorageIds[request.Language] = newFile.Id;
                        existing.FileStorageIds = fileStorageIds;

                        // Detach previous file for this language if it existed and was different
                        if (previousFileId.HasValue && previousFileId.Value != Guid.Empty && previousFileId.Value != newFile.Id)
                        {
                            var prevFile = await _commonDbContext.FileStorageItems.FirstOrDefaultAsync(f => f.Id == previousFileId.Value, cancellationToken);
                            if (prevFile != null)
                            {
                                prevFile.OwnerId = Guid.Empty;
                                prevFile.LastUpdateAt = DateTime.UtcNow;
                                _commonDbContext.FileStorageItems.Update(prevFile);
                                await _commonDbContext.SaveChangesAsync(cancellationToken);
                            }
                        }
                    }
                    else
                    {
                        // Update FileStorageIds from incoming model (for removing files or updating without uploading)
                        existing.FileStorageIds = incomingModel.FileStorageIds;
                    }

                    _commonDbContext.Set<SystemDocumentation>().Update(existing);
                    await _commonDbContext.SaveChangesAsync(cancellationToken);

                    await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, existing.Id.ToString()), requestPath);
                    return existing.Id;
                }
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}

