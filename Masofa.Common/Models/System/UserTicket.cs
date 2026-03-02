using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.SystemCrical
{
    public class UserTicket : BaseEntity
    {
        /// <summary>
        /// Имя создателя тикета
        /// </summary>
        public string? CreateUserName { get; set; }

        /// <summary>
        /// Почта создателя тикета
        /// </summary>
        public string? CreateUserEmail { get; set; }

        /// <summary>
        /// Тип ошибки
        /// </summary>
        public string ExceptionType { get; set; } = null!;

        /// <summary>
        /// Ошибка
        /// </summary>
        public string ExceptionJson { get; set; } = null!;

        /// <summary>
        /// Название модуля по которому создан тикет
        /// </summary>
        public string ModuleName { get; set; } = null!;

        /// <summary>
        /// Айди девайса
        /// </summary>
        public Guid? UserDeviceId { get; set; }

        /// <summary>
        /// Тип девайса
        /// </summary>
        public string? UserDeviceType { get; set; }

        /// <summary>
        /// Дейсон девайса
        /// </summary>
        public string UserDeviceJson { get; set; } = null!;

        /// <summary>
        /// Статус тикета
        /// </summary>
        public UserTicketStatus TicketStatus { get; set; } = UserTicketStatus.New;

        /// <summary>
        /// Номер тикета
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Number { get; set; }
    }

    public enum UserTicketStatus
    {
        New = 0,
        InProgress = 1,
        Rejected = 2,
        Complite = 3,
    }

    public class UserTicketHistory : BaseHistoryEntity<UserTicket> { }
}
