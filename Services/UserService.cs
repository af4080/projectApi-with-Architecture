using projectApiAngular.Models;
using projectApiAngular.Repositories;
using static projectApiAngular.DTO.UserDto;



namespace projectApiAngular.Services
{  
    using BCrypt.Net;
    using Microsoft.OpenApi.Extensions;

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserService> _logger;
        
        public UserService(IUserRepository userRepository, ITokenService tokenService,ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }
        //map user
        private static ReadUserDto MapUser(User u)
        {
            return new ReadUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                Role = Role.user.GetDisplayName()
            };
        }

        //register user
        public async Task<ReadUserDto> RegisterUser(CreateUserDto user)
        {
            if (await _userRepository.GetUserByEmail(user.Email) != null)
            {
               _logger.LogError("Attempt to register with existing email: {Email}", user.Email);
               throw new Exception("User with this email already exists.");
            }
              
            try
            {
                var NewUser = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Password = BCrypt.HashPassword(user.Password),
                    Role = Role.user
                };
                var created = await _userRepository.RegisterUser(NewUser);
                _logger.LogInformation("New user registered with email: {Email}", user.Email);
                return MapUser(created);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user with email: {Email}", user.Email);
                throw new Exception(ex.Message);
            }

        }
        //login user
        public async Task<string> LoginUser(string email, string password)
        {
            _logger.LogInformation("User login attempt with email: {Email}", email);
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null || !BCrypt.Verify(password, user.Password))
            {
                throw new Exception("invalid email or password.");

            }
            var token = _tokenService.GenerateToken(user.Id, user.Email, user.Name, user.Phone, user.Role);
            _logger.LogInformation("User logged in successfully with email: {Email}", email);

            return token;
        }
    }
}
