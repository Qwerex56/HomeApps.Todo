using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Application.Events;
using Todo.Domain.Tasks;

namespace Todo.Application.Tasks.DeleteTask;

internal sealed class DeleteTaskHandler(ITodoListRepository lists, ITodoTaskRepository tasks, Actors actors, TimeProvider time) : ICommandHandler<DeleteTaskCommand>
{
    public async Task<Result> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await tasks.FindVisibleAsync(lists, command.HouseholdId, command.TaskId, actor, cancellationToken);
        if (found.IsFailed)
        {
            return found.ToResult();
        }

        var scope = command.Scope is null ? Result.Ok(RecurrenceScope.All) : ScopeParser.Parse(command.Scope);
        if (scope.IsFailed)
        {
            return scope.ToResult();
        }

        var task = found.Value.Task;
        var allowed = task.CanDelete(found.Value.List, actor);
        if (allowed.IsSuccess)
        {
            if (scope.Value == RecurrenceScope.ThisInstance && task.Skip(time) is { } next)
            {
                tasks.Add(next);
            }

            tasks.Remove(task);
        }

        return allowed;
    }
}
