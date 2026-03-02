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
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic
{
    /// <summary>
    /// Базовая команда для создания сущностей, которые наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую нужно добавить</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>
    [RequestPermission(ActionType = "Create")]
    public class BaseCreateCommand<TModel, TDbContext> : IRequest<Guid>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        /// <summary>
        /// Модель которую надо добавить
        /// </summary>
        [Required]
        public TModel Model { get; set; }

        /// <summary>
        /// Пользователь-автор изменений
        /// </summary>
        [Required]
        public string Author { get; set; }
    }

    /// <summary>
    /// Базовое событие для создания сущностей, которые наследуются от <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую добавили</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>>
    public class BaseCreateEvent<TModel, TDbContext> : INotification
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        public TModel Model { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }

    public class BaseCreateCommandHandler<TModel, TDbContext> : IRequestHandler<BaseCreateCommand<TModel, TDbContext>, Guid>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {

        public TDbContext DbContext { get; set; }
        public MasofaIdentityDbContext IdentityDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IMediator Mediator { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BaseCreateCommandHandler(
            TDbContext dbContext,
            MasofaIdentityDbContext identityDbContext,
            ILogger<BaseCreateCommandHandler<TModel, TDbContext>> logger,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger)
        {
            DbContext = dbContext;
            IdentityDbContext = identityDbContext;
            Logger = logger;
            Mediator = mediator;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<Guid> Handle(BaseCreateCommand<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var model = request.Model;
                var lastUpdateUser = await IdentityDbContext.Set<User>().FirstAsync(m => m.UserName.ToLower().Equals(request.Author.ToLower()));

                model.CreateAt = DateTime.UtcNow;
                model.CreateUser = lastUpdateUser.Id;
                model.LastUpdateUser = lastUpdateUser.Id;
                model.LastUpdateAt = DateTime.UtcNow;
                model.Status = StatusType.Active;
                var result = await DbContext
                    .Set<TModel>()
                    .AddAsync(model);

                await DbContext.SaveChangesAsync();
                await Mediator.Publish(new BaseCreateEvent<TModel, TDbContext>()
                {
                    Model = result.Entity,
                    DateTime = DateTime.UtcNow,
                    UserId = lastUpdateUser.Id
                }, cancellationToken);
                return result.Entity.Id;
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
