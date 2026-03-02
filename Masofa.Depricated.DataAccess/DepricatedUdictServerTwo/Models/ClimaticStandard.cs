using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник климатических норм
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.ClimaticStandard))]
public partial class ClimaticStandard
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

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

    public virtual Region? Region { get; set; }
}
