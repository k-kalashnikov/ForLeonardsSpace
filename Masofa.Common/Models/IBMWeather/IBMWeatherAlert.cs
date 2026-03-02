using System.ComponentModel.DataAnnotations.Schema;
using Masofa.Common.Models;

namespace Masofa.Common.Models.IBMWeather
{

    /// <summary>
    /// Weather alert entity for IBM Weather API
    /// </summary>
    [Table("IBMWeatherAlerts")]
    public class IBMWeatherAlert : BaseNamedEntity
    {
        /// <summary>
        /// Идентификатор метеостанции IBM
        /// </summary>
        public Guid IBMMeteoStationId { get; set; }

        /// <summary>
        /// Административный округ
        /// </summary>
        public string? AdminDistrict { get; set; }

        /// <summary>
        /// Код административного округа
        /// </summary>
        public string? AdminDistrictCode { get; set; }

        /// <summary>
        /// Идентификатор области
        /// </summary>
        public string AreaId { get; set; } = string.Empty;

        /// <summary>
        /// Название области
        /// </summary>
        public string AreaName { get; set; } = string.Empty;

        /// <summary>
        /// Код типа области (C - county, Z - zone, CLC - Canada Location Code)
        /// </summary>
        public string AreaTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// Уверенность (Observed, Likely, Possible)
        /// </summary>
        public string Certainty { get; set; } = string.Empty;

        /// <summary>
        /// Код уверенности (1 - Observed, 2 - Likely, 3 - Possible)
        /// </summary>
        public string CertaintyCode { get; set; } = string.Empty;

        /// <summary>
        /// Код страны
        /// </summary>
        public string CountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Название страны
        /// </summary>
        public string CountryName { get; set; } = string.Empty;

        /// <summary>
        /// Ключ для получения детализации
        /// </summary>
        public string DetailKey { get; set; } = string.Empty;

        /// <summary>
        /// Отказ от ответственности
        /// </summary>
        public string? Disclaimer { get; set; }

        /// <summary>
        /// Ранг отображения
        /// </summary>
        public int DisplayRank { get; set; }

        /// <summary>
        /// Время вступления в силу (локальное)
        /// </summary>
        public DateTime? EffectiveTimeLocal { get; set; }

        /// <summary>
        /// Часовой пояс времени вступления в силу
        /// </summary>
        public string? EffectiveTimeLocalTimeZone { get; set; }

        /// <summary>
        /// Описание события
        /// </summary>
        public string EventDescription { get; set; } = string.Empty;

        /// <summary>
        /// Номер отслеживания события
        /// </summary>
        public string EventTrackingNumber { get; set; } = string.Empty;

        /// <summary>
        /// Время истечения (локальное)
        /// </summary>
        public DateTime ExpireTimeLocal { get; set; }

        /// <summary>
        /// Часовой пояс времени истечения
        /// </summary>
        public string ExpireTimeLocalTimeZone { get; set; } = string.Empty;

        /// <summary>
        /// Время истечения (UTC)
        /// </summary>
        public DateTime ExpireTimeUTC { get; set; }

        /// <summary>
        /// Текст заголовка
        /// </summary>
        public string HeadlineText { get; set; } = string.Empty;

        /// <summary>
        /// IANA часовой пояс
        /// </summary>
        public string? IanaTimeZone { get; set; }

        /// <summary>
        /// Уникальный идентификатор алерта (checksum)
        /// </summary>
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Время выпуска (локальное)
        /// </summary>
        public DateTime IssueTimeLocal { get; set; }

        /// <summary>
        /// Часовой пояс времени выпуска
        /// </summary>
        public string IssueTimeLocalTimeZone { get; set; } = string.Empty;

        /// <summary>
        /// Широта
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Тип сообщения (New, Update, Cancel)
        /// </summary>
        public string MessageType { get; set; } = string.Empty;

        /// <summary>
        /// Код типа сообщения (1 - New, 2 - Update, 3 - Cancel)
        /// </summary>
        public int MessageTypeCode { get; set; }

        /// <summary>
        /// Административный округ офиса
        /// </summary>
        public string? OfficeAdminDistrict { get; set; }

