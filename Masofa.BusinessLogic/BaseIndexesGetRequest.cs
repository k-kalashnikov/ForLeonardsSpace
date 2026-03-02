using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Masofa.BusinessLogic
{
    /// <summary>
    /// Базовая команда для получения списка сущностей с фильтом сортировкой и отступом, которые наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую удалили</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>
    [RequestPermission(ActionType = "Read")]
    public class BaseIndexesGetRequest<TModel> : IRequest<List<TModel>>
        where TModel : class
    {
        /// <summary>
        /// Модель запрос <see cref="../Masofa.Common/Models/BaseGetQuery.cs"/>
        /// </summary>
        public BaseGetQuery<TModel> Query { get; set; }
    }

    public class BaseIndexesGetRequestHandler<TModel> : IRequestHandler<BaseIndexesGetRequest<TModel>, List<TModel>>
        where TModel : class
    {
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BaseIndexesGetRequestHandler(MasofaIndicesDbContext dbContext, ILogger<BaseIndexesGetRequestHandler<TModel>> logger, IBusinessLogicLogger businessLogicLogger)
        {
            MasofaIndicesDbContext = dbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<List<TModel>> Handle(BaseIndexesGetRequest<TModel> request, CancellationToken cancellationToken)
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
