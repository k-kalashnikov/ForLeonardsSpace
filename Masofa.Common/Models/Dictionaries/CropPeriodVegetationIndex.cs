using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Индексы периодов развития
    /// </summary>
    /// <remarks>
    /// Справочник индексов периодов развития культур
    /// </remarks>
    public class CropPeriodVegetationIndex : BaseDictionaryItem
    {
        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        public Guid CropId { get; set; }

        /// <summary>
        /// Идентификатор периода развития культуры
        /// </summary>
        public Guid CropPeriodId { get; set; }

        /// <summary>
        /// Идентификатор вегетационного индекса
        /// </summary>
        public Guid VegetationIndexId { get; set; }

        /// <summary>
        /// Тип вегетационного индекса
        /// </summary>
        public VegetationIndexType VegetationIndexType { get; set; }

        /// <summary>
        /// Значение индекса
        /// </summary>
        [NotMapped]
        public decimal? Value 
        {
            get
            {
                if (Min != null && Max != null)
                {
                    return (Min + Max) / 2;
                }
                return default;
            }
        }

        /// <summary>
        /// Минимальное значение индекса
        /// </summary>
        public decimal? Min { get; set; }

        /// <summary>
        /// Максимальное значение индекса
        /// </summary>
        public decimal? Max { get; set; }

        public double BioMass { get; set; }
    }

    public class CropPeriodVegetationIndexHistory : BaseHistoryEntity<CropPeriodVegetationIndex> { }

    public enum VegetationIndexType
    {
        ARVI = 1,
        EVI = 2,
        GNDVI = 3,
        MNDWI = 4,
        NDMI = 5,
        NDVI = 6,
        NDWI = 7,
        ORVI = 8,
        OSAVI = 9
    }
}
