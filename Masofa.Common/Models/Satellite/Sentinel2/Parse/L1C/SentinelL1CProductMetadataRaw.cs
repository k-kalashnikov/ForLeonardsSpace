using System.Text.Json.Serialization;
using Masofa.Common.Models.Satellite.Auxiliary;

namespace Masofa.Common.Models.Satellite.Parse.Sentinel.L1C
{
    /// <summary>
    /// JSON модель для L1C Product Metadata - соответствует структуре Rootobject
    /// </summary>
    public class SentinelL1CProductMetadataRaw
    {
        [JsonPropertyName("n1")]
        public string N1 { get; set; }

        [JsonPropertyName("xsi")]
        public string Xsi { get; set; }

        [JsonPropertyName("schemaLocation")]
        public string SchemaLocation { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("General_Info")]
        public General_Info General_Info { get; set; }

        [JsonPropertyName("Geometric_Info")]
        public Geometric_Info Geometric_Info { get; set; }

        [JsonPropertyName("Auxiliary_Data_Info")]
        public Auxiliary_Data_Info Auxiliary_Data_Info { get; set; }

        [JsonPropertyName("Quality_Indicators_Info")]
        public Quality_Indicators_Info Quality_Indicators_Info { get; set; }

        /// <summary>
        /// Извлекает время начала продукта из метаданных
        /// </summary>
        public string ProductStartTime => General_Info?.Product_Info?.PRODUCT_START_TIME.ToString() ?? string.Empty;
    }

    public class General_Info
    {
        [JsonPropertyName("Product_Info")]
        public Product_Info Product_Info { get; set; }

        [JsonPropertyName("Product_Image_Characteristics")]
        public Product_Image_Characteristics Product_Image_Characteristics { get; set; }
    }

    public class Product_Info
    {
        [JsonPropertyName("PRODUCT_START_TIME")]
        public DateTime PRODUCT_START_TIME { get; set; }

        [JsonPropertyName("PRODUCT_STOP_TIME")]
        public DateTime PRODUCT_STOP_TIME { get; set; }

        [JsonPropertyName("PRODUCT_URI")]
        public string PRODUCT_URI { get; set; }

        [JsonPropertyName("PROCESSING_LEVEL")]
        public string PROCESSING_LEVEL { get; set; }

        [JsonPropertyName("PRODUCT_TYPE")]
        public string PRODUCT_TYPE { get; set; }

        [JsonPropertyName("PROCESSING_BASELINE")]
        public object PROCESSING_BASELINE { get; set; }

        [JsonPropertyName("PRODUCT_DOI")]
        public string PRODUCT_DOI { get; set; }

        [JsonPropertyName("GENERATION_TIME")]
        public DateTime GENERATION_TIME { get; set; }

        [JsonPropertyName("PREVIEW_IMAGE_URL")]
        public string PREVIEW_IMAGE_URL { get; set; }

        [JsonPropertyName("PREVIEW_GEO_INFO")]
        public string PREVIEW_GEO_INFO { get; set; }

        [JsonPropertyName("Datatake")]
        public Datatake Datatake { get; set; }

        [JsonPropertyName("Query_Options")]
        public Query_Options Query_Options { get; set; }

        [JsonPropertyName("Product_Organisation")]
        public Product_Organisation Product_Organisation { get; set; }
    }

    public class Datatake
    {
        [JsonPropertyName("datatakeIdentifier")]
        public string datatakeIdentifier { get; set; }

        [JsonPropertyName("value")]
        public string value { get; set; }

        [JsonPropertyName("SPACECRAFT_NAME")]
        public string SPACECRAFT_NAME { get; set; }

        [JsonPropertyName("DATATAKE_TYPE")]
        public string DATATAKE_TYPE { get; set; }

        [JsonPropertyName("DATATAKE_SENSING_START")]
        public DateTime DATATAKE_SENSING_START { get; set; }

        [JsonPropertyName("SENSING_ORBIT_NUMBER")]
        public int SENSING_ORBIT_NUMBER { get; set; }

        [JsonPropertyName("SENSING_ORBIT_DIRECTION")]
        public string SENSING_ORBIT_DIRECTION { get; set; }

        public override string ToString()
        {
            return string.Join("|",
            [
                datatakeIdentifier ?? string.Empty,
                value ?? string.Empty,
                SPACECRAFT_NAME ?? string.Empty,
                DATATAKE_TYPE ?? string.Empty,
                DATATAKE_SENSING_START.ToString("o") ?? string.Empty,
                SENSING_ORBIT_NUMBER.ToString() ?? string.Empty,
                SENSING_ORBIT_DIRECTION ?? string.Empty
            ]);
        }
    }

