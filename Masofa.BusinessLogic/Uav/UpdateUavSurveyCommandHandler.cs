using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Models.Uav;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Uav
{
    public class UpdateUavSurveyCommandHandler : IRequestHandler<Commands.UpdateUavSurveyCommand, UAVFlyPath>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly ILogger<UpdateUavSurveyCommandHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public UpdateUavSurveyCommandHandler(
            MasofaUAVDbContext uavDbContext,
            MasofaIdentityDbContext identityDbContext,
            MasofaCommonDbContext commonDbContext,
            ILogger<UpdateUavSurveyCommandHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _uavDbContext = uavDbContext;
            _identityDbContext = identityDbContext;
            _commonDbContext = commonDbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<UAVFlyPath> Handle(Commands.UpdateUavSurveyCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var user = await _identityDbContext.Set<User>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.UserName.ToLower() == request.UserName.ToLower(), cancellationToken);

                if (user == null)
                    throw new InvalidOperationException($"User {request.UserName} not found");

                var entity = await _uavDbContext.Set<UAVFlyPath>()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    throw new KeyNotFoundException($"UAVFlyPath {request.Id} not found");

                entity.Comment = request.Comment;
                entity.DataTypeId = request.DataTypeId;
                entity.CameraTypeId = request.CameraTypeId;
                entity.LastUpdateAt = DateTime.UtcNow;
                entity.LastUpdateUser = user.Id;

                _uavDbContext.Set<UAVFlyPath>().Update(entity);
                await _uavDbContext.SaveChangesAsync(cancellationToken);

                await UpdateTagsAsync(entity.Id, request.TagIds, user.Id, cancellationToken);

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, $"Updated UAVFlyPath {entity.Id}"), requestPath);

                return entity;
            }
            catch (Exception ex)
            {
                var msg = $"Error updating UAV FlyPath: {ex.Message}";
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        private async Task UpdateTagsAsync(Guid ownerId, List<Guid> newTagIds, Guid userId, CancellationToken ct)
        {
            var ownerTypeFullName = typeof(UAVFlyPath).FullName ?? "Masofa.Common.Models.Uav.UAVFlyPath";

            var existingRelations = await _commonDbContext.TagRelations
                .Where(x => x.OwnerId == ownerId && x.OwnerTypeFullName == ownerTypeFullName)
                .ToListAsync(ct);

            var incomingTagIds = newTagIds.Distinct().ToList();

            foreach (var relation in existingRelations)
            {
                if (!incomingTagIds.Contains(relation.TagId))
                {
                    if (relation.Status != Masofa.Common.Models.StatusType.Deleted)
                    {
                        relation.Status = Masofa.Common.Models.StatusType.Deleted;
                        relation.LastUpdateAt = DateTime.UtcNow;
                        relation.LastUpdateUser = userId;
                    }
                }
                else
                {
                    if (relation.Status != Masofa.Common.Models.StatusType.Active)
                    {
                        relation.Status = Masofa.Common.Models.StatusType.Active;
                        relation.LastUpdateAt = DateTime.UtcNow;
                        relation.LastUpdateUser = userId;
                    }
                }
            }

            var existingTagIdsInDb = existingRelations.Select(x => x.TagId).ToHashSet();
            var tagsToCreate = incomingTagIds
                .Where(id => !existingTagIdsInDb.Contains(id))
                .ToList();

            if (tagsToCreate.Any())
            {
                var newRelations = tagsToCreate.Select(tagId => new TagRelation
                {
                    Id = Guid.NewGuid(),
                    TagId = tagId,
                    OwnerId = ownerId,
                    OwnerTypeFullName = ownerTypeFullName,
                    Status = Masofa.Common.Models.StatusType.Active,
                    CreateAt = DateTime.UtcNow,
                    CreateUser = userId,
                    LastUpdateAt = DateTime.UtcNow,
                    LastUpdateUser = userId
                }).ToList();

                await _commonDbContext.TagRelations.AddRangeAsync(newRelations, ct);
            }

            await _commonDbContext.SaveChangesAsync(ct);
        }
    }
}