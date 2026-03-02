namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Статусы заявок
    /// </summary>
    /// <remarks>
    /// Справочник статусов заявок
    /// </remarks>
    public class BidState : NamedDictionaryItem { }

    public class BidStateHistory : BaseHistoryEntity<BidState> { }
}

