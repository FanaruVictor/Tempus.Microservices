using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace UserService.Infrastructure.Commands.Users.Create
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().Must(ValidEmail);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }

        private bool ValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }
    }
}
