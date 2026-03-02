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
    /// Базовая команда для удаления сущностей, которые наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>. Модель не удаляется из БД, а помечаяется как удалённое
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую нужно удалить</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>
    [RequestPermission(ActionType = "Delete")]
    public class BaseDeleteCommand<TModel, TDbContext> : IRequest<bool>
    {
        /// <summary>
        /// Id модели, которую надо добавить
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Пользователь-автор изменений
        /// </summary>
        public string Author { get; set; }
    }

    /// <summary>
    /// Базовое событие которое срабатывает при удалении сущностей, которые наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую удалили</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>
    public class BaseDeleteEvent<TModel, TDbContext> : INotification
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        public TModel Model { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }

    public class BaseDeleteCommandHandler<TModel, TDbContext> : IRequestHandler<BaseDeleteCommand<TModel, TDbContext>, bool>
    where TModel : BaseEntity
    where TDbContext : DbContext
    {

        public TDbContext DbContext { get; set; }
        public MasofaIdentityDbContext IdentityDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IMediator Mediator { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BaseDeleteCommandHandler(
            TDbContext dbContext,
            MasofaIdentityDbContext identityDbContext,
            ILogger<BaseDeleteCommandHandler<TModel, TDbContext>> logger,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger)
        {
            DbContext = dbContext;
            IdentityDbContext = identityDbContext;
            Logger = logger;
            Mediator = mediator;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<bool> Handle(BaseDeleteCommand<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                var model = DbContext.Set<TModel>().FirstOrDefault(m => m.Id.Equals(request.Id));

                if (model == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(TModel).FullName, request.Id.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    throw new Exception(errorMsg);
                }

                var lastUpdateUser = IdentityDbContext.Set<User>().First(m => m.UserName.ToLower().Equals(request.Author.ToLower()));

                model.LastUpdateAt = DateTime.UtcNow;
                model.LastUpdateUser = lastUpdateUser.Id;
                model.Status = StatusType.Deleted;
                var result = DbContext
                    .Set<TModel>()
                    .Update(model);

                DbContext.SaveChanges();
                await Mediator.Publish(new BaseDeleteEvent<TModel, TDbContext>()
                {
                    Model = model,
                    DateTime = DateTime.UtcNow,
                    UserId = lastUpdateUser.Id
                });
                return true;
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