        /// <summary>
        /// Код административного округа офиса
        /// </summary>
        public string? OfficeAdminDistrictCode { get; set; }

        /// <summary>
        /// Код офиса-источника (например, NWS)
        /// </summary>
        public string OfficeCode { get; set; } = string.Empty;

        /// <summary>
        /// Код страны офиса
        /// </summary>
        public string? OfficeCountryCode { get; set; }

        /// <summary>
        /// Название офиса
        /// </summary>
        public string OfficeName { get; set; } = string.Empty;

        /// <summary>
        /// Время начала (локальное)
        /// </summary>
        public DateTime? OnsetTimeLocal { get; set; }

        /// <summary>
        /// Часовой пояс времени начала
        /// </summary>
        public string? OnsetTimeLocalTimeZone { get; set; }

        /// <summary>
        /// Код явления (TO, SU, FL и т.д.)
        /// </summary>
        public string Phenomena { get; set; } = string.Empty;

        /// <summary>
        /// Время обработки (UTC)
        /// </summary>
        public DateTime ProcessTimeUTC { get; set; }

        /// <summary>
        /// Идентификатор продукта
        /// </summary>
        public string ProductIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// Серьезность (Extreme, Severe, Moderate, Minor, Unknown)
        /// </summary>
        public string Severity { get; set; } = string.Empty;

        /// <summary>
        /// Код серьезности (1 - Extreme, 2 - Severe, ...)
        /// </summary>
        public int SeverityCode { get; set; }

        /// <summary>
        /// Значимость (W - Warning, Y - Advisory, O - Outlook)
        /// </summary>
        public string Significance { get; set; } = string.Empty;

        /// <summary>
        /// Источник
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Срочность (Immediate, Expected, Future, Past, Unknown)
        /// </summary>
        public string Urgency { get; set; } = string.Empty;

        /// <summary>
        /// Код срочности
        /// </summary>
        public int UrgencyCode { get; set; }

        /// <summary>
        /// Время окончания (локальное)
        /// </summary>
        public DateTime? EndTimeLocal { get; set; }

        /// <summary>
        /// Часовой пояс времени окончания
        /// </summary>
        public string? EndTimeLocalTimeZone { get; set; }

        /// <summary>
        /// Время окончания (UTC)
        /// </summary>
        public DateTime? EndTimeUTC { get; set; }

        /// <summary>
        /// Идентификатор типа ответа
        /// </summary>
        public IBMWeatherAlertResponseTypeEnum WeatherAlertResponseType { get; set; }

        /// <summary>
        /// Идентификатор категории
        /// </summary>
        public IBMWeatherAlertCategoryEnum WeatherAlertCategory { get; set; }
    }

    [Flags]
    public enum IBMWeatherAlertCategoryEnum
    {
        undefined = 0,

        Geo = 1 << 0,        // 1     — Geophysical (землетрясения, оползни)
        Met = 1 << 1,        // 2     — Meteorological (погода)
        Safety = 1 << 2,     // 4     — General emergency and public safety
        Security = 1 << 3,   // 8     — Law enforcement, military/security
        Rescue = 1 << 4,     // 16    — Rescue and recovery
        Fire = 1 << 5,       // 32    — Fire suppression and rescue
        Health = 1 << 6,     // 64    — Medical and public health
        Env = 1 << 7,        // 128   — Pollution/environmental
        Transport = 1 << 8,  // 256   — Public/private transportation
        Infra = 1 << 9,      // 512   — Utilities/infrastructure
        CBRNE = 1 << 10,     // 1024  — Chemical/Biological/Radiological/Nuclear/Explosive
        Other = 1 << 11      // 2048  — Other
    }

    [Flags]
    public enum IBMWeatherAlertResponseTypeEnum
    {
        undefined = 0,

        Shelter = 1 << 0,     // Take shelter
        Evacuate = 2 << 0,    // Evacuate
        Prepare = 3 << 0,     // Prepare
        Execute = 4 << 0,     // Execute
        Avoid = 5 << 0,       // Avoid
        Monitor = 6 << 0,     // Monitor
        Assess = 7 << 0,      // Assess
        AllClear = 8 << 0,    // All clear
        None = 9 << 0         // No action required
    }
}