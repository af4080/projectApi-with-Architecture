using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public interface IUserRepository
    {
        Task<User> RegisterUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);

    }
}