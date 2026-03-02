using MediatR;
using Masofa.Common.Models.Uav;

namespace Masofa.BusinessLogic.Uav.Commands
{
    public class UpdateUavSurveyCommand : IRequest<UAVFlyPath>
    {
        public Guid Id { get; set; }
        public string? Comment { get; set; }
        public Guid? DataTypeId { get; set; }
        public Guid? CameraTypeId { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();

        [Newtonsoft.Json.JsonIgnore]
        public string? UserName { get; set; }
    }
}