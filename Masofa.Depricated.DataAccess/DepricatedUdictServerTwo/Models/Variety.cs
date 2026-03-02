using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник сортов
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.Variety))]
public partial class Variety
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор культуры
    /// </summary>
    public Guid CropId { get; set; }

    /// <summary>
    /// Наименование на латыни
    /// </summary>
    public string? NameLa { get; set; }

    /// <summary>
    /// Публикация
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Код сортировки
    /// </summary>
    public string? OrderCode { get; set; }

    /// <summary>
    /// Дополнительная информация
    /// </summary>
    public string? ExtData { get; set; }

    /// <summary>
    /// Комментарий
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public Instant CreateDate { get; set; }

    /// <summary>
    /// Автор
    /// </summary>
    public string CreateUser { get; set; } = null!;

    /// <summary>
    /// Дата обновления
    /// </summary>
    public Instant UpdateDate { get; set; }

    /// <summary>
    /// Автор обновления
    /// </summary>
    public string UpdateUser { get; set; } = null!;

    /// <summary>
    /// Наименование на узбекском
    /// </summary>
    public string? NameUz { get; set; }

    /// <summary>
    /// Наименование на английском
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// Наименование на русском
    /// </summary>
    public string? NameRu { get; set; }

    /// <summary>
    /// Средняя урожайность (ц/Га)
    /// </summary>
    public decimal? AverageYield { get; set; }

    /// <summary>
    /// Срок созревания в днях
    /// </summary>
    public int? RipeningPeriod { get; set; }

    //public virtual ICollection<AgrotechnicalMeasure> AgrotechnicalMeasures { get; set; } = new List<AgrotechnicalMeasure>();

    //public virtual Crop Crop { get; set; } = null!;

    //public virtual ICollection<CropPeriod> CropPeriods { get; set; } = new List<CropPeriod>();

    //public virtual ICollection<SolarRadiationInfluence> SolarRadiationInfluences { get; set; } = new List<SolarRadiationInfluence>();

    //public virtual ICollection<VegetationPeriod> VegetationPeriods { get; set; } = new List<VegetationPeriod>();

    //public virtual ICollection<VarietyFeature> Features { get; set; } = new List<VarietyFeature>();

    //public virtual ICollection<Region> Regions { get; set; } = new List<Region>();
}
