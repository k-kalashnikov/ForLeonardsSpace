using MediatR;

namespace Masofa.BusinessLogic.Uav.Queries
{
    public class GetUavFieldRelationsQuery : IRequest<List<UavSurveyFieldRelationDto>>
    {
        public Guid SurveyId { get; set; }
    }
}