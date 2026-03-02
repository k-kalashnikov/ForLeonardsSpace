using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite.Auxiliary;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Sentinel
{
    [PartitionedTable]
    public class SentinelL1CProductMetadata : BaseEntity
    {
        // General Info
        public DateTime ProductStartTime { get; set; }
        public DateTime ProductStopTime { get; set; }
        public string? ProductUri { get; set; }
        public string? ProcessingLevel { get; set; }
        public string? ProductType { get; set; }
        public string? ProcessingBaseline { get; set; }
        public string? ProductDoi { get; set; }
        public DateTime GenerationTime { get; set; }
        [NotMapped]
        public Datatake Datatake { get; set; }
        public string DatatakeRaw
        {
            get => Datatake?.ToString();
            set => Datatake = string.IsNullOrEmpty(value) ? null : Datatake.FromString(value);
        }

        [NotMapped]
        public List<string> Granules { get; set; }
        public string GranulesRaw
        {
            get => Granules == null ? null : string.Join("~", Granules.Select(x => x.ToString()));
            set => Granules = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(x => x).Where(x => x != null).ToList();
        }

        [NotMapped]
        public List<SpecialValue> SpecialValues { get; set; }
        public string SpecialValuesRaw
        {
            get => SpecialValues == null ? null : string.Join("~", SpecialValues.Select(x => x.ToString()));
            set => SpecialValues = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(SpecialValue.FromString).Where(x => x != null).ToList();
        }

        // Image Display Order
        public int RedChannel { get; set; }
        public int GreenChannel { get; set; }
        public int BlueChannel { get; set; }

        public int QuantificationValue { get; set; }

        // Radiometric Offsets
        [NotMapped]
        public List<string> Offsets { get; set; }
        public string OffsetsRaw
        {
            get => Offsets == null ? null : string.Join("~", Offsets.Select(x => x.ToString()));
            set => Offsets = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(x => x).Where(x => x != null).ToList();
        }

        // Reflectance Conversion
        public double ReflectanceU { get; set; }
        [NotMapped]
        public List<string> SolarIrradianceList { get; set; }
        public string SolarIrradianceListRaw
        {
            get => SolarIrradianceList == null ? null : string.Join("~", SolarIrradianceList.Select(x => x.ToString()));
            set => SolarIrradianceList = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(x => x).Where(x => x != null).ToList();
        }

        // Spectral Info
        public string? SpectralBandIds { get; set; }
        public string? SpectralPhysicalBands { get; set; }
        public string? SpectralResolutions { get; set; }
        public string? SpectralWavelengthMins { get; set; }
        public string? SpectralWavelengthMaxs { get; set; }
        public string? SpectralWavelengthCentrals { get; set; }
        [NotMapped]
        public List<SpectralInformation> SpectralInformationList { get; set; }
        public string SpectralInformationListRaw
        {
            get => SpectralInformationList == null ? null : string.Join("~", SpectralInformationList.Select(x => x.ToString()));
            set => SpectralInformationList = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(SpectralInformation.FromString).Where(x => x != null).ToList();
        }
        public int ReferenceBand { get; set; }

        // Product Footprint
        public string? ExtPosList { get; set; }
        public string? RasterCsType { get; set; }
        public int PixelOrigin { get; set; }

        // Coordinate Reference System
        public string? GeoTables { get; set; }
        public string? HorizontalCsType { get; set; }

        // Auxiliary data
        [NotMapped]
        public List<GippFile> GippFiles { get; set; }
        public string GippFilesRaw
        {
            get => GippFiles == null ? null : string.Join("~", GippFiles.Select(x => x.ToString()));
            set => GippFiles = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(GippFile.FromString).Where(x => x != null).ToList();
        }
    }

    public class SentinelL1CProductMetadataHistory : BaseHistoryEntity<SentinelL1CProductMetadata> { }
}