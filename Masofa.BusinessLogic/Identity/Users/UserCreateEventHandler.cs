using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common.BusinessLogic;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using MediatR;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Masofa.BusinessLogic.Identity.Users
{
    public class UserCreateEventHandler : INotificationHandler<UserCreateEvent>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public UserCreateEventHandler(IBusinessLogicLogger businessLogicLogger)
        {
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task Handle(UserCreateEvent notification, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var messageBody = new CreateMessageBody
            {
                ModelName = typeof(User).ToString(),
                NewModelValue = Newtonsoft.Json.JsonConvert.SerializeObject(notification.User)
            };

            await BusinessLogicLogger.LogInformationAsync(LogMessageResource.BusinessEventSerialized(Newtonsoft.Json.JsonConvert.SerializeObject(messageBody)), requestPath);
        }
    }
}
