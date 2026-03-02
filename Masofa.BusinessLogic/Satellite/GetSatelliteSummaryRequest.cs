using Masofa.BusinessLogic.Attributes;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Satellite
{
    [RequestPermission(ActionType = "Read")]
    public class GetSatelliteSummaryRequest : IRequest<GetSatelliteSummaryResponse>
    {
    }

    public class GetSatelliteSummaryResponse
    {
        public int TotalProducts { get; set; }
        public int Sentinel2Count { get; set; }
        public int LandsatCount { get; set; }
        public DateTime? LastUpdateUtc { get; set; }
    }

    public class GetSatelliteSummaryRequestHandler : IRequestHandler<GetSatelliteSummaryRequest, GetSatelliteSummaryResponse>
    {
        private readonly MasofaCommonDbContext _dbCommonContext;

        public GetSatelliteSummaryRequestHandler(MasofaCommonDbContext dbCommonContext)
        {
            _dbCommonContext = dbCommonContext;
        }

        public async Task<GetSatelliteSummaryResponse> Handle(GetSatelliteSummaryRequest request, CancellationToken cancellationToken)
        {
            var query = _dbCommonContext.Set<SatelliteProduct>().AsNoTracking().Where(p => p.Status == StatusType.Active);

            var total = await query.CountAsync(cancellationToken);
            var sentinel = await query.Where(p => p.ProductSourceType == ProductSourceType.Sentinel2 && p.Status == StatusType.Active).CountAsync(cancellationToken);
            var landsat = await query.Where(p => p.ProductSourceType == ProductSourceType.Landsat && p.Status == StatusType.Active).CountAsync(cancellationToken);

            // Определяем последнюю дату обновления по LastUpdateAt/CreateAt если доступно
            var lastUpdate = await query
                .Select(p => p.LastUpdateAt > p.CreateAt ? p.LastUpdateAt : p.CreateAt)
                .MaxAsync(cancellationToken);

            return new GetSatelliteSummaryResponse
            {
                TotalProducts = total,
                Sentinel2Count = sentinel,
                LandsatCount = landsat,
                LastUpdateUtc = lastUpdate
            };
        }
    }
}


