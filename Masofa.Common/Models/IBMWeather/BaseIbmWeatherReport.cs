using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.IBMWeather
{
    public class BaseIbmWeatherReport
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Температура средняя
        /// </summary>
        [NotMapped]
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureAverage
        {
            get
            {
                return (TemperatureMin + TemperatureMax) / 2.0;
            }
        }

        /// <summary>
        /// Температура максимальная (средняя)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMin { get; set; }

        /// <summary>
        /// Температура минимальная (средняя)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMax { get; set; }

        /// <summary>
        /// Температура максимальная (максимальная)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMinTotal { get; set; }

        /// <summary>
        /// Температура минимальная (минимальная)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMaxTotal { get; set; }

        /// <summary>
        /// Осадки
        /// </summary>
        [ReportValue(ColorTable = "Fallout")]
        public double Fallout { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        [ReportValue(ColorTable = "Humidity")]
        public double Humidity { get; set; }

        /// <summary>
        /// Скорость ветра
        /// </summary>
        public double WindSpeed { get; set; }

        /// <summary>
        /// Направление ветра
        /// </summary>
        public double WindDerection { get; set; }

        /// <summary>
        /// Точка Era5
        /// </summary>
        public Guid? WeatherStation { get; set; }
    }
}
