namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Представляет информацию об отчете загрузки поле
    /// </summary>
    public class ImportedFieldReport : BaseEntity
    {
        /// <summary>
        /// Комментарий к файлу
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Cсылка на файл
        /// </summary>
        public Guid FileStorageItemId { get; set; }

        /// <summary>
        /// Количество валидных контуров из файла
        /// </summary>
        public int CountValidImportedField { get; set; }

        /// <summary>
        /// Количество невалидных контуров из файла
        /// </summary>
        public int CountNotValidImportedField { get; set; }

        /// <summary>
        /// Количество полей
        /// </summary>
        public int? FieldsCount { get; set; }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long? FileSize { get; set; }
    }

    public class ImportedFieldReportHistory : BaseHistoryEntity<ImportedFieldReport> { }
}
