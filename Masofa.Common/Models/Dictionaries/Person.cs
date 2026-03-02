using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
	/// <summary>
	/// Физические лица
	/// </summary>
	/// <remarks>
	/// Справочник физических лиц
	/// </remarks>
	public class Person : BaseDictionaryItem
	{
		/// <summary>
		/// Налоговый номер ФЛ
		/// </summary>
		public string? Pinfl { get; set; }

		/// <summary>
		/// Идентификатор пользователя в системе
		/// </summary>
		public Guid? UserId { get; set; }

		/// <summary>
		/// Полное имя (ФИО)
		/// </summary>
		public string? FullName { get; set; }

		/// <summary>
		/// Телеграм
		/// </summary>
		public string? Telegram { get; set; }

		/// <summary>
		/// Телефоны
		/// </summary>
		public string? Phones { get; set; }

		/// <summary>
		/// Email
		/// </summary>
		public string? Email { get; set; }

		/// <summary>
		/// Основной регион
		/// </summary>
		public Guid? MainRegionId { get; set; }

		/// <summary>
		/// Адрес осуществления деятельности
		/// </summary>
		public string? PhysicalAddress { get; set; }

		/// <summary>
		/// Почтовый адрес
		/// </summary>
		public string? MailingAddress { get; set; }

		/// <summary>
		/// Основной регион
		/// </summary>
		[NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [SwaggerIgnore]
        public Region? MainRegion { get; set; }

		/// <summary>
		/// Виды деятельности
		/// </summary>
		[NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [SwaggerIgnore]
        public ICollection<BusinessType> BusinessTypes { get; set; } = [];
	}

    public class BusinessTypePerson : BaseEntity
    {
		public Guid PersonId { get; set; }
		public Guid BusinessTypeId { get; set; }
    }

	public class PersonHistory : BaseHistoryEntity<Person> { }

	public class BusinessTypePersonHistory : BaseHistoryEntity<BusinessTypePerson> { }
}
