using FluentValidation;

namespace Todo.Application.Lists.RenameList;

internal sealed class RenameListValidator : AbstractValidator<RenameListCommand>
{
    public RenameListValidator() => RuleFor(c => c.Name).NotNull();
}
