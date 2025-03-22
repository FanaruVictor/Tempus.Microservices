using FluentValidation;

namespace GroupService.Infrastructure.Commands.Groups.Create;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Members).NotEqual("\"\"").NotEmpty().WithMessage("Members cannot be empty. Please select at least one member.");
        RuleFor(x => x.Name).NotEmpty();
    }
}