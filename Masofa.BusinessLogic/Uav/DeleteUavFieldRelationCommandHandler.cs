using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.Uav.Commands;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.Uav;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Uav.Handlers
{
    public class DeleteUavFieldRelationCommandHandler : IRequestHandler<DeleteUavFieldRelationCommand, bool>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly ILogger<DeleteUavFieldRelationCommandHandler> _logger;

        public DeleteUavFieldRelationCommandHandler(
            MasofaUAVDbContext uavDbContext,
            MasofaIdentityDbContext identityDbContext,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<DeleteUavFieldRelationCommandHandler> logger)
        {
            _uavDbContext = uavDbContext;
            _identityDbContext = identityDbContext;
            _businessLogicLogger = businessLogicLogger;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUavFieldRelationCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
            try
            {
                var user = await _identityDbContext.Set<User>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);
                var userId = user?.Id ?? Guid.Empty;
                var surveyExists = await _uavDbContext.Set<UAVFlyPath>()
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == request.SurveyId, cancellationToken);
                if (!surveyExists)
                {
                    throw new KeyNotFoundException($"FlyPath (Survey) with ID {request.SurveyId} not found");
                }
                var surveyCollectionIds = await _uavDbContext.Set<UAVPhotoCollection>()
                    .AsNoTracking()
                    .Where(c => c.UAVFlyPathId == request.SurveyId)
                    .Select(c => c.Id)
                    .ToListAsync(cancellationToken);
                if (!surveyCollectionIds.Any())
                {
                    await _businessLogicLogger.LogWarningAsync($"No photo collections found for survey {request.SurveyId}.", requestPath);
                    return false;
                }
                var relationsToDelete = await _uavDbContext.Set<UAVPhotoCollectionRelation>()
                    .Where(r => surveyCollectionIds.Contains(r.UAVPhotoCollectionId)
                                && r.FieldId == request.FieldId
                                && r.Status != StatusType.Deleted)
                    .ToListAsync(cancellationToken);

                if (!relationsToDelete.Any())
                {
                    await _businessLogicLogger.LogInformationAsync($"No active relations found for Field {request.FieldId} in Survey {request.SurveyId}.", requestPath);
                    return false;
                }
                var now = DateTime.UtcNow;
                foreach (var relation in relationsToDelete)
                {
                    relation.Status = StatusType.Deleted;
                    relation.LastUpdateAt = now;
                    relation.LastUpdateUser = userId;
                }
                _uavDbContext.Set<UAVPhotoCollectionRelation>().UpdateRange(relationsToDelete);
                await _uavDbContext.SaveChangesAsync(cancellationToken);

                await _businessLogicLogger.LogInformationAsync(
                    LogMessageResource.RequestFinishedWithResult(requestPath,
                    $"Soft Deleted {relationsToDelete.Count} relations for Survey {request.SurveyId} and Field {request.FieldId}"),
                    requestPath);

                return true;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}