using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Юридические лица
    /// </summary>
    /// <remarks>
    /// Справочник юридических лиц
    /// </remarks>
    public class Firm : BaseDictionaryItem
    {
        /// <summary>
        /// Налоговый номер ЮЛ (ИНН)
        /// </summary>
        public string? Inn { get; set; }

        /// <summary>
        /// Регистрационный номер ЮЛ (ЕГРПО)
        /// </summary>
        public string? Egrpo { get; set; }

        /// <summary>
        /// Сайт
        /// </summary>
        public string? Site { get; set; }

        /// <summary>
        /// Телефоны
        /// </summary>
        public string? Phones { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        public string? Chief { get; set; }

        /// <summary>
        /// Основной регион
        /// </summary>
        public Guid? MainRegionId { get; set; }

        /// <summary>
        /// Адрес нахождения
        /// </summary>
        public string? PhysicalAddress { get; set; }

        /// <summary>
        /// Почтовый адрес
        /// </summary>
        public string? MailingAddress { get; set; }

        /// <summary>
        /// Краткое наименование
        /// </summary>
        [Required]
        public string ShortName { get; set; } = null!;

        /// <summary>
        /// Полное наименование
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Международное наименование
        /// </summary>
        public string? InternationalName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Point? Location { get; set; }

        [NotMapped]
        public string? LocationJson
        {
            get
            {
                if (Location == null || Location.IsEmpty)
                {
                    return null;
                }
                return Location.AsText();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Location = null;
                }
                else
                {
                    try
                    {
                        var reader = new WKTReader();
                        var geometry = reader.Read(value);
                        if (geometry is Point point)
                        {
                            Location = point;
                            Location.SRID = 4326;
                        }
                        else
                        {
                            Location = null;
                        }
                    }
                    catch
                    {
                        Location = null;
                    }
                }
            }
        }

        /// <summary>
        /// Основной регион
        /// </summary>
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [SwaggerIgnore]
        public Region? MainRegion { get; set; } = new();

        /// <summary>
        /// Виды деятельности
        /// </summary>
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [SwaggerIgnore]
        public ICollection<BusinessType> BusinessTypes { get; set; } = [];

        /// <summary>
        /// Погодные станции
        /// </summary>
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [SwaggerIgnore]
        public ICollection<WeatherStation> WeatherStations { get; set; } = [];
    }

    public class BusinessTypeFirm : BaseEntity
    {
        public Guid FirmId { get; set; }
        public Guid BusinessTypeId { get; set; }
    }

    public class FirmHistory : BaseHistoryEntity<Firm> { }

    public class BusinessTypeFirmHistory : BaseHistoryEntity<BusinessTypeFirm> { }
}
