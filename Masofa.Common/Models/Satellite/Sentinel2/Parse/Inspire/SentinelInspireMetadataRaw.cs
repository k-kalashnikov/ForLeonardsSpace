using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Sentinel.Inspire
{
    /// <summary>
    /// Модель для десериализации INSPIRE Metadata JSON файла Sentinel2
    /// </summary>
    public class SentinelInspireMetadataRaw
    {
        [JsonPropertyName("gco")]
        public string Gco { get; set; }

        [JsonPropertyName("gmd")]
        public string Gmd { get; set; }

        [JsonPropertyName("gml")]
        public string Gml { get; set; }

        [JsonPropertyName("xlink")]
        public string Xlink { get; set; }

        [JsonPropertyName("xsi")]
        public string Xsi { get; set; }

        [JsonPropertyName("schemaLocation")]
        public string SchemaLocation { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("fileIdentifier")]
        public Fileidentifier FileIdentifier { get; set; }

        [JsonPropertyName("language")]
        public Language Language { get; set; }

        [JsonPropertyName("characterSet")]
        public Characterset CharacterSet { get; set; }

        [JsonPropertyName("hierarchyLevel")]
        public Hierarchylevel HierarchyLevel { get; set; }

        [JsonPropertyName("contact")]
        public Contact Contact { get; set; }

        [JsonPropertyName("dateStamp")]
        public Datestamp DateStamp { get; set; }

        [JsonPropertyName("metadataStandardName")]
        public Metadatastandardname MetadataStandardName { get; set; }

        [JsonPropertyName("metadataStandardVersion")]
        public Metadatastandardversion MetadataStandardVersion { get; set; }

        [JsonPropertyName("referenceSystemInfo")]
        public Referencesysteminfo ReferenceSystemInfo { get; set; }

        [JsonPropertyName("identificationInfo")]
        public Identificationinfo IdentificationInfo { get; set; }

        [JsonPropertyName("distributionInfo")]
        public Distributioninfo DistributionInfo { get; set; }

        [JsonPropertyName("dataQualityInfo")]
        public Dataqualityinfo DataQualityInfo { get; set; }
    }

    public class Fileidentifier
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Language
    {
        [JsonPropertyName("LanguageCode")]
        public string LanguageCode { get; set; }
    }

    public class Characterset
    {
        [JsonPropertyName("MD_CharacterSetCode")]
        public string MDCharacterSetCode { get; set; }
    }

    public class Hierarchylevel
    {
        [JsonPropertyName("MD_ScopeCode")]
        public string MDScopeCode { get; set; }
    }

    public class Contact
    {
        [JsonPropertyName("CI_ResponsibleParty")]
        public CI_Responsibleparty CIResponsibleParty { get; set; }
    }

    public class CI_Responsibleparty
    {
        [JsonPropertyName("organisationName")]
        public Organisationname OrganisationName { get; set; }

        [JsonPropertyName("contactInfo")]
        public Contactinfo ContactInfo { get; set; }

        [JsonPropertyName("role")]
        public Role Role { get; set; }
    }

    public class Organisationname
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Contactinfo
    {
        [JsonPropertyName("CI_Contact")]
        public CI_Contact CIContact { get; set; }
    }

    public class CI_Contact
    {
        [JsonPropertyName("address")]
        public Address Address { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("CI_Address")]
        public CI_Address CIAddress { get; set; }
    }

    public class CI_Address
    {
        [JsonPropertyName("electronicMailAddress")]
        public Electronicmailaddress ElectronicMailAddress { get; set; }
    }

    public class Electronicmailaddress
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Role
    {
        [JsonPropertyName("CI_RoleCode")]
        public string CIRoleCode { get; set; }
    }

    public class Datestamp
    {
        [JsonPropertyName("Date")]
        public string Date { get; set; }
    }

    public class Metadatastandardname
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Metadatastandardversion
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Referencesysteminfo
    {
        [JsonPropertyName("MD_ReferenceSystem")]
        public MD_Referencesystem MDReferenceSystem { get; set; }
    }

    public class MD_Referencesystem
    {
        [JsonPropertyName("referenceSystemIdentifier")]
        public Referencesystemidentifier ReferenceSystemIdentifier { get; set; }
    }

    public class Referencesystemidentifier
    {
        [JsonPropertyName("RS_Identifier")]
        public RS_Identifier RSIdentifier { get; set; }
    }

    public class RS_Identifier
    {
        [JsonPropertyName("code")]
        public Code Code { get; set; }

        [JsonPropertyName("codeSpace")]
        public Codespace CodeSpace { get; set; }
    }

    public class Code
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Codespace
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Identificationinfo
    {
        [JsonPropertyName("MD_DataIdentification")]
        public MD_Dataidentification MDDataIdentification { get; set; }
    }

    public class MD_Dataidentification
    {
        [JsonPropertyName("citation")]
        public Citation Citation { get; set; }

        [JsonPropertyName("abstract")]
        public Abstract Abstract { get; set; }

        [JsonPropertyName("pointOfContact")]
        public Pointofcontact PointOfContact { get; set; }

        [JsonPropertyName("descriptiveKeywords")]
        public Descriptivekeyword[] DescriptiveKeywords { get; set; }

        [JsonPropertyName("resourceConstraints")]
        public Resourceconstraint[] ResourceConstraints { get; set; }

        [JsonPropertyName("spatialResolution")]
        public Spatialresolution SpatialResolution { get; set; }

        [JsonPropertyName("language")]
        public Language1 Language { get; set; }

        [JsonPropertyName("topicCategory")]
        public Topiccategory TopicCategory { get; set; }

        [JsonPropertyName("extent")]
        public Extent Extent { get; set; }
    }

    public class Citation
    {
        [JsonPropertyName("CI_Citation")]
        public CI_Citation CICitation { get; set; }
    }

    public class CI_Citation
    {
        [JsonPropertyName("title")]
        public Title Title { get; set; }

        [JsonPropertyName("date")]
        public Date Date { get; set; }

        [JsonPropertyName("identifier")]
        public Identifier[] Identifier { get; set; }
    }

    public class Title
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Date
    {
        [JsonPropertyName("CI_Date")]
        public CI_Date CIDate { get; set; }
    }

    public class CI_Date
    {
        [JsonPropertyName("date")]
        public Date1 Date { get; set; }

        [JsonPropertyName("dateType")]
        public Datetype DateType { get; set; }
    }

    public class Date1
    {
        [JsonPropertyName("Date")]
        public string DateValue { get; set; }
    }

    public class Datetype
    {
        [JsonPropertyName("CI_DateTypeCode")]
        public string CIDateTypeCode { get; set; }
    }

    public class Identifier
    {
        [JsonPropertyName("RS_Identifier")]
        public RS_Identifier1 RSIdentifier { get; set; }
    }

    public class RS_Identifier1
    {
        [JsonPropertyName("code")]
        public Code1 Code { get; set; }

        [JsonPropertyName("codeSpace")]
        public Codespace1 CodeSpace { get; set; }
    }

    public class Code1
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Codespace1
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Abstract
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Pointofcontact
    {
        [JsonPropertyName("CI_ResponsibleParty")]
        public CI_Responsibleparty1 CIResponsibleParty { get; set; }
    }

    public class CI_Responsibleparty1
    {
        [JsonPropertyName("organisationName")]
        public Organisationname1 OrganisationName { get; set; }

        [JsonPropertyName("contactInfo")]
        public Contactinfo1 ContactInfo { get; set; }

        [JsonPropertyName("role")]
        public Role1 Role { get; set; }
    }

    public class Organisationname1
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Contactinfo1
    {
        [JsonPropertyName("CI_Contact")]
        public CI_Contact1 CIContact { get; set; }
    }

    public class CI_Contact1
    {
        [JsonPropertyName("address")]
        public Address1 Address { get; set; }
    }

    public class Address1
    {
        [JsonPropertyName("CI_Address")]
        public CI_Address1 CIAddress { get; set; }
    }

    public class CI_Address1
    {
        [JsonPropertyName("electronicMailAddress")]
        public Electronicmailaddress1 ElectronicMailAddress { get; set; }
    }

    public class Electronicmailaddress1
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Role1
    {
        [JsonPropertyName("CI_RoleCode")]
        public string CIRoleCode { get; set; }
    }

    public class Spatialresolution
    {
        [JsonPropertyName("MD_Resolution")]
        public MD_Resolution MDResolution { get; set; }
    }

    public class MD_Resolution
    {
        [JsonPropertyName("equivalentScale")]
        public Equivalentscale EquivalentScale { get; set; }
    }

    public class Equivalentscale
    {
        [JsonPropertyName("MD_RepresentativeFraction")]
        public MD_Representativefraction MDRepresentativeFraction { get; set; }
    }

    public class MD_Representativefraction
    {
        [JsonPropertyName("denominator")]
        public Denominator Denominator { get; set; }
    }

    public class Denominator
    {
        [JsonPropertyName("Integer")]
        public int Integer { get; set; }
    }

    public class Language1
    {
        [JsonPropertyName("LanguageCode")]
        public string LanguageCode { get; set; }
    }

    public class Topiccategory
    {
        [JsonPropertyName("MD_TopicCategoryCode")]
        public string MDTopicCategoryCode { get; set; }
    }

    public class Extent
    {
        [JsonPropertyName("EX_Extent")]
        public EX_Extent EXExtent { get; set; }
    }

    public class EX_Extent
    {
        [JsonPropertyName("geographicElement")]
        public Geographicelement GeographicElement { get; set; }

        [JsonPropertyName("temporalElement")]
        public Temporalelement TemporalElement { get; set; }
    }

    public class Geographicelement
    {
        [JsonPropertyName("EX_GeographicBoundingBox")]
        public EX_Geographicboundingbox EXGeographicBoundingBox { get; set; }
    }

    public class EX_Geographicboundingbox
    {
        [JsonPropertyName("westBoundLongitude")]
        public Westboundlongitude WestBoundLongitude { get; set; }

        [JsonPropertyName("eastBoundLongitude")]
        public Eastboundlongitude EastBoundLongitude { get; set; }

        [JsonPropertyName("southBoundLatitude")]
        public Southboundlatitude SouthBoundLatitude { get; set; }

        [JsonPropertyName("northBoundLatitude")]
        public Northboundlatitude NorthBoundLatitude { get; set; }
    }

    public class Westboundlongitude
    {
        [JsonPropertyName("Decimal")]
        public string Decimal { get; set; }
    }

    public class Eastboundlongitude
    {
        [JsonPropertyName("Decimal")]
        public string Decimal { get; set; }
    }

    public class Southboundlatitude
    {
        [JsonPropertyName("Decimal")]
        public string Decimal { get; set; }
    }

    public class Northboundlatitude
    {
        [JsonPropertyName("Decimal")]
        public string Decimal { get; set; }
    }

    public class Temporalelement
    {
        [JsonPropertyName("EX_TemporalExtent")]
        public EX_Temporalextent EXTemporalExtent { get; set; }
    }

    public class EX_Temporalextent
    {
        [JsonPropertyName("extent")]
        public Extent1 Extent { get; set; }
    }

    public class Extent1
    {
        [JsonPropertyName("TimePeriod")]
        public Timeperiod TimePeriod { get; set; }
    }

    public class Timeperiod
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("beginPosition")]
        public DateTime BeginPosition { get; set; }

        [JsonPropertyName("endPosition")]
        public DateTime EndPosition { get; set; }
    }

    public class Descriptivekeyword
    {
        [JsonPropertyName("MD_Keywords")]
        public MD_Keywords MDKeywords { get; set; }
    }

    public class MD_Keywords
    {
        [JsonPropertyName("keyword")]
        public object Keyword { get; set; }

        [JsonPropertyName("thesaurusName")]
        public Thesaurusname ThesaurusName { get; set; }
    }

    public class Thesaurusname
    {
        [JsonPropertyName("CI_Citation")]
        public CI_Citation1 CICitation { get; set; }
    }

    public class CI_Citation1
    {
        [JsonPropertyName("title")]
        public Title1 Title { get; set; }

        [JsonPropertyName("date")]
        public Date2 Date { get; set; }
    }

    public class Title1
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Date2
    {
        [JsonPropertyName("CI_Date")]
        public CI_Date1 CIDate { get; set; }
    }

    public class CI_Date1
    {
        [JsonPropertyName("date")]
        public Date3 Date { get; set; }

        [JsonPropertyName("dateType")]
        public Datetype1 DateType { get; set; }
    }

    public class Date3
    {
        [JsonPropertyName("Date")]
        public string DateValue { get; set; }
    }

    public class Datetype1
    {
        [JsonPropertyName("CI_DateTypeCode")]
        public string CIDateTypeCode { get; set; }
    }

    public class Resourceconstraint
    {
        [JsonPropertyName("MD_Constraints")]
        public MD_Constraints MDConstraints { get; set; }

        [JsonPropertyName("MD_LegalConstraints")]
        public MD_Legalconstraints MDLegalConstraints { get; set; }
    }

    public class MD_Constraints
    {
        [JsonPropertyName("useLimitation")]
        public Uselimitation UseLimitation { get; set; }
    }

    public class Uselimitation
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class MD_Legalconstraints
    {
        [JsonPropertyName("accessConstraints")]
        public Accessconstraints AccessConstraints { get; set; }

        [JsonPropertyName("otherConstraints")]
        public Otherconstraints OtherConstraints { get; set; }
    }

    public class Accessconstraints
    {
        [JsonPropertyName("MD_RestrictionCode")]
        public string MDRestrictionCode { get; set; }
    }

    public class Otherconstraints
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Distributioninfo
    {
        [JsonPropertyName("MD_Distribution")]
        public MD_Distribution MDDistribution { get; set; }
    }

    public class MD_Distribution
    {
        [JsonPropertyName("distributionFormat")]
        public Distributionformat DistributionFormat { get; set; }

        [JsonPropertyName("transferOptions")]
        public Transferoptions TransferOptions { get; set; }
    }

    public class Distributionformat
    {
        [JsonPropertyName("MD_Format")]
        public MD_Format MDFormat { get; set; }
    }

    public class MD_Format
    {
        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("version")]
        public Version Version { get; set; }
    }

    public class Name
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Version
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Transferoptions
    {
        [JsonPropertyName("MD_DigitalTransferOptions")]
        public MD_Digitaltransferoptions MDDigitalTransferOptions { get; set; }
    }

    public class MD_Digitaltransferoptions
    {
        [JsonPropertyName("onLine")]
        public Online OnLine { get; set; }
    }

    public class Online
    {
        [JsonPropertyName("CI_OnlineResource")]
        public CI_Onlineresource CIOnlineResource { get; set; }
    }

    public class CI_Onlineresource
    {
        [JsonPropertyName("linkage")]
        public Linkage Linkage { get; set; }
    }

    public class Linkage
    {
        [JsonPropertyName("URL")]
        public string URL { get; set; }
    }

    public class Dataqualityinfo
    {
        [JsonPropertyName("DQ_DataQuality")]
        public DQ_Dataquality DQDataQuality { get; set; }
    }

    public class DQ_Dataquality
    {
        [JsonPropertyName("scope")]
        public Scope Scope { get; set; }

        [JsonPropertyName("report")]
        public Report Report { get; set; }

        [JsonPropertyName("lineage")]
        public Lineage Lineage { get; set; }
    }

    public class Scope
    {
        [JsonPropertyName("DQ_Scope")]
        public DQ_Scope DQScope { get; set; }
    }

    public class DQ_Scope
    {
        [JsonPropertyName("level")]
        public Level Level { get; set; }
    }

    public class Level
    {
        [JsonPropertyName("MD_ScopeCode")]
        public string MDScopeCode { get; set; }
    }

    public class Report
    {
        [JsonPropertyName("DQ_DomainConsistency")]
        public DQ_Domainconsistency DQDomainConsistency { get; set; }
    }

    public class DQ_Domainconsistency
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("result")]
        public Result Result { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("DQ_ConformanceResult")]
        public DQ_Conformanceresult DQConformanceResult { get; set; }
    }

    public class DQ_Conformanceresult
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("specification")]
        public Specification Specification { get; set; }

        [JsonPropertyName("explanation")]
        public Explanation Explanation { get; set; }

        [JsonPropertyName("pass")]
        public Pass Pass { get; set; }
    }

    public class Specification
    {
        [JsonPropertyName("CI_Citation")]
        public CI_Citation2 CICitation { get; set; }
    }

    public class CI_Citation2
    {
        [JsonPropertyName("title")]
        public Title2 Title { get; set; }

        [JsonPropertyName("date")]
        public Date4 Date { get; set; }
    }

    public class Title2
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Date4
    {
        [JsonPropertyName("CI_Date")]
        public CI_Date2 CIDate { get; set; }
    }

    public class CI_Date2
    {
        [JsonPropertyName("date")]
        public Date5 Date { get; set; }

        [JsonPropertyName("dateType")]
        public Datetype2 DateType { get; set; }
    }

    public class Date5
    {
        [JsonPropertyName("Date")]
        public string DateValue { get; set; }
    }

    public class Datetype2
    {
        [JsonPropertyName("CI_DateTypeCode")]
        public string CIDateTypeCode { get; set; }
    }

    public class Explanation
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }

    public class Pass
    {
        [JsonPropertyName("Boolean")]
        public bool Boolean { get; set; }
    }

    public class Lineage
    {
        [JsonPropertyName("LI_Lineage")]
        public LI_Lineage LILineage { get; set; }
    }

    public class LI_Lineage
    {
        [JsonPropertyName("statement")]
        public Statement Statement { get; set; }
    }

    public class Statement
    {
        [JsonPropertyName("CharacterString")]
        public string CharacterString { get; set; }
    }
} 