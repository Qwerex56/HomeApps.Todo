using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Common;
using Todo.Domain.Tasks;

namespace Todo.Application.Tasks.MoveTask;

internal sealed class MoveTaskHandler(ITodoListRepository lists, ITodoTaskRepository tasks, Actors actors, TimeProvider time)
    : ICommandHandler<MoveTaskCommand>
{
    public async Task<Result> Handle(MoveTaskCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await tasks.FindVisibleAsync(lists, command.HouseholdId, command.TaskId, actor, cancellationToken);
        if (found.IsFailed)
        {
            return found.ToResult();
        }

        var (task, current) = found.Value;
        var target = command.ListId == current.Id
            ? Result.Ok(current)
            : await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        if (target.IsFailed)
        {
            return target.ToResult();
        }

        var position = await PositionAsync(task, target.Value.Id, command.BeforeTaskId, cancellationToken);
        return position.IsFailed ? position.ToResult() : task.Move(current, target.Value, actor, position.Value, time);
    }

    private async Task<Result<decimal>> PositionAsync(TodoTask task, Guid listId, Guid? beforeTaskId, CancellationToken cancellationToken)
    {
        if (beforeTaskId is not { } beforeId)
        {
            return (await tasks.LastPositionAsync(listId, task.Id, cancellationToken) ?? 0m) + 1m;
        }

        if (beforeId == task.Id)
        {
            return task.Position;
        }

        var before = await tasks.GetAsync(task.HouseholdId, beforeId, cancellationToken);
        if (before is null || before.ListId != listId)
        {
            return Result.Fail<decimal>(TodoErrors.TaskNotFound);
        }

        var previous = await tasks.PositionBeforeAsync(listId, before.Position, task.Id, cancellationToken);
        return previous is { } p ? (p + before.Position) / 2m : before.Position - 1m;
    }
}
