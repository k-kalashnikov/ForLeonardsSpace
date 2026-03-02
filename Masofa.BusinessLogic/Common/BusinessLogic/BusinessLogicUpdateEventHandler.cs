using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Masofa.BusinessLogic.Common.BusinessLogic
{
    public class BusinessLogicUpdateEventHandler<TModel, TDbContext> : INotificationHandler<BaseUpdateEvent<TModel, TDbContext>>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BusinessLogicUpdateEventHandler(IBusinessLogicLogger businessLogicLogger)
        {
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task Handle(BaseUpdateEvent<TModel, TDbContext> notification, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var messageBody = new UpdateMessageBody
            {
                ModelName = typeof(TModel).ToString(),
                OldModelValue = JsonSerializer.Serialize(notification.OldModel, options),
                NewModelValue = JsonSerializer.Serialize(notification.CurrentModel, options)
            };

            await BusinessLogicLogger.LogInformationAsync(LogMessageResource.BusinessEventSerialized(Newtonsoft.Json.JsonConvert.SerializeObject(messageBody)), requestPath);
        }
    }

    public class UpdateMessageBody
    {
        public string? ModelName { get; set; }
        public string? OldModelValue { get; set; }
        public string? NewModelValue { get; set; }
    }
}
