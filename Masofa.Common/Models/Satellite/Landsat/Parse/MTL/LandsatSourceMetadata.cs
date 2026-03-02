namespace Masofa.Common.Models.Satellite.Parse.Landsat.MTL
{
    public class LandsatSourceMetadata
    {
        public string SatellateProductId { get; set; } = default!;
        public Guid? ProductContentsId { get; set; }
        public Guid? ImageAttributesId { get; set; }
        public Guid? ProjectionAttributesId { get; set; }
        public Guid? Level2ProcessingRecordId { get; set; }
        public Guid? Level2SurfaceReflectanceParametersId { get; set; }
        public Guid? Level2SurfaceTemperatureParametersId { get; set; }
        public Guid? Level1ProcessingRecordId { get; set; }
        public Guid? Level1MinMaxRadianceId { get; set; }
        public Guid? Level1MinMaxReflectanceId { get; set; }
        public Guid? Level1MinMaxPixelValueId { get; set; }
        public Guid? Level1RadiometricRescalingId { get; set; }
        public Guid? Level1ThermalConstantsId { get; set; }
        public Guid? Level1ProjectionParametersId { get; set; }
    }
}