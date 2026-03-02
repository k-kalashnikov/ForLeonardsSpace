using System.ComponentModel.DataAnnotations;

namespace Masofa.Common.ViewModels.Account
{
    /// <summary>
    /// Вью Модель для смены пароля
    /// </summary>
    public class ChangePasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match.")]
        public string ConfirmNewPassword { get; set; }

    }
}
