using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Masofa.BusinessLogic.Common.BusinessLogic
{
    public class BusinessLogicDeleteEventHandler<TModel, TDbContext> : INotificationHandler<BaseDeleteEvent<TModel, TDbContext>>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BusinessLogicDeleteEventHandler(IBusinessLogicLogger businessLogicLogger)
        {
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task Handle(BaseDeleteEvent<TModel, TDbContext> notification, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var messageBody = new UpdateMessageBody
            {
                ModelName = typeof(TModel).ToString(),
                OldModelValue = Newtonsoft.Json.JsonConvert.SerializeObject(notification.Model),
            };

            await BusinessLogicLogger.LogInformationAsync(LogMessageResource.BusinessEventSerialized(Newtonsoft.Json.JsonConvert.SerializeObject(messageBody)), requestPath);
        }
    }

    public class DeleteMessageBody
    {
        public string? ModelName { get; set; }
        public string? OldModelValue { get; set; }
    }
}
