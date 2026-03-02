using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Dictionaries
{
    public class GetCropIndicesRequest : IRequest<List<CropIndices>>
    {
        [Required]
        public required VegetationIndexType VegetationIndexType { get; set; }

        [Required]
        public required Guid CropId { get; set; }
    }

    public class GetCropIndicesRequestHandler : IRequestHandler<GetCropIndicesRequest, List<CropIndices>>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<GetCropIndicesRequestHandler> Logger { get; set; }

        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }
        public GetCropIndicesRequestHandler(
            MasofaDictionariesDbContext dictionariesDbContext,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<GetCropIndicesRequestHandler> logger)
        {
            DictionariesDbContext = dictionariesDbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<List<CropIndices>> Handle(GetCropIndicesRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                List<CropIndices> result = [];

                var cropPeriods = await DictionariesDbContext.CropPeriods
                    .Where(cp => cp.CropId == request.CropId)
                    .ToListAsync(cancellationToken);
                if (cropPeriods.Count == 0)
                {
                    return result;
                }

                var cropPeriodVegetationIndexes = await DictionariesDbContext.CropPeriodVegetationIndexes
                    .Where(cpvi => cropPeriods.Select(cp => cp.Id).Contains(cpvi.CropPeriodId) && cpvi.VegetationIndexType == request.VegetationIndexType)
                    .ToListAsync(cancellationToken);
                if (cropPeriodVegetationIndexes.Count == 0)
                {
                    return result;
                }

                foreach (var cropPeriodVegetationIndex in cropPeriodVegetationIndexes)
                {
                    var cropPeriod = cropPeriods.FirstOrDefault(cp => cp.Id == cropPeriodVegetationIndex.CropPeriodId);
                    if (cropPeriod == null)
                    {
                        continue;
                    }

                    if (cropPeriod.DayStart == null && cropPeriod.DayEnd == null)
                    {
                        continue;
                    }
                    result.Add(new CropIndices
                    {
                        DayStart = cropPeriod.DayStart,
                        DayEnd = cropPeriod.DayEnd,
                        IndexValue = (float)cropPeriodVegetationIndex.Value,
                        IndexMax = (float)cropPeriodVegetationIndex.Max,
                        IndexMin = (float)cropPeriodVegetationIndex.Min
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }

    public class CropIndices
    {
        public int? DayStart { get; set; }
        public int? DayEnd { get; set; }
        public float? IndexValue { get; set; }
        public float? IndexMin { get; set; }
        public float? IndexMax { get; set; }
    }
}
