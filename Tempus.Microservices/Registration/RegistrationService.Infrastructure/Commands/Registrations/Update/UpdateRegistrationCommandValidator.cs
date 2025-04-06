using FluentValidation;

namespace RegistrationService.Infrastructure.Commands.Registrations.Update;

public class UpdateRegistrationCommandValidator : AbstractValidator<UpdateRegistrationCommand>
{
    public UpdateRegistrationCommandValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}