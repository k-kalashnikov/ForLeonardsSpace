using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник видов сельскохозяйственной техники
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.AgroMachineType))]
public partial class AgroMachineType
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Признак почовосберегающей техники
    /// </summary>
    public bool? IsSoilSafe { get; set; }

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

    //public static implicit operator Masofa.Common.Models.Dictionaries.AgroMachineType(AgroMachineType administrativeUnit)
    //{
    //    return new Common.Models.Dictionaries.AgroMachineType()
    //    {
    //        Id = administrativeUnit.Id,
    //        Comment = administrativeUnit.Comment,
    //        CreateAt = administrativeUnit.CreateDate.ToDateTimeUtc(),
    //        ExtData = administrativeUnit.ExtData,
    //        LastUpdateAt = administrativeUnit.UpdateDate.ToDateTimeUtc(),
    //        NameEn = administrativeUnit.NameEn,
    //        NameRu = administrativeUnit.NameRu,
    //        NameUz = administrativeUnit.NameUz,
    //        OrderCode = administrativeUnit.OrderCode,
    //        Visible = administrativeUnit.Visible,
    //        IsSoilSafe = administrativeUnit.IsSoilSafe
    //    };
    //}
}
