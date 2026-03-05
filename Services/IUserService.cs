using projectApiAngular.DTO;

namespace projectApiAngular.Services
{
    public interface IUserService
    {
        Task<string> LoginUser(string email, string password);
        Task<UserDto.ReadUserDto> RegisterUser(UserDto.CreateUserDto user);
    }
}