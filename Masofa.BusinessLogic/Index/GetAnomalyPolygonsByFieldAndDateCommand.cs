using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Index
{
    public class GetAnomalyPolygonsByFieldAndDateCommand : IRequest<List<AnomalyPolygon>>
    {
        [Required]
        public required Guid FieldId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public List<AnomalyType> AnomalyTypes { get; set; }
    }

    public class GetPolygonsByFieldAndDateCommandHandler : IRequestHandler<GetAnomalyPolygonsByFieldAndDateCommand, List<AnomalyPolygon>>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<GetPolygonsByFieldAndDateCommandHandler> Logger { get; set; }
        private MasofaIndicesDbContext IndicesDbContext { get; set; }

        public GetPolygonsByFieldAndDateCommandHandler(
            IBusinessLogicLogger businessLogicLogger, 
            ILogger<GetPolygonsByFieldAndDateCommandHandler> logger, 
            MasofaIndicesDbContext indicesDbContext)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            IndicesDbContext = indicesDbContext;
        }

        public async Task<List<AnomalyPolygon>> Handle(GetAnomalyPolygonsByFieldAndDateCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var query = IndicesDbContext.AnomalyPolygons.AsNoTracking();
                query = query.Where(ap => ap.Status == StatusType.Active);
                query = query.Where(ap => ap.FieldId == request.FieldId && DateOnly.FromDateTime(ap.OriginalDate) == request.Date);
                if (request.AnomalyTypes.Count != 0)
                {
                    query = query.Where(ap => request.AnomalyTypes.Contains(ap.AnomalyType));
                }

                var result = await query.ToListAsync(cancellationToken);

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
}
