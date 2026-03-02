namespace Masofa.Common.Models.Identity
{
    /// <summary>
    /// Сущность описывающая мобильное устройство с которого заходит пользователь
    /// </summary>
    public class UserDevice : BaseEntity
    {
        /// <summary>
        /// Идентификатор пользователя, которому принадлежит это устройство.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Уникальный идентификатор устройства, задаваемый клиентом (например, UUID).
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Название устройства (например: "iPhone 13", "Samsung Galaxy S24").
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Платформа устройства ("iOS", "Android", "Web").
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Версия операционной системы устройства.
        /// </summary>
        public string OsVersion { get; set; }

        /// <summary>
        /// Версия установленного мобильного приложения.
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Токен для отправки push-уведомлений (Firebase/FCM или APNs).
        /// </summary>
        public string PushToken { get; set; }

        /// <summary>
        /// Указывает, активно ли устройство (можно использовать для выхода со всех устройств).
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время регистрации устройства в системе.
        /// </summary>
        public DateTime RegisteredAt { get; set; }

        /// <summary>
        /// Дата и время последней активности устройства.
        /// Может быть null, если активность ещё не отслеживалась.
        /// </summary>
        public DateTime? LastActiveAt { get; set; }
    }

    public class UserDeviceHistory : BaseHistoryEntity<UserDevice> { }
}
