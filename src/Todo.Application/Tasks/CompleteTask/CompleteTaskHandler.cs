using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Tasks.CompleteTask;

internal sealed class CompleteTaskHandler(ITodoListRepository lists, ITodoTaskRepository tasks, Actors actors, TimeProvider time)
    : ICommandHandler<CompleteTaskCommand>
{
    public async Task<Result> Handle(CompleteTaskCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await tasks.FindVisibleAsync(lists, command.HouseholdId, command.TaskId, actor, cancellationToken);
        return found.IsFailed ? found.ToResult() : found.Value.Task.Complete(found.Value.List, actor, time);
    }
}
