using Masofa.Common.Models.Identity;
using Masofa.Common.Models.Uav;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;

namespace Masofa.BusinessLogic.Uav.Commands
{
    public class RecalculateUavSurveyRelationsCommand : IRequest<bool>
    {
        public Guid SurveyId { get; set; }
        public string? UserName { get; set; }
    }

    public class RecalculateUavSurveyRelationsCommandHandler : IRequestHandler<RecalculateUavSurveyRelationsCommand, bool>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaCropMonitoringDbContext _cropMonitoringDbContext;
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        private record SpatialAnalysisResult(
            Guid? RegionId,
            Guid? FieldId,
            Guid? SeasonId,
            Guid? CropId,
            Guid? FirmId
        );

        public RecalculateUavSurveyRelationsCommandHandler(
            MasofaUAVDbContext uavDbContext,
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaIdentityDbContext identityDbContext,
            IBusinessLogicLogger businessLogicLogger)
        {
            _uavDbContext = uavDbContext;
            _cropMonitoringDbContext = cropMonitoringDbContext;
            _dictionariesDbContext = dictionariesDbContext;
            _identityDbContext = identityDbContext;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<bool> Handle(RecalculateUavSurveyRelationsCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
            var strategy = _uavDbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _uavDbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var user = await _identityDbContext.Set<User>()
                        .FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);
                    var userId = user?.Id ?? Guid.Empty;
                    var flyPath = await _uavDbContext.Set<UAVFlyPath>()
                        .FirstOrDefaultAsync(x => x.Id == request.SurveyId, cancellationToken);
                    if (flyPath == null)
                    {
                        throw new KeyNotFoundException($"FlyPath with ID {request.SurveyId} not found.");
                    }
                    var collections = await _uavDbContext.Set<UAVPhotoCollection>()
                        .Where(c => c.UAVFlyPathId == request.SurveyId && c.Status == Masofa.Common.Models.StatusType.Active)
                        .Select(c => new
                        {
                            Collection = c,
                            PhotoDate = _uavDbContext.Set<UAVPhoto>()
                                .Where(p => p.UAVPhotoCollectionId == c.Id && p.Status == Masofa.Common.Models.StatusType.Active)
                                .Select(p => p.OriginalDate)
                                .FirstOrDefault()
                        })
                        .ToListAsync(cancellationToken);
                    if (!collections.Any())
                    {
                        await _businessLogicLogger.LogWarningAsync("No active photo collections found for this survey.", requestPath);
                        return true;
                    }
                    var collectionIds = collections.Select(x => x.Collection.Id).ToList();
                    var existingRelations = await _uavDbContext.Set<UAVPhotoCollectionRelation>()
                        .Where(r => collectionIds.Contains(r.UAVPhotoCollectionId) && r.Status == Masofa.Common.Models.StatusType.Active)
                        .ToListAsync(cancellationToken);
                    foreach (var rel in existingRelations)
                    {
                        rel.Status = Masofa.Common.Models.StatusType.Deleted;
                        rel.LastUpdateAt = DateTime.UtcNow;
                        rel.LastUpdateUser = userId;
                    }
                    var newRelations = new List<UAVPhotoCollectionRelation>();
                    foreach (var item in collections)
                    {
                        var point = item.Collection.Point;
                        if (point == null) continue;
                        var dateForAnalysis = item.PhotoDate.ToDateTime(TimeOnly.MinValue);
                        var spatialData = await AnalyzeSpatialData(point, dateForAnalysis);
                        if (spatialData.RegionId.HasValue || spatialData.FieldId.HasValue)
                        {
                            var newRelation = new UAVPhotoCollectionRelation
                            {
                                Id = Guid.NewGuid(),
                                UAVPhotoCollectionId = item.Collection.Id,
                                RegionId = spatialData.RegionId,
                                FieldId = spatialData.FieldId,
                                SeasonId = spatialData.SeasonId,
                                CropId = spatialData.CropId,
                                FirmId = spatialData.FirmId,
                                CreateAt = DateTime.UtcNow,
                                CreateUser = userId,
                                LastUpdateAt = DateTime.UtcNow,
                                LastUpdateUser = userId,
                                Status = Masofa.Common.Models.StatusType.Active
                            };
                            newRelations.Add(newRelation);
                        }
                    }
                    if (newRelations.Any())
                    {
                        await _uavDbContext.Set<UAVPhotoCollectionRelation>().AddRangeAsync(newRelations, cancellationToken);
                    }
                    await _uavDbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, $"Recalculated relations. Deleted: {existingRelations.Count}, Created: {newRelations.Count}"), requestPath);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                    await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                    throw;
                }
            });
        }

        private async Task<SpatialAnalysisResult> AnalyzeSpatialData(Point point, DateTime originalDate)
        {
            Guid? fieldId = null;
            Guid? regionId = null;
            Guid? seasonId = null;
            Guid? cropId = null;
            Guid? firmId = null;
            var field = await _cropMonitoringDbContext.Fields
                .FromSqlInterpolated($@"
                    SELECT * FROM ""Fields""
                    WHERE ""Status"" = {(int)Masofa.Common.Models.StatusType.Active}
                    AND ""Polygon"" IS NOT NULL
                    AND ST_Covers(ST_SetSRID(""Polygon""::geometry, 4326), {point})
                    LIMIT 1
                ")
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (field != null)
            {
                fieldId = field.Id;
                regionId = field.RegionId;
                firmId = field.AgricultureProducerId;
                var season = await _cropMonitoringDbContext.Seasons
                    .AsNoTracking()
                    .Where(s => s.FieldId == field.Id
                             && DateOnly.FromDateTime(originalDate) >= s.PlantingDate
                             && DateOnly.FromDateTime(originalDate) <= s.HarvestingDate)
                    .FirstOrDefaultAsync();
                if (season != null)
                {
                    seasonId = season.Id;
                    cropId = season.CropId;
                }
            }
            if (regionId == null)
            {
                var regionMap = await _dictionariesDbContext.RegionMaps
                    .FromSqlInterpolated($@"
                        SELECT * FROM ""RegionMaps""
                        WHERE ""Polygon"" IS NOT NULL
                        AND ST_Covers(ST_SetSRID(""Polygon""::geometry, 4326), {point})
                        ORDER BY ST_Area(""Polygon"") ASC
                        LIMIT 1
                    ")
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (regionMap != null)
                {
                    var region = await _dictionariesDbContext.Regions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.RegionMapId == regionMap.Id);
                    if (region != null)
                    {
                        regionId = region.Id;
                    }
                }
            }
            return new SpatialAnalysisResult(regionId, fieldId, seasonId, cropId, firmId);
        }
    }
}