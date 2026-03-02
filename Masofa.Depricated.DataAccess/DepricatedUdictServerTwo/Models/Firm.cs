using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник юридических лиц
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.Firm))]
public partial class Firm
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Налоговый номер ЮЛ (ИНН)
    /// </summary>
    public string? Inn { get; set; }

    /// <summary>
    /// Регистрационный номер ЮЛ (ЕГРПО)
    /// </summary>
    public string? Egrpo { get; set; }

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
    /// Руководитель
    /// </summary>
    public string? Chief { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Почтовый адрес
    /// </summary>
    public string? MailingAddress { get; set; }

    /// <summary>
    /// Основной регион
    /// </summary>
    public Guid? MainRegionId { get; set; }

    /// <summary>
    /// Телефоны
    /// </summary>
    public string? Phones { get; set; }

    /// <summary>
    /// Адрес нахождения
    /// </summary>
    public string? PhysicalAddress { get; set; }

    /// <summary>
    /// Сайт
    /// </summary>
    public string? Site { get; set; }

    /// <summary>
    /// Полное наименование
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Международное наименование
    /// </summary>
    public string? InternationalName { get; set; }

    /// <summary>
    /// Краткое наименование
    /// </summary>
    public string ShortName { get; set; } = null!;

    //public virtual Region? MainRegion { get; set; }

    //public virtual ICollection<WeatherStation> WeatherStations { get; set; } = new List<WeatherStation>();

    //public virtual ICollection<BusinessType> BusinessTypes { get; set; } = new List<BusinessType>();
}
