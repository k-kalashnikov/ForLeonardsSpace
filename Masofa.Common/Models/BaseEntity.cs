using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models
{
    /// <summary>
    /// Основная сущность
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [SystemFieldNotForImport]
        public Guid Id { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [SystemFieldNotForImport]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Статус сущности
        /// </summary>
        [SystemFieldNotForImport]
        public StatusType Status { get; set; } = StatusType.Active;

        /// <summary>
        /// Дата обновления
        /// </summary>
        [SystemFieldNotForImport]
        public DateTime LastUpdateAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Автор
        /// </summary>
        [SystemFieldNotForImport]
        public Guid CreateUser { get; set; }

        /// <summary>
        /// Автор обновления
        /// </summary>
        [SystemFieldNotForImport]
        public Guid LastUpdateUser { get; set; }

        /// <summary>
        /// Показатель публикации данных
        /// </summary>
        public bool IsPublic { get; set; } = false;
    }

    /// <summary>
    /// Возможные статусы сущности
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// Удалена
        /// </summary>
        Deleted = 0,

        /// <summary>
        /// Активна
        /// </summary>
        Active = 1,

        /// <summary>
        /// Скрыта
        /// </summary>
        Hiden = 2
	}

	public class BaseEntityHistoryItem<TModel> where TModel : BaseEntity
	{
		[Key]
		public Guid Id { get; set; }
		public DateTime DateTime { get; set; }
		public Guid ModelId { get; set; }

		public string OldJson 
		{
			get
			{
				return Newtonsoft.Json.JsonConvert.SerializeObject(Old);
			}
			set
			{
				Old = Newtonsoft.Json.JsonConvert.DeserializeObject<TModel>(value ?? string.Empty) ?? default;
			} 
		}
		public string NewJson 
		{
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(New);
            }
            set
            {
                New = Newtonsoft.Json.JsonConvert.DeserializeObject<TModel>(value ?? string.Empty) ?? default;
            }
        }

		[NotMapped]
		public TModel Old { get; set; }

        [NotMapped]
        public TModel New { get; set; }
	}
}
