using FluentValidation;
using System.ComponentModel.DataAnnotations;
using UserService.Infrastructure.Models;

namespace Tempus.Infrastructure.Services.AuthService;

public class RegisterUserValidation : AbstractValidator<LoginCredentials>
{
    public RegisterUserValidation()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().Must(ValidEmail);
        RuleFor(x => x.ExternalId).NotEmpty();
    }

    private bool ValidEmail(string email)
    {
        return new EmailAddressAttribute().IsValid(email);
    }
}