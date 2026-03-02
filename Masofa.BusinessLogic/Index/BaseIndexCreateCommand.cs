using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Tls;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Index
{
    [RequestPermission(ActionType = "Create")]
    public class BaseIndexCreateCommand<TModel> : IRequest<Guid>
        where TModel : BaseIndexReport
    {
        /// <summary>
        /// Модель которую надо добавить
        /// </summary>
        [Required]
        public TModel Model { get; set; }
    }

    public class BaseIndexCreateEvent<TModel> : INotification
        where TModel : BaseIndexReport
    {
        public TModel Model { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }

    public class BaseIndexCreateCommandHandler<TModel> : IRequestHandler<BaseIndexCreateCommand<TModel>, Guid>
        where TModel : BaseIndexReport
    {
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IMediator Mediator { get; set; }
        private ILogger Logger { get; set; }
        public BaseIndexCreateCommandHandler(MasofaIndicesDbContext masofaIndicesDbContext, IBusinessLogicLogger businessLogicLogger, IMediator mediator, ILogger logger)
        {
            MasofaIndicesDbContext = masofaIndicesDbContext;
            BusinessLogicLogger = businessLogicLogger;
            Mediator = mediator;
            Logger = logger;
        }
        public async Task<Guid> Handle(BaseIndexCreateCommand<TModel> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var model = request.Model;

                var result = await MasofaIndicesDbContext
                    .Set<TModel>()
                    .AddAsync(model);

                await MasofaIndicesDbContext.SaveChangesAsync();
                await Mediator.Publish(new BaseIndexCreateEvent<TModel>()
                {
                    Model = result.Entity,
                    DateTime = DateTime.UtcNow,
                }, cancellationToken);
                return result.Entity.Id;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}
