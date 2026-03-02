using MediatR;

namespace Masofa.BusinessLogic.Uav.Commands
{
    public class DeleteUavSurveyCommand : IRequest<bool>
    {
        public Guid SurveyId { get; set; }
        public string UserName { get; set; }
    }
}