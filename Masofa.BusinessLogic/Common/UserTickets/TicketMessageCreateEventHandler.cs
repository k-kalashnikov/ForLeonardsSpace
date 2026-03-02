using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common.BusinessLogic;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Notifications;
using Masofa.DataAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Common.UserTickets
{
    public abstract class TicketMessageCreateEventHandler : INotificationHandler<MessageCreateEvent>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public TicketMessageCreateEventHandler(IBusinessLogicLogger businessLogicLogger)
        {
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task Handle(MessageCreateEvent notification, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var messageBody = new CreateMessageBody
            {
                ModelName = typeof(UserTicketMessage).ToString(),
                NewModelValue = Newtonsoft.Json.JsonConvert.SerializeObject(notification.Model)
            };

            await BusinessLogicLogger.LogInformationAsync(LogMessageResource.BusinessEventSerialized(Newtonsoft.Json.JsonConvert.SerializeObject(messageBody)), requestPath);
        }
    }
}
