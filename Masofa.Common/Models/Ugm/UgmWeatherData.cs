using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Ugm
{
    [Table("UgmWeatherData")]
    [PartitionedTable]
    public class UgmWeatherData
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Id по УГМ
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// Дата прогноза
        /// </summary>
        public DateOnly? Date { get; set; }

        /// <summary>
        /// Время текущего прогноза
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// Время суток
        /// </summary>
        public DayPart DayPart { get; set; }

        /// <summary>
        /// Значек
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Температура минимальная
        /// </summary>
        public double? AirTMin { get; set; }

        /// <summary>
        /// Температура максимальная
        /// </summary>
        public double? AirTMax { get; set; }

        /// <summary>
        /// Температура средняя
        /// </summary>
        [NotMapped]
        public double? AirTAverage
        {
            get
            {
                if (AirTMin != null && AirTMax != null)
                {
                    return (AirTMin + AirTMax) / 2;
                }

                return null;
            }
        }

        /// <summary>
        /// Направление ветра
        /// </summary>
        public int? WindDirection { get; set; }

        /// <summary>
        /// Изменение напрвления ветра
        /// </summary>
        public int? WindDirectionChange { get; set; }

        /// <summary>
        /// Скорость ветра минимальная
        /// </summary>
        public int? WindSpeedMin { get; set; }

        /// <summary>
        /// Скорость ветра максимальная
        /// </summary>
        public int? WindSpeedMax { get; set; }

        /// <summary>
        /// Скорость ветра минимальная после изменения
        /// </summary>
        public int? WindSpeedMinAfterChange { get; set; }

        /// <summary>
        /// Скорость ветра максимальная после изменения
        /// </summary>
        public int? WindSpeedMaxAfterChange { get; set; }

        /// <summary>
        /// Облачность
        /// </summary>
        public string? CloudAmount { get; set; }

        /// <summary>
        /// Период
        /// </summary>
        public string? TimePeriod { get; set; }

        /// <summary>
        /// Осадки
        /// </summary>
        public string? Precipitation { get; set; }

        /// <summary>
        /// Случайно
        /// </summary>
        public int? IsOccasional { get; set; }

        /// <summary>
        /// Возможно
        /// </summary>
        public int? IsPossible { get; set; }

        /// <summary>
        /// Гроза
        /// </summary>
        public int? Thunderstorm { get; set; }

        /// <summary>
        /// Локация
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Погодный код
        /// </summary>
        public string? WeatherCode { get; set; }
    }

    public enum DayPart
    {
        Day = 1,
        Night = 2
    }
}
