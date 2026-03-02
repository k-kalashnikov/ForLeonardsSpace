namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Виды сх техники
    /// </summary>
    /// <remarks>
    /// Справочник видов сельскохозяйственной техники
    /// </remarks>
    public class AgroMachineType : NamedDictionaryItem
    {
        /// <summary>
		/// Признак почовосберегающей техники
		/// </summary>
        public bool? IsSoilSafe { get; set; }
    }

    public class AgroMachineTypeHistory : BaseHistoryEntity<AgroMachineType> { }
}

