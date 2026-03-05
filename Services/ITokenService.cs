using projectApiAngular.Models;

namespace projectApiAngular.Services
{
    public interface ITokenService
    {
       public string GenerateToken(int userId, string email, string username, string phone, Role role);
    }
}