using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Tasks.DeleteTask;

internal sealed class DeleteTaskHandler(ITodoListRepository lists, ITodoTaskRepository tasks, Actors actors) : ICommandHandler<DeleteTaskCommand>
{
    public async Task<Result> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await tasks.FindVisibleAsync(lists, command.HouseholdId, command.TaskId, actor, cancellationToken);
        if (found.IsFailed)
        {
            return found.ToResult();
        }

        var allowed = found.Value.Task.CanDelete(found.Value.List, actor);
        if (allowed.IsSuccess)
        {
            tasks.Remove(found.Value.Task);
        }

        return allowed;
    }
}
