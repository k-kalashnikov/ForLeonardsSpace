using MediatR;

namespace Masofa.BusinessLogic.Features.RecommendedPlantingDates
{
    public class CreateRecommendedPlantingDateCommand : IRequest<Guid>
    {
        public Masofa.Common.Models.Dictionaries.RecommendedPlantingDates Model { get; set; }
    }

    public class EditRecommendedPlantingDateCommand : IRequest<Guid>
    {
        public Masofa.Common.Models.Dictionaries.RecommendedPlantingDates Model { get; set; }
    }

    public class GetRecommendedPlantingDatesCustomQuery : IRequest<List<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>>
    {
        public Guid CropId { get; set; }

        public GetRecommendedPlantingDatesCustomQuery(Guid cropId)
        {
            CropId = cropId;
        }
    }
}