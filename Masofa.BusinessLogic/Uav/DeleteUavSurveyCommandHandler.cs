using Masofa.BusinessLogic.Uav.Commands;
using Masofa.Common.Models;
using Masofa.Common.Models.Uav;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Uav.Handlers
{
    public class DeleteUavSurveyCommandHandler : IRequestHandler<DeleteUavSurveyCommand, bool>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly ILogger<DeleteUavSurveyCommandHandler> _logger;

        public DeleteUavSurveyCommandHandler(
            MasofaUAVDbContext uavDbContext,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            ILogger<DeleteUavSurveyCommandHandler> logger)
        {
            _uavDbContext = uavDbContext;
            _commonDbContext = commonDbContext;
            _identityDbContext = identityDbContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUavSurveyCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);

            var userId = user?.Id ?? Guid.Empty;

            var flyPath = await _uavDbContext.Set<UAVFlyPath>()
                .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);

            if (flyPath == null)
            {
                throw new KeyNotFoundException($"UAVFlyPath with Id {request.SurveyId} not found.");
            }

            var collections = await _uavDbContext.Set<UAVPhotoCollection>()
                .Where(c => c.UAVFlyPathId == flyPath.Id)
                .ToListAsync(cancellationToken);

            var collectionIds = collections.Select(c => c.Id).ToList();

            var photos = new List<UAVPhoto>();
            if (collectionIds.Any())
            {
                photos = await _uavDbContext.Set<UAVPhoto>()
                    .Where(p => collectionIds.Contains(p.UAVPhotoCollectionId))
                    .ToListAsync(cancellationToken);
            }

            var relations = new List<UAVPhotoCollectionRelation>();
            if (collectionIds.Any())
            {
                relations = await _uavDbContext.Set<UAVPhotoCollectionRelation>()
                    .Where(r => collectionIds.Contains(r.UAVPhotoCollectionId))
                    .ToListAsync(cancellationToken);
            }

            var fileStorageIds = photos
                .Where(p => p.FileStorageId != Guid.Empty)
                .Select(p => p.FileStorageId)
                .Distinct()
                .ToList();

            if (fileStorageIds.Any())
            {
                var filesStorageItems = await _commonDbContext.FileStorageItems
                    .Where(f => fileStorageIds.Contains(f.Id) && f.Status == StatusType.Active)
                    .ToListAsync(cancellationToken);

                foreach (var file in filesStorageItems)
                {
                    MarkAsDeleted(file, userId);
                }
            }

            var flyPathType = typeof(UAVFlyPath).FullName;
            var collectionType = typeof(UAVPhotoCollection).FullName;

            var tagsToDelete = await _commonDbContext.TagRelations
                .Where(tr =>
                    (tr.OwnerId == flyPath.Id && tr.OwnerTypeFullName == flyPathType) ||
                    (collectionIds.Contains(tr.OwnerId) && tr.OwnerTypeFullName == collectionType)
                )
                .Where(tr => tr.Status == StatusType.Active)
                .ToListAsync(cancellationToken);

            foreach (var tag in tagsToDelete)
            {
                MarkAsDeleted(tag, userId);
            }

            await _commonDbContext.SaveChangesAsync(cancellationToken);

            foreach (var rel in relations)
            {
                MarkAsDeleted(rel, userId);
            }

            foreach (var photo in photos)
            {
                MarkAsDeleted(photo, userId);
            }

            foreach (var collection in collections)
            {
                MarkAsDeleted(collection, userId);
            }

            MarkAsDeleted(flyPath, userId);

            await _uavDbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"User {request.UserName} soft-deleted UAVFlyPath {request.SurveyId} and related entities ({collections.Count} collections, {photos.Count} photos).");

            return true;
        }

        private void MarkAsDeleted(BaseEntity entity, Guid userId)
        {
            entity.Status = StatusType.Deleted;
            entity.LastUpdateAt = DateTime.UtcNow;
            entity.LastUpdateUser = userId;
        }
    }
}