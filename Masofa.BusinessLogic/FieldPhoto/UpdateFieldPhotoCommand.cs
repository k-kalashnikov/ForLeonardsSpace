using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Masofa.BusinessLogic.FieldPhotoRequest
{
    public class UpdateFieldPhotoCommand : IRequest<FieldPhoto>
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? CaptureDateUtc { get; set; }
        public IEnumerable<Guid> TagIds { get; set; } = Enumerable.Empty<Guid>();
        public required string Author { get; set; }
    }

    public class UpdateFieldPhotoCommandHandler : IRequestHandler<UpdateFieldPhotoCommand, FieldPhoto>
    {
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<UpdateFieldPhotoCommandHandler> Logger { get; set; }
        private IMediator Mediator { get; set; }

        public UpdateFieldPhotoCommandHandler(
            IBusinessLogicLogger businessLogicLogger,
            ILogger<UpdateFieldPhotoCommandHandler> logger,
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

        public async Task<FieldPhoto> Handle(UpdateFieldPhotoCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync($"Start update field photo in {requestPath}", requestPath);

                var lastUpdateUser = await IdentityDbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.UserName != null && m.UserName.ToLower().Equals(request.Author.ToLower()), cancellationToken);

                if (lastUpdateUser == null)
                {
                    var errorMsg = $"User '{request.Author}' not found.";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    throw new InvalidOperationException(errorMsg);
                }

                var existingPhoto = await CropMonitoringDbContext.FieldPhotos
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (existingPhoto == null)
                {
                    var errorMsg = $"Field photo with ID {request.Id} not found.";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    throw new InvalidOperationException(errorMsg);
                }

                // Обновляем поля фото
                if (request.Title != null)
                {
                    existingPhoto.Title = request.Title;
                }
                if (request.Description != null)
                {
                    existingPhoto.Description = request.Description;
                }
                if (request.CaptureDateUtc.HasValue)
                {
                    existingPhoto.CaptureDateUtc = request.CaptureDateUtc.Value;
                }

                existingPhoto.LastUpdateAt = DateTime.UtcNow;
                existingPhoto.LastUpdateUser = lastUpdateUser.Id;

                CropMonitoringDbContext.FieldPhotos.Update(existingPhoto);
                await CropMonitoringDbContext.SaveChangesAsync(cancellationToken);

                // Обновляем теги
                var tagIds = (request.TagIds ?? Enumerable.Empty<Guid>()).Distinct().ToList();
                var ownerTypeFullName = typeof(FieldPhoto).FullName ?? string.Empty;

                // Удаляем существующие связи с тегами
                var existingRelations = await CommonDbContext.TagRelations
                    .Where(tr => tr.OwnerId == request.Id &&
                                 tr.OwnerTypeFullName == ownerTypeFullName)
                    .ToListAsync(cancellationToken);

                if (existingRelations.Count > 0)
                {
                    CommonDbContext.TagRelations.RemoveRange(existingRelations);
                    await CommonDbContext.SaveChangesAsync(cancellationToken);
                }

                // Добавляем новые связи с тегами
                if (tagIds.Count > 0)
                {
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
                                OwnerId = request.Id,
                                OwnerTypeFullName = ownerTypeFullName,
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

                await BusinessLogicLogger.LogInformationAsync($"Field photo updated successfully with ID: {existingPhoto.Id}", requestPath);
                return existingPhoto;
            }
            catch (Exception ex)
            {
                var msg = $"Error updating field photo in {requestPath}: {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}

