using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник типов административных единиц
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.AdministrativeUnit))]
public partial class AdministrativeUnit
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Уровень иерархии
    /// </summary>
    public int? Level { get; set; }

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

    //public static implicit operator Masofa.Common.Models.Dictionaries.AdministrativeUnit(AdministrativeUnit administrativeUnit)
    //{
    //    return new Common.Models.Dictionaries.AdministrativeUnit()
    //    {
    //        Id = administrativeUnit.Id,
    //        Comment = administrativeUnit.Comment,
    //        CreateAt = administrativeUnit.CreateDate.ToDateTimeUtc(),
    //        ExtData = administrativeUnit.ExtData,
    //        LastUpdateAt = administrativeUnit.UpdateDate.ToDateTimeUtc(),
    //        Level = administrativeUnit.Level,
    //        NameEn = administrativeUnit.NameEn,
    //        NameRu = administrativeUnit.NameRu,
    //        NameUz = administrativeUnit.NameUz,
    //        OrderCode = administrativeUnit.OrderCode,
    //        Visible = administrativeUnit.Visible
    //    };
    //}
}
