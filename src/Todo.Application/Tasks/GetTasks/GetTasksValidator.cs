using FluentValidation;

namespace Todo.Application.Tasks.GetTasks;

internal sealed class GetTasksValidator : AbstractValidator<GetTasksQuery>
{
    public GetTasksValidator() => RuleFor(q => q.Status).IsInEnum();
}
