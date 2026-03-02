using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Masofa.BusinessLogic.FieldPhotoRequest
{
    public class UploadFieldPhotoCommand : IRequest<Guid>
    {
        [Required]
        public required IFormFile File { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid? FieldId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? ParentRegionId { get; set; }
        public DateTime? CaptureDateUtc { get; set; }
        public string? Description { get; set; }
        public string? PointJson { get; set; }
        public IEnumerable<Guid> TagIds { get; set; } = Enumerable.Empty<Guid>();
        public required string Author { get; set; }
    }

    public class UploadFieldPhotoCommandHandler : IRequestHandler<UploadFieldPhotoCommand, Guid>
    {
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<UploadFieldPhotoCommandHandler> Logger { get; set; }
        private IMediator Mediator { get; set; }

        public UploadFieldPhotoCommandHandler(
            IBusinessLogicLogger businessLogicLogger,
            ILogger<UploadFieldPhotoCommandHandler> logger,
            MasofaIdentityDbContext identityDbContext,
            IMediator mediator,
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            MasofaCommonDbContext commonDbContext,
            MasofaDictionariesDbContext dictionariesDbContext)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            IdentityDbContext = identityDbContext;
            Mediator = mediator;
            CropMonitoringDbContext = cropMonitoringDbContext;
            CommonDbContext = commonDbContext;
            DictionariesDbContext = dictionariesDbContext;
        }

        public async Task<Guid> Handle(UploadFieldPhotoCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var lastUpdateUser = await IdentityDbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.UserName != null && m.UserName.ToLower().Equals(request.Author.ToLower()), cancellationToken);

                if (lastUpdateUser == null)
                {
                    var errorMsg = LogMessageResource.UserNotFound(request.Author);
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    throw new InvalidOperationException(errorMsg);
                }

                var newFieldPhoto = new FieldPhoto()
                {
                    CreateAt = DateTime.UtcNow,
                    LastUpdateAt = DateTime.UtcNow,
                    CreateUser = lastUpdateUser.Id,
                    LastUpdateUser = lastUpdateUser.Id,
                    Status = StatusType.Active,
                    Title = request.Title,
                    FieldId = request.FieldId,
                    RegionId = request.RegionId,
                    ParentRegionId = request.ParentRegionId,
                    CaptureDateUtc = request.CaptureDateUtc,
                    Description = request.Description,
                    PointJson = request.PointJson
                };

                var uploadCommand = new UploadDocumentCommand
                {
                    File = request.File,
                    OwnerTypeFullName = typeof(FieldPhoto).FullName ?? string.Empty,
                    Bucket = "field-photos"
                };

                var fileStorageItem = await Mediator.Send(uploadCommand, cancellationToken);

                var addResult = await CropMonitoringDbContext.FieldPhotos.AddAsync(newFieldPhoto, cancellationToken);
                await CropMonitoringDbContext.SaveChangesAsync(cancellationToken);

                var createdFieldPhoto = addResult.Entity;

                fileStorageItem.OwnerId = createdFieldPhoto.Id;
                fileStorageItem.OwnerTypeFullName = typeof(FieldPhoto).FullName ?? string.Empty;
                fileStorageItem.LastUpdateAt = DateTime.UtcNow;
                fileStorageItem.LastUpdateUser = lastUpdateUser.Id;
                fileStorageItem.CreateUser = lastUpdateUser.Id;
                fileStorageItem.LastUpdateAt = DateTime.UtcNow;
                CommonDbContext.FileStorageItems.Update(fileStorageItem);
                await CommonDbContext.SaveChangesAsync(cancellationToken);

                createdFieldPhoto.FileStorageId = fileStorageItem.Id;
                CropMonitoringDbContext.FieldPhotos.Update(createdFieldPhoto);
                await CropMonitoringDbContext.SaveChangesAsync(cancellationToken);

                var tagIds = (request.TagIds ?? Enumerable.Empty<Guid>()).Distinct().ToList();
                if (tagIds.Count > 0)
                {
                    var existingRelations = await CommonDbContext.TagRelations
                        .Where(tr => tr.OwnerId == createdFieldPhoto.Id &&
                                     tr.OwnerTypeFullName == typeof(FieldPhoto).FullName)
                        .ToListAsync(cancellationToken);

                    if (existingRelations.Count > 0)
                    {
                        CommonDbContext.TagRelations.RemoveRange(existingRelations);
                        await CommonDbContext.SaveChangesAsync(cancellationToken);
                    }

                    var availableTagIds = await DictionariesDbContext.Tags
                        .AsNoTracking()
                        .Where(t => tagIds.Contains(t.Id) && t.Status == StatusType.Active)
                        .Select(t => t.Id)
                        .ToListAsync(cancellationToken);

                    if (availableTagIds.Count > 0)
                    {
                        foreach (var tagId in availableTagIds)
                        {
                            var relation = new TagRelation
                            {
                                Id = Guid.NewGuid(),
                                OwnerId = createdFieldPhoto.Id,
                                OwnerTypeFullName = typeof(FieldPhoto).FullName ?? string.Empty,
                                TagId = tagId,
                                CreateAt = DateTime.UtcNow,
                                LastUpdateAt = DateTime.UtcNow,
                                CreateUser = lastUpdateUser.Id,
                                LastUpdateUser = lastUpdateUser.Id,
                                Status = StatusType.Active
                            };

                            await CommonDbContext.TagRelations.AddAsync(relation, cancellationToken);
                        }

                        await CommonDbContext.SaveChangesAsync(cancellationToken);
                    }
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,"result"), requestPath);
                return createdFieldPhoto.Id;
            }
            catch (Exception ex)
            {
                var msg = $"Error uploading field photo in {requestPath}: {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}
