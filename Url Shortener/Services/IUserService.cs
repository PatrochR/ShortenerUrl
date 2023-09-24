using Url_Shortener.Entities;

namespace Url_Shortener.Services
{
    public interface IUserService
    {
        Task<bool> Register(string username , string password);
        bool Login(User user, out string token);

        Task<User?> FindUser(string username);

    }
}
