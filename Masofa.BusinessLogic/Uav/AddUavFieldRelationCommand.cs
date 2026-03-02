using MediatR;

namespace Masofa.BusinessLogic.Uav.Commands
{
    public class AddUavFieldRelationCommand : IRequest<Guid>
    {
        public Guid SurveyId { get; set; }
        public Guid FieldId { get; set; }
        public Guid? CropId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string? UserName { get; set; }
    }
}