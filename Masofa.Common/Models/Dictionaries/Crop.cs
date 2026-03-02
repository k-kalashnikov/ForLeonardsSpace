namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Культуры
    /// </summary>
    /// <remarks>
    /// Справочник культур
    /// </remarks>
    public class Crop : NamedDictionaryItem
    {
        /// <summary>
        /// Наименование на латыни
        /// </summary>
        public string? NameLa { get; set; }

        /// <summary>
        /// Признак осуществления мониторинга
        /// </summary>
        public bool? IsMonitoring { get; set; }
    }

    public class CropHistory : BaseHistoryEntity<Crop> { }
}
