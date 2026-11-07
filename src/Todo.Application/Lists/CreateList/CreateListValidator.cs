using FluentValidation;

namespace Todo.Application.Lists.CreateList;

internal sealed class CreateListValidator : AbstractValidator<CreateListCommand>
{
    public CreateListValidator()
    {
        RuleFor(c => c.Name).NotNull();
        RuleFor(c => c.Visibility).IsInEnum();
    }
}
