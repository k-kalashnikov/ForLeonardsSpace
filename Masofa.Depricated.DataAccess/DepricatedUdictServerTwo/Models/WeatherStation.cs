using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник погодных станций
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.WeatherStation))]
public partial class WeatherStation
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
    /// Идентификатор класса
    /// </summary>
    public Guid? ClassId { get; set; }

    /// <summary>
    /// Признак автоматизированной передачи данных
    /// </summary>
    public bool? IsAuto { get; set; }

    /// <summary>
    /// Широта
    /// </summary>
    public decimal? Lat { get; set; }

    /// <summary>
    /// Долгота
    /// </summary>
    public decimal? Lng { get; set; }

    /// <summary>
    /// Радиус покрытия
    /// </summary>
    public decimal? Radius { get; set; }

    /// <summary>
    /// Тип станции (станция/пост)
    /// </summary>
    public Guid? TypeId { get; set; }

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
    /// Идентификатор компании-владельца станции
    /// </summary>
    public Guid? FirmId { get; set; }

    /// <summary>
    /// Наименование на английском
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// Наименование на русском
    /// </summary>
    public string? NameRu { get; set; }

    /// <summary>
    /// Наименование на узбекском
    /// </summary>
    public string? NameUz { get; set; }

    //public virtual Firm? Firm { get; set; }

    //public virtual Region? Region { get; set; }

    //public virtual WeatherStationType? Type { get; set; }
}
