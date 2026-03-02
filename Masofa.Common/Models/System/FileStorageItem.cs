using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.SystemCrical
{
    /// <summary>
    /// Сохраненный файл
    /// </summary>
    /// <remarks>
    /// Справочник сохраненных файлов
    /// </remarks>
    public class FileStorageItem : BaseEntity
    {
        /// <summary>
        /// Владелец
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Тип владельца
        /// </summary>
        public string OwnerTypeFullName { get; set; }

        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FileStoragePath { get; set; }

        /// <summary>
        /// Бакет где лежит файл
        /// </summary>
        public string FileStorageBacket { get; set; }

        /// <summary>
        /// Наименование типа файла
        /// </summary>
        public string FileContentTypeName
        {
            get
            {
                switch (FileContentType)
                {
                    case FileContentType.ImageJPG:
                        return "image/JPG";
                    case FileContentType.ImagePNG:
                        return "image/PNG";
                    case FileContentType.ImageWEBP:
                        return "image/WEBP";
                    case FileContentType.ImageTiff:
                        return "image/TIFF";
                    default:
                        return "application/octet-stream";
                }
            }
        }

        /// <summary>
        /// Тип файла
        /// </summary>
        public FileContentType FileContentType { get; set; }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long? FileLength { get; set; }

        /// <summary>
        /// Тип владельца
        /// </summary>
        [NotMapped]
        public Type OwnerType { get; set; }
    }

    /// <summary>
    /// Возможные типы файлов
    /// </summary>
    public enum FileContentType
    {
        /// <summary>
        /// По умолчанию
        /// </summary>
        Default = 0,

        /// <summary>
        /// image/JPG
        /// </summary>
        ImageJPG = 1,

        /// <summary>
        /// image/PNG
        /// </summary>
        ImagePNG = 2,

        /// <summary>
        /// image/WEBP
        /// </summary>
        ImageWEBP = 3,

        /// <summary>
        /// application/octet-stream
        /// </summary>
        ArchiveZIP = 4,

        /// <summary>
        /// image/TIFF
        /// </summary>
        ImageTiff = 5,

        /// <summary>
        /// text/html
        /// </summary>
        HtmlFile = 6, 
    }

    public class FileStorageItemHistory : BaseHistoryEntity<FileStorageItem> { }
}
