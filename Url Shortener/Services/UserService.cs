using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public async Task<User?> FindUser(string username) => await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

        public bool Login(User user, out string token)
        {
            var claims = new List<Claim> { 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name , user.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("IF I LOSE IT ALL . SLIP AND FALL I NEVEBR LOOK AWAY IF I LOSE IT ALL LOSE IT ALL LOSE IT ALL IF I LOSE IT ALL OUTSIDE THE WALL "));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var jwttoken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(jwttoken);
            token = jwt;
            return true;
        }

        public async Task<bool> Register(string username, string password)
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
