using FluentValidation;

namespace Todo.Application.Tasks;

internal sealed class TaskInputValidator : AbstractValidator<TaskInput>
{
    public TaskInputValidator()
    {
        RuleFor(t => t.Title).NotNull();
        RuleFor(t => t.Priority).IsInEnum();
        RuleFor(t => t.DueTime).Null().When(t => t.DueDate is null).WithMessage("A due time needs a due date.");
    }
}
