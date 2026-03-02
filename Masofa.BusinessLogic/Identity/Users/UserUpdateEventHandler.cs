using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common.BusinessLogic;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using MediatR;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Masofa.BusinessLogic.Identity.Users
{
    public class UserUpdateEventHandler : INotificationHandler<UserUpdateEvent>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public UserUpdateEventHandler(IBusinessLogicLogger businessLogicLogger)
        {
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task Handle(UserUpdateEvent notification, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var messageBody = new UpdateMessageBody
            {
                ModelName = typeof(User).ToString(),
                OldModelValue = Newtonsoft.Json.JsonConvert.SerializeObject(notification.OldModel),
                NewModelValue = Newtonsoft.Json.JsonConvert.SerializeObject(notification.CurrentModel)
            };

            await BusinessLogicLogger.LogInformationAsync(LogMessageResource.BusinessEventSerialized(Newtonsoft.Json.JsonConvert.SerializeObject(messageBody)), requestPath);
        }
    }
}
