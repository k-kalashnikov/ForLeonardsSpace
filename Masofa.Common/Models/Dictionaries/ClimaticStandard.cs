using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Климатические нормы
    /// </summary>
    /// <remarks>
    /// Справочник климатических норм
    /// </remarks>
    public class ClimaticStandard : BaseDictionaryItem
    {
        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Месяц
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Число
        /// </summary>
        public int? Day { get; set; }

        /// <summary>
        /// Средняя температура
        /// </summary>
        public decimal? TempAvg { get; set; }

        /// <summary>
        /// Температура минимум
        /// </summary>
        public decimal? TempMin { get; set; }

        /// <summary>
        /// Температура максимум
        /// </summary>
        public decimal? TempMax { get; set; }

        /// <summary>
        /// Суммарное значение накопленных осадков
        /// </summary>
        public decimal? PrecDayAvg { get; set; }

        /// <summary>
        /// Суммарное значение накопленной солнечной радиции
        /// </summary>
        public decimal? RadDayAvg { get; set; }

        /// <summary>
        /// Среднее значение влажности
        /// </summary>
        public decimal? HumAvg { get; set; }

        /// <summary>
        /// Коэффициент Селянинова
        /// </summary>
        public decimal? CoefSel { get; set; }

        /// <summary>
        /// Идентификатор региона
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Region? Region { get; set; }
    }

    public class ClimaticStandardHistory : BaseHistoryEntity<ClimaticStandard> { }
}
