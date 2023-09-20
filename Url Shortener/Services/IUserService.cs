using Url_Shortener.Entities;

namespace Url_Shortener.Services
{
    public interface IUserService
    {
        Task<bool> Register(string username , string password);

    }
}
