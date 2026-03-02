using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Indices
{
    /// <summary>
    /// Базовый класс отчетов по посевам для новых продуктов на базе индексов.
    /// </summary>
    public class BaseIndexReport
    {
        /// <summary>
		/// Идентификатор
		/// </summary>
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateOnly DateOnly { get; set; }

        /// <summary>
		/// Среднее значение индекса
		/// </summary>
        public double Average { get; set; }

        /// <summary>
		/// Максимальное значение по точкам
		/// </summary>
        public double TotalMax { get; set; }

        /// <summary>
		/// Минимальное значение по точкам
		/// </summary>
        public double TotalMin { get; set; }

        /// <summary>
		/// Идентификатор региона
		/// </summary>
        public Guid? RegionId { get; set; }

        [NotMapped]
        public double PosibleMaxValue { get; set; }

        [NotMapped]
        public double PosibleMinValue { get; set; }
    }

    /// <summary>
    /// Отчёт для посева
    /// </summary>
    public class IndexReportSeason : BaseIndexReport
    {
        /// <summary>
		/// Идентификатор посева
		/// </summary>
        public Guid SeasonId { get; set; }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры
    /// </summary>
    public class IndexReportShared : BaseIndexReport
    {
        /// <summary>
		/// Идентификатор культуры
		/// </summary>
        public Guid? CropId { get; set; }

        /// <summary>
		/// Среднее максимальное значение индекса по культуре
		/// </summary>
        public double AverageMax { get; set; }

        /// <summary>
		/// Среднее минимальное значение индекса по культуре
		/// </summary>
        public double AverageMin { get; set; }
    }

    /// <summary>
    /// Конкретный отчёт ARVI по сезону
    /// </summary>
    public class ArviSeasonReport : IndexReportSeason
    {
        public ArviSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Конкретный отчёт EVI по сезону
    /// </summary>
    public class EviSeasonReport : IndexReportSeason
    {
        public EviSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Конкретный отчёт GNDVI по сезону
    /// </summary>
    public class GndviSeasonReport : IndexReportSeason
    {
        public GndviSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Конкретный отчёт MNDWI по сезону
    /// </summary>
    public class MndwiSeasonReport : IndexReportSeason
    {
        public MndwiSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Конкретный отчёт NDMI по сезону
    /// </summary>
    public class NdmiSeasonReport : IndexReportSeason
    {
        public NdmiSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Конкретный отчёт NDVI по сезону
    /// </summary>
    public class NdviSeasonReport : IndexReportSeason
    {
        public NdviSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Конкретный отчёт ORVI по сезону
    /// </summary>
    public class OrviSeasonReport : IndexReportSeason
    {
        public OrviSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Конкретный отчёт OSAVI по сезону
    /// </summary>
    public class OsaviSeasonReport : IndexReportSeason
    {
        public OsaviSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Конкретный отчёт NDWI по сезону
    /// </summary>
    public class NdwiSeasonReport : IndexReportSeason
    {
        public NdwiSeasonReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу ARVI
    /// </summary>
    public class ArviSharedReport : IndexReportShared
    {
        public ArviSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу EVI
    /// </summary>
    public class EviSharedReport : IndexReportShared
    {
        public EviSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу GNDVI
    /// </summary>
    public class GndviSharedReport : IndexReportShared
    {
        public GndviSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу MNDWI
    /// </summary>
    public class MndwiSharedReport : IndexReportShared
    {
        public MndwiSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу NDMI
    /// </summary>
    public class NdmiSharedReport : IndexReportShared
    {
        public NdmiSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу NDVI 
    /// </summary>
    public class NdviSharedReport : IndexReportShared
    {
        public NdviSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу ORVI
    /// </summary>
    public class OrviSharedReport : IndexReportShared
    {
        public OrviSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу OSAVI
    /// </summary>
    public class OsaviSharedReport : IndexReportShared
    {
        public OsaviSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }

    /// <summary>
    /// Отчёт для всех посевов одной культуры по индексу NDWI
    /// </summary>
    public class NdwiSharedReport : IndexReportShared
    {
        public NdwiSharedReport()
        {
            PosibleMinValue = -1.0;
            PosibleMaxValue = 1.0;
        }
    }
}
