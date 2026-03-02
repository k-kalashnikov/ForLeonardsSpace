using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Masofa.BusinessLogic.Era
{
    public class BaseEra5WeatherReportGetRequest<TModel> : IRequest<List<TModel>>
        where TModel : BaseEra5WeatherReport
    {
        /// <summary>
        /// Модель запрос <see cref="../Masofa.Common/Models/Era/BaseEra5WeatherReportQuery.cs"/>
        /// </summary>
        public BaseEra5WeatherReportQuery<TModel> Query { get; set; }
    }

    public class BaseEra5WeatherReportGetRequestHandler<TModel> : IRequestHandler<BaseEra5WeatherReportGetRequest<TModel>, List<TModel>>
        where TModel : BaseEra5WeatherReport
    {
        private MasofaEraDbContext DbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BaseEra5WeatherReportGetRequestHandler(MasofaEraDbContext dbContext, ILogger<BaseEra5WeatherReportGetRequestHandler<TModel>> logger, IBusinessLogicLogger businessLogicLogger)
        {
            DbContext = dbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<List<TModel>> Handle(BaseEra5WeatherReportGetRequest<TModel> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                IQueryable<TModel> resultQuery = DbContext.Set<TModel>().AsNoTracking();

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

                var result = resultQuery?.ToList() ?? new List<TModel>();
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.Count.ToString()), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }
}
