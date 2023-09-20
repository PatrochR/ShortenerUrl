using Url_Shortener.Entities;

namespace Url_Shortener.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _context;

        public UserService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<bool> Register(string username , string password)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                PasswordHash = passwordHash,
                UserName = username,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
