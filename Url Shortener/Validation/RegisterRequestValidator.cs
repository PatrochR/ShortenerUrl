using FluentValidation;
using Url_Shortener.Models;

namespace Url_Shortener.Validation
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(r=>r.Username).NotEmpty().MinimumLength(3);
            RuleFor(r=>r.Password).NotEmpty().MinimumLength(8);
        }
    }
}
