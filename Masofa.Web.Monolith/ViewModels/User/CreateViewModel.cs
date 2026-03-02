using Masofa.Common.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace Masofa.Web.Monolith.ViewModels.User
{
    public class CreateViewModel
    {
        /// <summary>
        /// Имя НОВОГО пользователя
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Имя нового пользователя
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Второе имя нового пользователя
        /// </summary>
        public string? SecondName { get; set; }

        /// <summary>
        /// Фамилия нового пользователя
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Required]
        [Compare(nameof(Password),  ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Электронная почта пользователя
        /// </summary>
        [Required]
        public required string Email { get; set; }

        /// <summary>
        /// Одобрен
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Email подтвержден
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Заблокировать пользователя
        /// </summary>
        public bool LockUser { get; set; }

        /// <summary>
        /// Дата и время начала блокировки пользователя
        /// </summary>
        public DateTime? LockoutStart { get; set; }

        /// <summary>
        /// Дата и время окончания блокировки пользователя
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Тип пользователя
        /// </summary>
        public UserBusinessType UserBusinessType { get; set; }

        /// <summary>
        /// Ссылка на физ или юр лицо
        /// </summary>
        public Guid UserBusinessId { get; set; }

        /// <summary>
        /// Роли в которые добавить пользователя при создании
        /// </summary>
        public List<string> Roles { get; set; } = [];

        /// <summary>
        /// Комментарий к пользователю
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Идентификатор заместителя пользователя
        /// </summary>
        public Guid? DeputyId { get; set; }

        /// <summary>
        /// Основание подключения
        /// </summary>
        public string? ConnectionBasis { get; set; }

        /// <summary>
        /// Идентификатор родительского пользователя
        /// </summary>
        public Guid ParentId { get; set; }
    }
}
