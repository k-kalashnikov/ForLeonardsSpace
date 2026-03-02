using System.ComponentModel.DataAnnotations;

namespace Masofa.Common.ViewModels.Account
{
    public class LoginByOneIdCode
    {
        [Required]
        public required string Code { get; set; }
    }
}
