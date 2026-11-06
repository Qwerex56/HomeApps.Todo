using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Tasks.ReopenTask;

internal sealed class ReopenTaskHandler(ITodoListRepository lists, ITodoTaskRepository tasks, Actors actors, TimeProvider time)
    : ICommandHandler<ReopenTaskCommand>
{
    public async Task<Result> Handle(ReopenTaskCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await tasks.FindVisibleAsync(lists, command.HouseholdId, command.TaskId, actor, cancellationToken);
        return found.IsFailed ? found.ToResult() : found.Value.Task.Reopen(found.Value.List, actor, time);
    }
}
