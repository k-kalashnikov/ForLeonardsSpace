using MediatR;
using Newtonsoft.Json;

namespace Masofa.BusinessLogic.Uav.Commands
{
    public class DeleteUavFieldRelationCommand : IRequest<bool>
    {
        public Guid SurveyId { get; set; }
        public Guid FieldId { get; set; }

        [JsonIgnore]
        public string? UserName { get; set; }
    }
}