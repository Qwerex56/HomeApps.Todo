using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Tasks;

namespace Todo.Application.Tasks.CreateTask;

internal sealed class CreateTaskHandler(ITodoListRepository lists, ITodoTaskRepository tasks, Actors actors, TimeProvider time)
    : ICommandHandler<CreateTaskCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        if (list.IsFailed)
        {
            return list.ToResult<Guid>();
        }

        var position = (await tasks.LastPositionAsync(command.ListId, null, cancellationToken) ?? 0m) + 1m;
        var input = command.Task;
        var task = TodoTask.Create(list.Value, actor, input.Title, input.Notes, input.Due, input.Priority, input.AssigneeId, position, time);
        if (task.IsFailed)
        {
            return task.ToResult<Guid>();
        }

        tasks.Add(task.Value);
        return task.Value.Id;
    }
}
