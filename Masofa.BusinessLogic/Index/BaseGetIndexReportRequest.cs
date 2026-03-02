using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Index
{
    [RequestPermission(ActionType = "Read")]
    public class BaseGetIndexReportRequest<TModel> : IRequest<List<TModel>>
        where TModel : BaseIndexReport
    {
        /// <summary>
        /// Модель запрос <see cref="../Masofa.Common/Models/BaseGetQuery.cs"/>
        /// </summary>
        public BaseGetQuery<TModel> Query { get; set; }
    }

    public class BaseGetIndexRequestHandler<TModel> : IRequestHandler<BaseGetIndexReportRequest<TModel>, List<TModel>>
        where TModel : BaseIndexReport
    {
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BaseGetIndexRequestHandler(ILogger<BaseGetIndexRequestHandler<TModel>> logger, IBusinessLogicLogger businessLogicLogger, MasofaIndicesDbContext masofaIndicesDbContext)
        {
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
            MasofaIndicesDbContext = masofaIndicesDbContext;
        }

        public async Task<List<TModel>> Handle(BaseGetIndexReportRequest<TModel> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                IQueryable<TModel> resultQuery = MasofaIndicesDbContext.Set<TModel>().AsNoTracking();

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
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);
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
