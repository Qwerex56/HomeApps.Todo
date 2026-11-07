using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Tasks.UpdateTask;

internal sealed class UpdateTaskHandler(ITodoListRepository lists, ITodoTaskRepository tasks, Actors actors, TimeProvider time)
    : ICommandHandler<UpdateTaskCommand>
{
    public async Task<Result> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await tasks.FindVisibleAsync(lists, command.HouseholdId, command.TaskId, actor, cancellationToken);
        if (found.IsFailed)
        {
            return found.ToResult();
        }

        var (task, list) = found.Value;
        var input = command.Task;
        var edited = task.Edit(list, actor, input.Title, input.Notes, input.Due, input.Priority, time);
        return edited.IsFailed ? edited : task.Assign(list, actor, input.AssigneeId, time);
    }
}
