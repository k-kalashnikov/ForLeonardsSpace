using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Index
{
    public class CustomGetAnomalyPolygonsCommand : IRequest<List<CustomAnomalyPolygonsViewModel>>
    {
        public BaseGetQuery<AnomalyPolygon> GetQuery { get; set; }
        public List<Guid>? CropIds { get; set; }
        public List<Guid>? RegionIds { get; set; }
    }

    public class CustomGetAnomalyPolygonsCommandHandler : IRequestHandler<CustomGetAnomalyPolygonsCommand, List<CustomAnomalyPolygonsViewModel>>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<CustomGetAnomalyPolygonsCommandHandler> Logger { get; set; }
        private IMediator Mediator { get; set; }

        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }
        //private MasofaIndicesDbContext IndicesDbContext { get; set; }

        public CustomGetAnomalyPolygonsCommandHandler(IBusinessLogicLogger businessLogicLogger, ILogger<CustomGetAnomalyPolygonsCommandHandler> logger, IMediator mediator, MasofaCropMonitoringDbContext cropMonitoringDbContext, MasofaDictionariesDbContext dictionariesDbContext)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            Mediator = mediator;
            CropMonitoringDbContext = cropMonitoringDbContext;
            DictionariesDbContext = dictionariesDbContext;
        }

        public async Task<List<CustomAnomalyPolygonsViewModel>> Handle(CustomGetAnomalyPolygonsCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var result = new List<CustomAnomalyPolygonsViewModel>();

                var anomalyPolygons = await Mediator.Send(new BaseGetRequest<AnomalyPolygon, MasofaIndicesDbContext>()
                {
                    Query = request.GetQuery
                }, cancellationToken);

                if (anomalyPolygons.Count == 0)
                {
                    return result;
                }

                var fieldIds = anomalyPolygons.Where(ap => ap.FieldId != null).Select(ap => ap.FieldId).ToList();
                var seasonIds = anomalyPolygons.Where(ap => ap.SeasonId != null).Select(ap => ap.SeasonId).ToList();

                var fields = await CropMonitoringDbContext.Fields
                    .Where(f => fieldIds.Contains(f.Id))
                    .ToListAsync(cancellationToken);

                var seasons = await CropMonitoringDbContext.Seasons
                    .Where(s => seasonIds.Contains(s.Id))
                    .ToListAsync(cancellationToken);

                if (request.CropIds?.Count > 0)
                {
                    var filteredSeasonsIds = seasons
                        .Where(s => s.CropId != null && request.CropIds.Contains(s.CropId.Value))
                        .Select(s => s.Id)
                        .ToList();

                    anomalyPolygons = anomalyPolygons
                        .Where(ap => ap.SeasonId != null && filteredSeasonsIds.Contains(ap.SeasonId.Value))
                        .ToList();
                }

                if (request.RegionIds?.Count > 0)
                {
                    anomalyPolygons = anomalyPolygons
                        .Where(ap => ap.RegionId != null && request.RegionIds.Contains(ap.RegionId.Value))
                        .ToList();
                }

                if (anomalyPolygons.Count == 0)
                {
                    return result;
                }

                var regionIds = anomalyPolygons.Where(ap => ap.RegionId != null).Select(ap => ap.RegionId).ToList();

                var regions = await DictionariesDbContext.Regions
                    .Where(r => regionIds.Contains(r.Id))
                    .ToListAsync(cancellationToken);

                var cropIds = seasons.Where(s => s.CropId != null).Select(s => s.CropId).ToList();

                var crops = await DictionariesDbContext.Crops
                    .Where(c => cropIds.Contains(c.Id))
                    .ToListAsync(cancellationToken);

                foreach (var ap in anomalyPolygons)
                {
                    CustomAnomalyPolygonsViewModel capvm = new()
                    {
                        AnomalyPolygon = ap
                    };

                    if (ap.RegionId != null)
                    {
                        var region = regions.FirstOrDefault(r => r.Id == ap.RegionId.Value);
                        if (region != null)
                        {
                            capvm.RegionName = region.Names;
                        }
                    }

                    if (ap.FieldId != null)
                    {
                        var field = fields.FirstOrDefault(f => f.Id == ap.FieldId.Value);
                        if (field != null && field.Name != null)
                        {
                            capvm.FieldName = field.Name;
                        }
                    }

                    if (ap.SeasonId != null)
                    {
                        var season = seasons.FirstOrDefault(s => s.Id == ap.SeasonId.Value);
                        if (season != null && season.CropId != null)
                        {
                            var crop = crops.FirstOrDefault(c => c.Id == season.CropId);
                            if (crop != null)
                            {
                                capvm.CropName = crop.Names;
                            }
                        }
                    }

                    result.Add(capvm);
                }

                return result;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }

    public class CustomAnomalyPolygonsViewModel
    {
        public AnomalyPolygon AnomalyPolygon { get; set; }
        public LocalizationString RegionName { get; set; } = new LocalizationString();
        public string FieldName { get; set; } = string.Empty;
        public LocalizationString CropName { get; set; } = new LocalizationString();
    }
}
