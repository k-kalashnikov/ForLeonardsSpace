using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Era
{
    public class EraWeatherDataGetRequest : IRequest<List<EraWeatherData>>
    {
        public EraWeatherDataGetQuery Query { get; set; }
    }

    public class EraWeatherDataGetRequestHandler : IRequestHandler<EraWeatherDataGetRequest, List<EraWeatherData>>
    {
        private ILogger<EraWeatherDataGetRequestHandler> Logger { get; set; }
        private MasofaEraDbContext EraDbContext { get; set; }

        public EraWeatherDataGetRequestHandler(ILogger<EraWeatherDataGetRequestHandler> logger, MasofaEraDbContext eraDbContext)
        {
            Logger = logger;
            EraDbContext = eraDbContext;
        }

        public async Task<List<EraWeatherData>> Handle(EraWeatherDataGetRequest request, CancellationToken cancellation)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                IQueryable<EraWeatherData> resultQuery = EraDbContext.EraWeatherData;

                if (request.Query.Filters.Any())
                {
                    foreach (var item in request.Query.Filters)
                    {
                        resultQuery = resultQuery
                            .ApplyFiltering(item);
                    }
                }

                if (request.Query.Take.HasValue)
                {
                    resultQuery = resultQuery
                        .Skip(request.Query.Offset)
                        .Take(request.Query.Take.Value);
                }

                if (!string.IsNullOrEmpty(request.Query.SortBy))
                {
                    resultQuery = resultQuery
                        .ApplyOrdering(request.Query.SortBy, request.Query.Sort);
                }

                var result = await resultQuery.ToListAsync(cancellationToken: cancellation);
                return result;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }
}
