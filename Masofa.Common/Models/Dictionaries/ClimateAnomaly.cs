namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Справочник климатических аномалий и их воздействий на сельское хозяйство
    /// </summary>
    /// <remarks>
    /// Справочник климатических аномалий и их воздействий на сельское хозяйство
    /// </remarks>
    public class ClimateAnomaly : NamedDictionaryItem { }

    public class ClimateAnomalyHistory : BaseHistoryEntity<ClimateAnomaly> { }
}
