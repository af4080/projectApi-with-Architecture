using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using projectApiAngular.Configurations;
using projectApiAngular.DTO;
using projectApiAngular.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace projectApiAngular.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(int userId, string email, string username, string phone, Role role)
        {
            var now = DateTime.Now;
            var expires = now.AddMinutes(_jwtSettings.ExpiryMinutes);
            Console.WriteLine($"Now: {now}");
            Console.WriteLine($"Expires: {expires}");
            Console.WriteLine($"ExpiryMinutes value: {_jwtSettings.ExpiryMinutes}");
            var claims = new[]
            {
                new Claim("name",username ),
                new Claim("role",Role.GetName(typeof(Role),role)!),
                new Claim("email" ,email ),
                new Claim("Phone",phone),
                new Claim("id", userId.ToString())



            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               issuer: _jwtSettings.Issuer,
               audience: _jwtSettings.Audience,
               claims: claims,
               expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
               signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
