using FluentValidation;
using Url_Shortener.Models;

namespace Url_Shortener.Validation
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(l => l.Username).NotEmpty().MinimumLength(3);
            RuleFor(l => l.Password).NotEmpty().MinimumLength(8);
        }
    }
}
