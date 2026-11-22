using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Application.Events;
using Todo.Domain.Tasks;

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
        Result edited;
        if (command.Scope is null && !task.IsRecurring && string.IsNullOrWhiteSpace(input.Rrule))
        {
            edited = task.Edit(list, actor, input.Title, input.Notes, input.Due, input.Priority, time);
        }
        else
        {
            var scope = command.Scope is null ? Result.Ok(RecurrenceScope.All) : ScopeParser.Parse(command.Scope);
            if (scope.IsFailed)
            {
                return scope.ToResult();
            }

            var rrule = scope.Value == RecurrenceScope.ThisInstance || string.IsNullOrWhiteSpace(input.Rrule) ? null : input.Rrule;
            var next = scope.Value == RecurrenceScope.ThisInstance ? task.Skip(time) : null;
            edited = task.EditRecurring(
                list, actor, scope.Value, input.Title, input.Notes, input.Due, input.Priority, rrule,
                input.Anchor ?? task.RecurrenceAnchor ?? RecurrenceAnchor.FromDueDate, time);
            if (edited.IsSuccess && next is not null)
            {
                tasks.Add(next);
            }
        }

        return edited.IsFailed ? edited : task.Assign(list, actor, input.AssigneeId, time);
    }
}
