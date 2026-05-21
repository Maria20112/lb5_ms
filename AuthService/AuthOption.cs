using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService
{
    public class AuthOption
    {
        public string ISSUER { get; set; }
        public string AUDIENCE { get; set; }
        public static string KEY { get; set; }
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
