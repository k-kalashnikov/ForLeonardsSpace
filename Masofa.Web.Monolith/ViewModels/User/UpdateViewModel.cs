using System.ComponentModel.DataAnnotations;

namespace Masofa.Web.Monolith.ViewModels.User
{
    public class UpdateViewModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Второе имя пользователя
        /// </summary>
        public string? SecondName { get; set; }

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Старый пароль пользователя
        /// </summary>
        public string? OldPassword { get; set; }

        /// <summary>
        /// Новый пароль пользователя
        /// </summary>
        public string? NewPassword { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Compare(nameof(NewPassword),  ErrorMessage = "Passwords don't match.")]
        public string? ConfirmPassword { get; set; }

        /// <summary>
        /// Электронная почта пользователя
        /// </summary>
        public string? Email { get; set; }

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
        /// Роль в которую добавить пользователя при обновлении
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
