using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Masofa.BusinessLogic.Common.BusinessLogic
{
    public class BusinessLogicCreateEventHandler<TModel, TDbContext> : INotificationHandler<BaseCreateEvent<TModel, TDbContext>>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BusinessLogicCreateEventHandler(IBusinessLogicLogger businessLogicLogger)
        {
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task Handle(BaseCreateEvent<TModel, TDbContext> notification, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var messageBody = new CreateMessageBody
            {
                ModelName = typeof(TModel).ToString(),
                NewModelValue = Newtonsoft.Json.JsonConvert.SerializeObject(notification.Model)
            };

            await BusinessLogicLogger.LogInformationAsync(LogMessageResource.BusinessEventSerialized(Newtonsoft.Json.JsonConvert.SerializeObject(messageBody)), requestPath);
        }
    }

    public class CreateMessageBody
    {
        public string? ModelName { get; set; }
        public string? NewModelValue { get; set; }
    }
}
