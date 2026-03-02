namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Реестр земельных ресурсов с указанием их плодородности
    /// </summary>
    /// <remarks>
    /// Реестр земельных ресурсов с указанием их плодородности
    /// </remarks>
    public class LandResourceFertility : NamedDictionaryItem { }

    public class LandResourceFertilityHistory : BaseHistoryEntity<LandResourceFertility> { }
}
