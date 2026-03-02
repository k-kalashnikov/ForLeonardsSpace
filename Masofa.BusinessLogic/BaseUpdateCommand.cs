using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic
{
    /// <summary>
    /// Базовая команда для обновления сущностей, которые наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую удалили</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>
    [RequestPermission(ActionType = "Update")]
    public class BaseUpdateCommand<TModel, TDbContext> : IRequest<TModel>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        /// <summary>
        /// Модель которую надо добавить
        /// </summary>
        public TModel Model { get; set; }

        /// <summary>
        /// Модель которую надо добавить
        /// </summary>
        public string Author { get; set; }
    }

    /// <summary>
    /// Базовое событие которое срабатывает при обновлении сущностей, которые наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую удалили</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>
    public class BaseUpdateEvent<TModel, TDbContext> : INotification
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        public TModel CurrentModel { get; set; }
        public TModel OldModel { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }

    public class BaseUpdateCommandHandler<TModel, TDbContext> : IRequestHandler<BaseUpdateCommand<TModel, TDbContext>, TModel>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {

        public TDbContext DbContext { get; set; }
        public MasofaIdentityDbContext IdentityDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IMediator Mediator { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger;

        public BaseUpdateCommandHandler(
            TDbContext dbContext,
            MasofaIdentityDbContext identityDbContext,
            ILogger<BaseUpdateCommandHandler<TModel, TDbContext>> logger,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger)
        {
            DbContext = dbContext;
            IdentityDbContext = identityDbContext;
            Logger = logger;
            Mediator = mediator;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<TModel> Handle(BaseUpdateCommand<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var oldModel = DbContext.Set<TModel>()
                    .AsNoTracking()
                    .FirstOrDefault(m => m.Id == request.Model.Id);

                if (oldModel == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(TModel).FullName, request.Model.Id.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    throw new Exception(errorMsg);
                }

                var model = request.Model;
                var lastUpdateUser = IdentityDbContext.Set<User>().First(m => m.UserName.ToLower().Equals(request.Author.ToLower()));
                model.CreateUser = oldModel.CreateUser;
                model.CreateAt = oldModel.CreateAt;
                model.LastUpdateAt = DateTime.UtcNow;
                model.LastUpdateUser = lastUpdateUser.Id;
                var result = DbContext.Set<TModel>().Update(model);

                await DbContext.SaveChangesAsync();
                await Mediator.Publish(new BaseUpdateEvent<TModel, TDbContext>()
                {
                    CurrentModel = model,
                    OldModel = oldModel,
                    DateTime = DateTime.UtcNow,
                    UserId = lastUpdateUser.Id
                });
                return result.Entity;
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
