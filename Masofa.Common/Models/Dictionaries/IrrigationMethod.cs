namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Технология орошения
    /// </summary>
    /// <remarks>
    /// Справочник по технологиям орошения
    /// </remarks>
    public class IrrigationMethod : NamedDictionaryItem
    {
        /// <summary>
        /// Водосберегающие технологии
        /// </summary>
        public bool? IsWaterSafe { get; set; }
    }

    public class IrrigationMethodHistory : BaseHistoryEntity<IrrigationMethod> { }
}
