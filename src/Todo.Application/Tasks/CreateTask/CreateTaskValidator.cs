using FluentValidation;

namespace Todo.Application.Tasks.CreateTask;

internal sealed class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator() => RuleFor(c => c.Task).NotNull().SetValidator(new TaskInputValidator());
}
