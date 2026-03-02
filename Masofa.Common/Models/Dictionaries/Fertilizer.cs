namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Удобрения
    /// </summary>
    /// <remarks>
    /// Справочник удобрений
    /// </remarks>
    public class Fertilizer : NamedDictionaryItem
    {
        /// <summary>
        /// Признак экологически чистого удобрения
        /// </summary>
        public bool? IsEco { get; set; }

        /// <summary>
        /// Признак органического удобрения
        /// </summary>
        public bool? IsOrganic { get; set; }
    }

    public class FertilizerHistory : BaseHistoryEntity<Fertilizer> { }
}
