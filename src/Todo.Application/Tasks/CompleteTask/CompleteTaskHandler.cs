using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Tasks;

namespace Todo.Application.Tasks.CompleteTask;

internal sealed class CompleteTaskHandler(ITodoListRepository lists, ITodoTaskRepository tasks, Actors actors, TimeProvider time)
    : ICommandHandler<CompleteTaskCommand>
{
    public async Task<Result> Handle(CompleteTaskCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await tasks.FindVisibleAsync(lists, command.HouseholdId, command.TaskId, actor, cancellationToken);
        if (found.IsFailed)
        {
            return found.ToResult();
        }

        // A repeated tick (two people at the wall) changes nothing, so it must not add another instance.
        var wasOpen = found.Value.Task.Status == TodoStatus.Open;
        var done = found.Value.Task.Complete(found.Value.List, actor, time);
        if (done.IsSuccess && wasOpen && found.Value.Task.NextInstance(time) is { } next
            && !await tasks.HasOpenInSeriesAsync(command.HouseholdId, next.SeriesId!.Value, cancellationToken))
        {
            tasks.Add(next);
        }

        return done;
    }
}
