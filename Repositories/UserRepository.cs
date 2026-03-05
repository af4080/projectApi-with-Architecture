using Microsoft.EntityFrameworkCore;
using projectApiAngular.Data;
using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Chinese_SalesDbContext _context;
        public UserRepository(Chinese_SalesDbContext context)
        {
            _context = context;
        }

        //register user
        public async Task<User> RegisterUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while registering the user.", ex);
            }

        }


        //get by email for the validation in service
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

        }
        //get by id
        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
