using System.ComponentModel.DataAnnotations;

namespace Masofa.Common.ViewModels.Account
{
    /// <summary>
    /// ВьюМодель для запроса авторизации
    /// </summary>
    public class LoginAndPasswordViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