    public class Query_Options
    {
        [JsonPropertyName("completeSingleTile")]
        public string completeSingleTile { get; set; }

        [JsonPropertyName("value")]
        public string value { get; set; }

        [JsonPropertyName("PRODUCT_FORMAT")]
        public string PRODUCT_FORMAT { get; set; }
    }

    public class Product_Organisation
    {
        [JsonPropertyName("Granule_List")]
        public Granule_List Granule_List { get; set; }
    }

    public class Granule_List
    {
        [JsonPropertyName("Granule")]
        public Granule Granule { get; set; }
    }

    public class Granule
    {
        [JsonPropertyName("datastripIdentifier")]
        public string datastripIdentifier { get; set; }

        [JsonPropertyName("granuleIdentifier")]
        public string granuleIdentifier { get; set; }

        [JsonPropertyName("imageFormat")]
        public string imageFormat { get; set; }

        [JsonPropertyName("value")]
        public string value { get; set; }

        [JsonPropertyName("IMAGE_FILE")]
        public string[] IMAGE_FILE { get; set; }

        public override string ToString()
        {
            return string.Join("|",
            [
                datastripIdentifier ?? string.Empty,
                granuleIdentifier ?? string.Empty,
                imageFormat ?? string.Empty,
                value ?? string.Empty,
                IMAGE_FILE != null ? string.Join("^", IMAGE_FILE) : string.Empty
            ]);
        }
    }

    public class Product_Image_Characteristics
    {
        [JsonPropertyName("Special_Values")]
        public Special_Values[] Special_Values { get; set; }

        [JsonPropertyName("Image_Display_Order")]
        public Image_Display_Order Image_Display_Order { get; set; }

        [JsonPropertyName("QUANTIFICATION_VALUE")]
        public int QUANTIFICATION_VALUE { get; set; }

        [JsonPropertyName("Radiometric_Offset_List")]
        public Radiometric_Offset_List Radiometric_Offset_List { get; set; }

        [JsonPropertyName("Reflectance_Conversion")]
        public Reflectance_Conversion Reflectance_Conversion { get; set; }

        [JsonPropertyName("Spectral_Information_List")]
        public Spectral_Information_List Spectral_Information_List { get; set; }

        [JsonPropertyName("PHYSICAL_GAINS")]
        public string[] PHYSICAL_GAINS { get; set; }

        [JsonPropertyName("REFERENCE_BAND")]
        public int REFERENCE_BAND { get; set; }
    }

    public class Image_Display_Order
    {
        [JsonPropertyName("RED_CHANNEL")]
        public int RED_CHANNEL { get; set; }

        [JsonPropertyName("GREEN_CHANNEL")]
        public int GREEN_CHANNEL { get; set; }

        [JsonPropertyName("BLUE_CHANNEL")]
        public int BLUE_CHANNEL { get; set; }
    }

    public class Radiometric_Offset_List
    {
        [JsonPropertyName("RADIO_ADD_OFFSET")]
        public int[] RADIO_ADD_OFFSET { get; set; }
    }

    public class Reflectance_Conversion
    {
        [JsonPropertyName("U")]
        public object U { get; set; }

        [JsonPropertyName("Solar_Irradiance_List")]
        public Solar_Irradiance_List Solar_Irradiance_List { get; set; }
    }

    public class Solar_Irradiance_List
    {
        [JsonPropertyName("SOLAR_IRRADIANCE")]
        public object[] SOLAR_IRRADIANCE { get; set; }
    }

    public class Spectral_Information_List
    {
        [JsonPropertyName("Spectral_Information")]
        public Spectral_Information[] Spectral_Information { get; set; }
    }

    public class Spectral_Information
    {
        [JsonPropertyName("bandId")]
        public object bandId { get; set; }

        [JsonPropertyName("physicalBand")]
        public object physicalBand { get; set; }

        [JsonPropertyName("value")]
        public object value { get; set; }

        [JsonPropertyName("RESOLUTION")]
        public int RESOLUTION { get; set; }

        [JsonPropertyName("Wavelength")]
        public Wavelength Wavelength { get; set; }

        [JsonPropertyName("Spectral_Response")]
        public Spectral_Response Spectral_Response { get; set; }
    }

    public class Wavelength
    {
        [JsonPropertyName("MIN")]
        public int MIN { get; set; }

        [JsonPropertyName("MAX")]
        public int MAX { get; set; }

