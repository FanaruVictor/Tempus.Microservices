using FluentValidation;

namespace GroupService.Infrastructure.Commands.Groups.Create;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}