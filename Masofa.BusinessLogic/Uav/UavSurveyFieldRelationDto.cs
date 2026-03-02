namespace Masofa.BusinessLogic.Uav
{
    public class UavSurveyFieldRelationDto
    {
        public Guid Id { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? CropId { get; set; }
        public Guid? SeasonId { get; set; }
        public Guid? FirmId { get; set; }
        public string FieldPolygonWkt { get; set; }
    }
}