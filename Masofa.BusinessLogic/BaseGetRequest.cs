using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;

namespace Masofa.BusinessLogic
{
    /// <summary>
    /// Базовая команда для получения списка сущностей с фильтом сортировкой и отступом, которые наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую удалили</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>
    [RequestPermission(ActionType = "Read")]
    public class BaseGetRequest<TModel, TDbContext> : IRequest<List<TModel>>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        /// <summary>
        /// Модель запрос <see cref="../Masofa.Common/Models/BaseGetQuery.cs"/>
        /// </summary>
        public BaseGetQuery<TModel> Query { get; set; }
    }

    public class BaseGetRequestHandler<TModel, TDbContext> : IRequestHandler<BaseGetRequest<TModel, TDbContext>, List<TModel>>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        private TDbContext DbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BaseGetRequestHandler(TDbContext dbContext, ILogger<BaseGetRequestHandler<TModel, TDbContext>> logger, IBusinessLogicLogger businessLogicLogger)
        {
            DbContext = dbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<List<TModel>> Handle(BaseGetRequest<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                IQueryable<TModel> resultQuery = DbContext.Set<TModel>().AsNoTracking().Where(m => m.Status == StatusType.Active);

                if (request.Query.Filters.Any())
                {
                    foreach (var item in request.Query.Filters)
                    {
                        resultQuery = resultQuery
                            .ApplyFiltering(item);
                    }
                }

                if (!string.IsNullOrEmpty(request.Query.SortBy))
                {
                    resultQuery = resultQuery
                        .ApplyOrdering(request.Query.SortBy, request.Query.Sort);
                }

                if (request.Query.Take.HasValue)
                {
                    resultQuery = resultQuery
                        .Skip(request.Query.Offset)
                        .Take(request.Query.Take.Value);
                }

                if (request.Query.SelectFields.Any())
                {
                    resultQuery = resultQuery.ApplySelectFields(request.Query.SelectFields);
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
