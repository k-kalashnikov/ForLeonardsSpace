using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Dictionaries
{
    /// <summary>
    /// Команда для создания или обновления нормативной документации с опциональным файлом
    /// </summary>
    public class RegulatoryDocumentationCreateWithFileCommand : IRequest<Guid>
    {
        public RegulatoryDocumentation Model { get; set; }

        public IFormFile? File { get; set; }

        public string Author { get; set; }
    }

    /// <summary>
    /// Обработчик команды создания нормативной документации с файлом
    /// </summary>
    public class RegulatoryDocumentationCreateWithFileCommandHandler : IRequestHandler<RegulatoryDocumentationCreateWithFileCommand, Guid>
    {
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly IMediator _mediator;
        private readonly ILogger<RegulatoryDocumentationCreateWithFileCommandHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public RegulatoryDocumentationCreateWithFileCommandHandler(
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            IMediator mediator,
            ILogger<RegulatoryDocumentationCreateWithFileCommandHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dictionariesDbContext = dictionariesDbContext;
            _commonDbContext = commonDbContext;
            _identityDbContext = identityDbContext;
            _mediator = mediator;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<Guid> Handle(RegulatoryDocumentationCreateWithFileCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var incomingModel = request.Model;

                var lastUpdateUser = await _identityDbContext.Set<User>().FirstAsync(m => m.UserName.ToLower().Equals(request.Author.ToLower()), cancellationToken);

                // Determine create or update by Id
                var isCreate = incomingModel.Id == Guid.Empty || !await _dictionariesDbContext.Set<RegulatoryDocumentation>().AnyAsync(e => e.Id == incomingModel.Id, cancellationToken);

                if (isCreate)
                {
                    // Create new entity
                    incomingModel.CreateAt = DateTime.UtcNow;
                    incomingModel.LastUpdateAt = DateTime.UtcNow;
                    incomingModel.CreateUser = lastUpdateUser.Id;
                    incomingModel.LastUpdateUser = lastUpdateUser.Id;
                    incomingModel.Status = StatusType.Active;

                    // Temporarily clear FileStorageId; will set after upload if file provided
                    var providedFileId = incomingModel.FileStorageId;
                    incomingModel.FileStorageId = null;

                    var addResult = await _dictionariesDbContext.Set<RegulatoryDocumentation>().AddAsync(incomingModel, cancellationToken);
                    await _dictionariesDbContext.SaveChangesAsync(cancellationToken);

                    var created = addResult.Entity;
                    var entityId = created.Id;

                    // If file uploaded, attach new file
                    if (request.File != null && request.File.Length > 0)
                    {
                        var uploadCommand = new UploadDocumentCommand
                        {
                            File = request.File,
                            OwnerTypeFullName = typeof(RegulatoryDocumentation).FullName,
                            Bucket = "normative-documents"
                        };

                        var fileStorageItem = await _mediator.Send(uploadCommand, cancellationToken);

                        fileStorageItem.OwnerId = entityId;
                        fileStorageItem.LastUpdateAt = DateTime.UtcNow;
                        _commonDbContext.FileStorageItems.Update(fileStorageItem);
                        await _commonDbContext.SaveChangesAsync(cancellationToken);

                        created.FileStorageId = fileStorageItem.Id;
                        _dictionariesDbContext.Set<RegulatoryDocumentation>().Update(created);
                        await _dictionariesDbContext.SaveChangesAsync(cancellationToken);
                    }
                    else if (providedFileId.HasValue)
                    {
                        // If client sent FileStorageId without uploading a file, try to bind existing file
                        var existingFile = await _commonDbContext.FileStorageItems.FirstOrDefaultAsync(f => f.Id == providedFileId.Value, cancellationToken);
                        if (existingFile != null)
                        {
                            existingFile.OwnerId = entityId;
                            existingFile.LastUpdateAt = DateTime.UtcNow;
                            _commonDbContext.FileStorageItems.Update(existingFile);
                            await _commonDbContext.SaveChangesAsync(cancellationToken);

                            created.FileStorageId = existingFile.Id;
                            _dictionariesDbContext.Set<RegulatoryDocumentation>().Update(created);
                            await _dictionariesDbContext.SaveChangesAsync(cancellationToken);
                        }
                    }

                    await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, entityId.ToString()), requestPath);
                    return entityId;
                }
                else
                {
                    // Update existing entity
                    var existing = await _dictionariesDbContext.Set<RegulatoryDocumentation>().FirstAsync(e => e.Id == incomingModel.Id, cancellationToken);

                    // Capture previous file id
                    var previousFileId = existing.FileStorageId;

                    // Update basic fields
                    existing.Names = incomingModel.Names;
                    existing.Visible = incomingModel.Visible;
                    existing.OrderCode = incomingModel.OrderCode;
                    existing.ExtData = incomingModel.ExtData;
                    existing.Comment = incomingModel.Comment;
                    existing.LastUpdateAt = DateTime.UtcNow;
                    existing.LastUpdateUser = lastUpdateUser.Id;

                    // Handle file logic
                    var hasUploadedNewFile = request.File != null && request.File.Length > 0;
                    var clientFileId = incomingModel.FileStorageId; // reflects UI intent

                    if (hasUploadedNewFile)
                    {
                        // Upload new file and bind
                        var uploadCommand = new UploadDocumentCommand
                        {
                            File = request.File!,
                            OwnerTypeFullName = typeof(RegulatoryDocumentation).FullName,
                            Bucket = "normative-documents"
                        };
                        var newFile = await _mediator.Send(uploadCommand, cancellationToken);

                        newFile.OwnerId = existing.Id;
                        newFile.LastUpdateAt = DateTime.UtcNow;
                        _commonDbContext.FileStorageItems.Update(newFile);
                        await _commonDbContext.SaveChangesAsync(cancellationToken);

                        existing.FileStorageId = newFile.Id;

                        // Detach previous file if it existed
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
                        // No new file uploaded
                        if (clientFileId.HasValue)
                        {
                            // Keep or re-bind the specified file id
                            if (!previousFileId.HasValue || previousFileId.Value != clientFileId.Value)
                            {
                                // Bind the specified file to this owner
                                var fileToBind = await _commonDbContext.FileStorageItems.FirstOrDefaultAsync(f => f.Id == clientFileId.Value, cancellationToken);
                                if (fileToBind != null)
                                {
                                    fileToBind.OwnerId = existing.Id;
                                    fileToBind.LastUpdateAt = DateTime.UtcNow;
                                    _commonDbContext.FileStorageItems.Update(fileToBind);
                                    await _commonDbContext.SaveChangesAsync(cancellationToken);
                                }
                            }
                            existing.FileStorageId = clientFileId.Value;
                        }
                        else
                        {
                            // Client removed file => detach previous file if existed
                            if (previousFileId.HasValue && previousFileId.Value != Guid.Empty)
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
                            existing.FileStorageId = null;
                        }
                    }

                    _dictionariesDbContext.Set<RegulatoryDocumentation>().Update(existing);
                    await _dictionariesDbContext.SaveChangesAsync(cancellationToken);

                    await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, existing.Id.ToString()), requestPath);
                    return existing.Id;
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error creating/updating regulatory documentation with optional file in {requestPath}: {ex.Message}";
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}

