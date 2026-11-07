using FluentValidation;

namespace Todo.Application.Tasks.UpdateTask;

internal sealed class UpdateTaskValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskValidator() => RuleFor(c => c.Task).NotNull().SetValidator(new TaskInputValidator());
}
