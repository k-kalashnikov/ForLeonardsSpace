using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
	/// <summary>
	/// Пестициды и агрохимикаты
	/// </summary>
	/// <remarks>
	/// Справочник пестицидов и агрохимикатов
	/// </remarks>
	public class Pesticide : NamedDictionaryItem
	{
		/// <summary>
		/// Идентификатор вида пестицида
		/// </summary>
		public Guid? PesticideTypeId { get; set; }

		/// <summary>
		/// Международный код
		/// </summary>
		public string? IntCode { get; set; }

		/// <summary>
		/// Идентификатор вида пестицида
		/// </summary>
		[NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public PesticideType? PesticideType { get; set; }
	}

	public class PesticideHistory : BaseHistoryEntity<Pesticide> { }
}
