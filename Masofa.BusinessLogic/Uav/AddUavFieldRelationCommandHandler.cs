using Masofa.BusinessLogic.Uav.Commands;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.Uav;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Uav.Handlers
{
    public class AddUavFieldRelationCommandHandler : IRequestHandler<AddUavFieldRelationCommand, Guid>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaCropMonitoringDbContext _cropMonitoringDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly ILogger<AddUavFieldRelationCommandHandler> _logger;

        public AddUavFieldRelationCommandHandler(
            MasofaUAVDbContext uavDbContext,
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            MasofaIdentityDbContext identityDbContext,
            ILogger<AddUavFieldRelationCommandHandler> logger)
        {
            _uavDbContext = uavDbContext;
            _cropMonitoringDbContext = cropMonitoringDbContext;
            _identityDbContext = identityDbContext;
            _logger = logger;
        }

        public async Task<Guid> Handle(AddUavFieldRelationCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityDbContext.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);
            var userId = user?.Id ?? Guid.Empty;
            var surveyExists = await _uavDbContext.Set<UAVFlyPath>()
                .AnyAsync(s => s.Id == request.SurveyId, cancellationToken);

            if (!surveyExists)
            {
                throw new KeyNotFoundException($"FlyPath (Survey) with ID {request.SurveyId} not found");
            }
            var fieldInfo = await _cropMonitoringDbContext.Fields
                .AsNoTracking()
                .Where(f => f.Id == request.FieldId)
                .Select(f => new
                {
                    f.Id,
                    f.Polygon,
                    f.RegionId,
                    f.AgricultureProducerId
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (fieldInfo == null)
            {
                throw new KeyNotFoundException($"Field {request.FieldId} not found");
            }
            if (fieldInfo.Polygon == null || fieldInfo.Polygon.IsEmpty)
            {
                throw new InvalidOperationException($"Field {request.FieldId} does not have a valid polygon for spatial check.");
            }
            var collections = await _uavDbContext.Set<UAVPhotoCollection>()
                .Where(c => c.UAVFlyPathId == request.SurveyId && c.Status == StatusType.Active)
                .ToListAsync(cancellationToken);

            if (!collections.Any())
            {
                throw new InvalidOperationException("Survey has no active photo collections.");
            }
            var collectionIds = collections.Select(c => c.Id).ToList();
            var existingRelations = await _uavDbContext.Set<UAVPhotoCollectionRelation>()
                .Where(r => collectionIds.Contains(r.UAVPhotoCollectionId) && r.FieldId == request.FieldId)
                .ToListAsync(cancellationToken);
            var newRelations = new List<UAVPhotoCollectionRelation>();
            int addedCount = 0;
            int restoredCount = 0;
            foreach (var collection in collections)
            {
                if (collection.Point == null) continue;
                if (fieldInfo.Polygon.Covers(collection.Point))
                {
                    var existingRelation = existingRelations
                        .FirstOrDefault(r => r.UAVPhotoCollectionId == collection.Id);
                    if (existingRelation != null)
                    {
                        if (existingRelation.Status == StatusType.Deleted)
                        {
                            existingRelation.Status = StatusType.Active;
                            existingRelation.LastUpdateAt = DateTime.UtcNow;
                            existingRelation.LastUpdateUser = userId;
                            existingRelation.CropId = request.CropId;
                            restoredCount++;
                        }
                        else if (request.CropId.HasValue && existingRelation.CropId != request.CropId)
                        {
                            existingRelation.CropId = request.CropId;
                            existingRelation.LastUpdateAt = DateTime.UtcNow;
                            existingRelation.LastUpdateUser = userId;
                        }
                    }
                    else
                    {
                        newRelations.Add(new UAVPhotoCollectionRelation
                        {
                            Id = Guid.NewGuid(),
                            UAVPhotoCollectionId = collection.Id,
                            FieldId = request.FieldId,
                            RegionId = fieldInfo.RegionId,
                            FirmId = fieldInfo.AgricultureProducerId,
                            CropId = request.CropId,
                            CreateAt = DateTime.UtcNow,
                            CreateUser = userId,
                            LastUpdateAt = DateTime.UtcNow,
                            LastUpdateUser = userId,
                            Status = StatusType.Active
                        });
                        addedCount++;
                    }
                }
            }

            if (newRelations.Any())
            {
                await _uavDbContext.Set<UAVPhotoCollectionRelation>().AddRangeAsync(newRelations, cancellationToken);
            }
            await _uavDbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"User {request.UserName} processed field link for survey {request.SurveyId}. Added: {addedCount}, Restored: {restoredCount} relations based on spatial overlap.");
            var resultId = newRelations.FirstOrDefault()?.Id ?? existingRelations.FirstOrDefault()?.Id ?? Guid.Empty;
            return resultId;
        }
    }
}