using FluentValidation;

namespace RegistrationService.Infrastructure.Commands.Registrations.Create;

public class CreateRegistrationCommandValidator : AbstractValidator<CreateRegistrationCommand>
{
    public CreateRegistrationCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotNull();
        RuleFor(x => x.CategoryId).NotEqual(Guid.Empty);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}