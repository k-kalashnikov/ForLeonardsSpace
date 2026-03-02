using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic
{
    /// <summary>
    /// Базовая команда для получения экземпляра сущности пл Id, которая наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую удалили</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>
    [RequestPermission(ActionType = "Read")]
    public class BaseGetByIdRequest<TModel, TDbContext> : IRequest<TModel>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        /// <summary>
        /// Id сущности
        /// </summary>
        public Guid Id { get; set; }
    }

    public class BaseGetByIdRequestHandler<TModel, TDbContext> : IRequestHandler<BaseGetByIdRequest<TModel, TDbContext>, TModel>
    where TModel : BaseEntity
    where TDbContext : DbContext
    {
        private TDbContext DbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BaseGetByIdRequestHandler(TDbContext dbContext, ILogger<BaseGetByIdRequestHandler<TModel, TDbContext>> logger, IBusinessLogicLogger businessLogicLogger)
        {
            DbContext = dbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<TModel> Handle(BaseGetByIdRequest<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                return await DbContext.Set<TModel>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id.Equals(request.Id));
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
