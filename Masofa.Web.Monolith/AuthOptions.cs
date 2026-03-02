using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Masofa.Common.Models.SystemCrical
{
    public class AuthOptions
    {
        public string ISSUER { get; set; } = "MyAuthServer"; // издатель токена
        public string AUDIENCE { get; set; } = "MyAuthClient"; // потребитель токена
        public string KEY { get; set; } = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
