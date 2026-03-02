namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Единицы измерения
    /// </summary>
    /// <remarks>
    /// Справочник единиц измерения
    /// </remarks>
    public class MeasurementUnit : NamedDictionaryItem
    {
        /// <summary>
        /// Полное наименование 
        /// </summary>
        public LocalizationString FullNames { get; set; } = new LocalizationString();

        /// <summary>
        /// Единица СИ
        /// </summary>
        public string? SiUnit { get; set; }

        /// <summary>
        /// Коэффициент (множитель) единицы СИ
        /// </summary>
        public decimal? Factor { get; set; }
    }

    public class MeasurementUnitHistory : BaseHistoryEntity<MeasurementUnit> { }
}
