namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Аналитические показатели
    /// </summary>
    /// <remarks>
    /// Справочник аналитических показателей
    /// </remarks>
    public class AnalyticalIndicator : NamedDictionaryItem { }

    public class AnalyticalIndicatorHistory : BaseHistoryEntity<AnalyticalIndicator> { }
}