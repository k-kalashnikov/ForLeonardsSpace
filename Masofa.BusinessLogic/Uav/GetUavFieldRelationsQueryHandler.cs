using Masofa.BusinessLogic.Uav.Queries;
using Masofa.Common.Models;
using Masofa.Common.Models.Uav;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Uav.Handlers
{
    public class GetUavFieldRelationsQueryHandler : IRequestHandler<GetUavFieldRelationsQuery, List<UavSurveyFieldRelationDto>>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaCropMonitoringDbContext _cropDbContext;
        private readonly ILogger<GetUavFieldRelationsQueryHandler> _logger;

        public GetUavFieldRelationsQueryHandler(
        MasofaUAVDbContext uavDbContext,
        MasofaCropMonitoringDbContext cropDbContext,
        ILogger<GetUavFieldRelationsQueryHandler> logger)
        {
            _uavDbContext = uavDbContext;
            _cropDbContext = cropDbContext;
            _logger = logger;
        }

        public async Task<List<UavSurveyFieldRelationDto>> Handle(GetUavFieldRelationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var rawRelations = await _uavDbContext.Set<UAVPhotoCollection>()
                    .AsNoTracking()
                    .Where(c => c.UAVFlyPathId == request.SurveyId)
                    .Join(_uavDbContext.Set<UAVPhotoCollectionRelation>(),
                        collection => collection.Id,
                        relation => relation.UAVPhotoCollectionId,
                        (collection, relation) => relation)
                    .Where(r => r.Status != StatusType.Deleted && r.FieldId != null)
                    .Select(r => new
                    {
                        Relation = r,
                        FieldId = r.FieldId.Value
                    })
                    .ToListAsync(cancellationToken);
                var uniqueFieldIds = rawRelations
                    .Select(x => x.FieldId)
                    .Distinct()
                    .ToList();
                var fieldsGeometry = await _cropDbContext.Fields
                    .AsNoTracking()
                    .Where(f => uniqueFieldIds.Contains(f.Id))
                    .Select(f => new { f.Id, Polygon = f.Polygon })
                    .ToListAsync(cancellationToken);
                var result = rawRelations
                    .GroupBy(r => r.FieldId)
                    .Select(g => g.First())
                    .Select(item =>
                    {
                        var geo = fieldsGeometry.FirstOrDefault(f => f.Id == item.FieldId);
                        return new UavSurveyFieldRelationDto
                        {
                            Id = item.Relation.Id,
                            FieldId = item.Relation.FieldId,
                            CropId = item.Relation.CropId,
                            SeasonId = item.Relation.SeasonId,
                            FirmId = item.Relation.FirmId,
                            FieldPolygonWkt = geo?.Polygon?.ToString()
                        };
                    })
                    .ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting field relations for survey {request.SurveyId}");
                throw;
            }
        }
    }
}