using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MasofaIndices.Sentinel
{
    [XmlRoot("MD_Metadata")]
    public class MD_Metadata
    {
        [XmlElement("fileIdentifier")]
        public string FileIdentifier { get; set; }

        [XmlElement("language")]
        public LanguageCode Language { get; set; }

        [XmlElement("characterSet")]
        public MD_CharacterSetCode CharacterSet { get; set; }

        [XmlElement("hierarchyLevel")]
        public MD_ScopeCode HierarchyLevel { get; set; }

        [XmlElement("contact")]
        public CI_ResponsibleParty Contact { get; set; }

        [XmlElement("dateStamp")]
        public string DateStamp { get; set; }

        [XmlElement("metadataStandardName")]
        public string MetadataStandardName { get; set; }

        [XmlElement("metadataStandardVersion")]
        public string MetadataStandardVersion { get; set; }

        [XmlElement("referenceSystemInfo")]
        public MD_ReferenceSystem ReferenceSystemInfo { get; set; }

        [XmlElement("identificationInfo")]
        public MD_DataIdentification IdentificationInfo { get; set; }

        [XmlElement("distributionInfo")]
        public MD_Distribution DistributionInfo { get; set; }

        [XmlElement("dataQualityInfo")]
        public DQ_DataQuality DataQualityInfo { get; set; }
    }

    public class LanguageCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_CharacterSetCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlAttribute("codeSpace")]
        public string CodeSpace { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_ScopeCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class CI_ResponsibleParty
    {
        [XmlElement("organisationName")]
        public string OrganisationName { get; set; }

        [XmlElement("contactInfo")]
        public CI_Contact ContactInfo { get; set; }

        [XmlElement("role")]
        public CI_RoleCode Role { get; set; }
    }

    public class CI_Contact
    {
        [XmlElement("address")]
        public CI_Address Address { get; set; }
    }

    public class CI_Address
    {
        [XmlElement("electronicMailAddress")]
        public string ElectronicMailAddress { get; set; }
    }

    public class CI_RoleCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_ReferenceSystem
    {
        [XmlElement("referenceSystemIdentifier")]
        public RS_Identifier ReferenceSystemIdentifier { get; set; }
    }

    public class RS_Identifier
    {
        [XmlElement("code")]
        public string Code { get; set; }

        [XmlElement("codeSpace")]
        public string CodeSpace { get; set; }
    }

    public class MD_DataIdentification
    {
        [XmlElement("citation")]
        public CI_Citation Citation { get; set; }

        [XmlElement("abstract")]
        public string Abstract { get; set; }

        [XmlElement("pointOfContact")]
        public CI_ResponsibleParty PointOfContact { get; set; }

        [XmlElement("descriptiveKeywords")]
        public List<MD_Keywords> DescriptiveKeywords { get; set; }

        //[XmlElement("resourceConstraints", Type = typeof(MD_Constraints))]
        //[XmlElement("resourceConstraints", Type = typeof(MD_LegalConstraints))]
        //public List<object> ResourceConstraints { get; set; }

        [XmlElement("spatialResolution")]
        public MD_Resolution SpatialResolution { get; set; }

        [XmlElement("language")]
        public LanguageCode Language { get; set; }

        [XmlElement("topicCategory")]
        public string TopicCategory { get; set; }

        [XmlElement("extent")]
        public EX_Extent Extent { get; set; }
    }

    public class CI_Citation
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("date")]
        public CI_DateWrapper Date { get; set; }

        [XmlElement("identifier")]
        public List<RS_Identifier> Identifier { get; set; }
    }

    public class CI_DateWrapper
    {
        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("dateType")]
        public CI_DateTypeCode DateType { get; set; }
    }

    public class CI_DateTypeCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_Keywords
    {
        [XmlElement("keyword")]
        public string[] Keyword { get; set; }

        [XmlElement("thesaurusName")]
        public CI_Citation ThesaurusName { get; set; }
    }

    public class MD_Constraints
    {
        [XmlElement("useLimitation")]
        public string UseLimitation { get; set; }
    }

    public class MD_LegalConstraints
    {
        [XmlElement("accessConstraints")]
        public MD_RestrictionCode AccessConstraints { get; set; }

        [XmlElement("otherConstraints")]
        public string OtherConstraints { get; set; }
    }

    public class MD_RestrictionCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_Resolution
    {
        [XmlElement("equivalentScale")]
        public MD_RepresentativeFraction EquivalentScale { get; set; }
    }

    public class MD_RepresentativeFraction
    {
        [XmlElement("denominator")]
        public int Denominator { get; set; }
    }

    public class EX_Extent
    {
        [XmlElement("geographicElement")]
        public EX_GeographicBoundingBox GeographicElement { get; set; }

        [XmlElement("temporalElement")]
        public EX_TemporalExtent TemporalElement { get; set; }
    }

    public class EX_GeographicBoundingBox
    {
        [XmlElement("westBoundLongitude")]
        public decimal WestBoundLongitude { get; set; }

        [XmlElement("eastBoundLongitude")]
        public decimal EastBoundLongitude { get; set; }

        [XmlElement("southBoundLatitude")]
        public decimal SouthBoundLatitude { get; set; }

        [XmlElement("northBoundLatitude")]
        public decimal NorthBoundLatitude { get; set; }
    }

    public class EX_TemporalExtent
    {
        [XmlElement("extent")]
        public TimePeriodWrapper Extent { get; set; }
    }

    public class TimePeriodWrapper
    {
        [XmlElement("TimePeriod")]
        public TimePeriod TimePeriod { get; set; }
    }

    public class TimePeriod
    {
        // gml:id игнорируется — не объявляем атрибут
        [XmlElement("beginPosition")]
        public DateTime BeginPosition { get; set; }

        [XmlElement("endPosition")]
        public DateTime EndPosition { get; set; }
    }

    public class MD_Distribution
    {
        [XmlElement("distributionFormat")]
        public MD_Format DistributionFormat { get; set; }

        [XmlElement("transferOptions")]
        public MD_DigitalTransferOptions TransferOptions { get; set; }
    }

    public class MD_Format
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }
    }

    public class MD_DigitalTransferOptions
    {
        [XmlElement("onLine")]
        public CI_OnlineResource OnLine { get; set; }
    }

    public class CI_OnlineResource
    {
        [XmlElement("linkage")]
        public string Linkage { get; set; }
    }

    public class DQ_DataQuality
    {
        [XmlElement("scope")]
        public DQ_Scope Scope { get; set; }

        [XmlElement("report")]
        public DQ_DomainConsistency Report { get; set; }

        [XmlElement("lineage")]
        public LI_Lineage Lineage { get; set; }
    }

    public class DQ_Scope
    {
        [XmlElement("level")]
        public MD_ScopeCode Level { get; set; }
    }

    public class DQ_DomainConsistency
    {
        [XmlElement("result")]
        public DQ_ConformanceResult Result { get; set; }
    }

    public class DQ_ConformanceResult
    {
        [XmlElement("specification")]
        public CI_Citation Specification { get; set; }

        [XmlElement("explanation")]
        public string Explanation { get; set; }

        [XmlElement("pass")]
        public bool Pass { get; set; }
    }

    public class LI_Lineage
    {
        [XmlElement("statement")]
        public string Statement { get; set; }
    }
}