        [JsonPropertyName("CENTRAL")]
        public object CENTRAL { get; set; }
    }

    public class Spectral_Response
    {
        [JsonPropertyName("STEP")]
        public int STEP { get; set; }

        [JsonPropertyName("VALUES")]
        public object VALUES { get; set; }
    }

    public class Special_Values
    {
        [JsonPropertyName("SPECIAL_VALUE_TEXT")]
        public object SPECIAL_VALUE_TEXT { get; set; }

        [JsonPropertyName("SPECIAL_VALUE_INDEX")]
        public int SPECIAL_VALUE_INDEX { get; set; }

        public override string ToString()
        {
            return string.Join("|",
            [
                SPECIAL_VALUE_TEXT ?? string.Empty,
                SPECIAL_VALUE_INDEX.ToString()
            ]);
        }
    }

    public class Geometric_Info
    {
        [JsonPropertyName("Product_Footprint")]
        public Product_Footprint Product_Footprint { get; set; }

        [JsonPropertyName("Coordinate_Reference_System")]
        public Coordinate_Reference_System Coordinate_Reference_System { get; set; }
    }

    public class Product_Footprint
    {
        [JsonPropertyName("Product_Footprint")]
        public Product_Footprint1 Product_Footprint1 { get; set; }

        [JsonPropertyName("RASTER_CS_TYPE")]
        public string RASTER_CS_TYPE { get; set; }

        [JsonPropertyName("PIXEL_ORIGIN")]
        public int PIXEL_ORIGIN { get; set; }
    }

    public class Product_Footprint1
    {
        [JsonPropertyName("Global_Footprint")]
        public Global_Footprint Global_Footprint { get; set; }
    }

    public class Global_Footprint
    {
        [JsonPropertyName("EXT_POS_LIST")]
        public string EXT_POS_LIST { get; set; }
    }

    public class Coordinate_Reference_System
    {
        [JsonPropertyName("GEO_TABLES")]
        public string GEO_TABLES { get; set; }

        [JsonPropertyName("HORIZONTAL_CS_TYPE")]
        public string HORIZONTAL_CS_TYPE { get; set; }
    }

    public class Auxiliary_Data_Info
    {
        [JsonPropertyName("GIPP_List")]
        public GIPP_List GIPP_List { get; set; }

        [JsonPropertyName("PRODUCTION_DEM_TYPE")]
        public string PRODUCTION_DEM_TYPE { get; set; }

        [JsonPropertyName("IERS_BULLETIN_FILENAME")]
        public string IERS_BULLETIN_FILENAME { get; set; }

        [JsonPropertyName("GRI_List")]
        public GRI_List GRI_List { get; set; }

        [JsonPropertyName("ECMWF_DATA_REF")]
        public string ECMWF_DATA_REF { get; set; }
    }

    public class GIPP_List
    {
        [JsonPropertyName("GIPP_FILENAME")]
        public string[] GIPP_FILENAME { get; set; }
    }

    public class GRI_List
    {
        [JsonPropertyName("GRI_FILENAME")]
        public string[] GRI_FILENAME { get; set; }
    }

    public class Quality_Indicators_Info
    {
        [JsonPropertyName("Cloud_Coverage_Assessment")]
        public string Cloud_Coverage_Assessment { get; set; }

        [JsonPropertyName("Snow_Coverage_Assessment")]
        public string Snow_Coverage_Assessment { get; set; }

        [JsonPropertyName("Technical_Quality_Assessment")]
        public Technical_Quality_Assessment Technical_Quality_Assessment { get; set; }

        [JsonPropertyName("Quality_Control_Checks")]
        public Quality_Control_Checks Quality_Control_Checks { get; set; }
    }

    public class Technical_Quality_Assessment
    {
        [JsonPropertyName("DEGRADED_ANC_DATA_PERCENTAGE")]
        public string DEGRADED_ANC_DATA_PERCENTAGE { get; set; }

        [JsonPropertyName("DEGRADED_MSI_DATA_PERCENTAGE")]
        public int DEGRADED_MSI_DATA_PERCENTAGE { get; set; }
    }

    public class Quality_Control_Checks
    {
        [JsonPropertyName("Quality_Inspections")]
        public Quality_Inspections Quality_Inspections { get; set; }

        [JsonPropertyName("Failed_Inspections")]
        public string Failed_Inspections { get; set; }
    }

    public class Quality_Inspections
    {
        [JsonPropertyName("quality_check")]
        public string[] quality_check { get; set; }
    }
} 