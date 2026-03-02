using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

/// <summary>
/// Справочник физических лиц
/// </summary>
[MigrationCompare(CompareToType = typeof(Masofa.Common.Models.Dictionaries.Person))]
public partial class Person
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Налоговый номер ФЛ
    /// </summary>
    public string? Pinfl { get; set; }

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
    /// Идентификатор пользователя в системе
    /// </summary>
    public Guid? UserId { get; set; }

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
    /// Адрес осуществления деятельности
    /// </summary>
    public string? PhysicalAddress { get; set; }

    /// <summary>
    /// Телеграм
    /// </summary>
    public string? Telegram { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; set; }

    //public virtual Region? MainRegion { get; set; }

    //public virtual ICollection<BusinessType> BusinessTypes { get; set; } = new List<BusinessType>();
}
