namespace Masofa.Web.Monolith.ViewModels.ImportedField
{
    /// <summary>
    /// Запрос на сохранение загружаемых полей
    /// </summary>
    public class MigrationImportedFieldViewModel
    {
        /// <summary>
        /// Идентификатор сессии загружаемых полей
        /// </summary>
        public Guid ImportedFieldReportId { get; set; }

        /// <summary>
        /// Сопоставление атрибутов сущности ImportedField и полей сущности Season
        /// Key: SeasonPropertyName, Value: List of ImportedFieldAttributeNames
        /// </summary>
        public Dictionary<string, List<string>> FieldMappings { get; set; } = [];

        /// <summary>
        /// Значения по умолчанию для посева
        /// </summary>
        public Masofa.Common.Models.CropMonitoring.Season? DefaultSeason { get; set; }
    }
}
