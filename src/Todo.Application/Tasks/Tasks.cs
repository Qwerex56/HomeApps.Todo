using FluentResults;
using FluentValidation;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Application.Common;
using Todo.Domain.Common;
using Todo.Domain.Tasks;

namespace Todo.Application.Tasks;

/// <summary>
/// Tasks on the lists the caller may see. <c>AssignedToMe</c> and <c>DueBy</c> make "moje" and "na dziś"; the day
/// comes from the client because only it knows the household's "today".
/// </summary>
public sealed record GetTasksQuery(Guid HouseholdId, Guid? ListId, bool AssignedToMe, DateOnly? DueBy, TodoStatus? Status)
    : IQuery<IReadOnlyList<TaskView>>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}

internal sealed class GetTasksValidator : AbstractValidator<GetTasksQuery>
{
    public GetTasksValidator() => RuleFor(q => q.Status).IsInEnum();
}

internal sealed class GetTasksHandler(ITodoReader reader, ITodoListRepository lists, Actors actors, ICurrentUser user)
    : IQueryHandler<GetTasksQuery, IReadOnlyList<TaskView>>
{
    public async Task<Result<IReadOnlyList<TaskView>>> Handle(GetTasksQuery query, CancellationToken cancellationToken)
    {
        if (query.ListId is { } listId)
        {
            var actor = await actors.GetAsync(query.HouseholdId, cancellationToken);
            var list = await lists.FindVisibleAsync(query.HouseholdId, listId, actor, cancellationToken);
            if (list.IsFailed)
            {
                return list.ToResult<IReadOnlyList<TaskView>>();
            }
        }

        var filter = new TaskFilter(query.ListId, query.AssignedToMe ? user.RequiredUserId : null, query.DueBy, query.Status);
        return Result.Ok(await reader.TasksAsync(query.HouseholdId, user.RequiredUserId, filter, cancellationToken));
    }
}

/// <summary>The fields a person edits on a task. A time needs a day.</summary>
public sealed record TaskInput(string Title, string? Notes, DateOnly? DueDate, TimeOnly? DueTime, TaskPriority Priority, Guid? AssigneeId)
{
    public TaskDue? Due => DueDate is { } date ? new TaskDue(date, DueTime) : null;
}

internal sealed class TaskInputValidator : AbstractValidator<TaskInput>
{
    public TaskInputValidator()
    {
        RuleFor(t => t.Title).NotNull();
        RuleFor(t => t.Priority).IsInEnum();
        RuleFor(t => t.DueTime).Null().When(t => t.DueDate is null).WithMessage("A due time needs a due date.");
    }
}

/// <summary>
/// Adds a task at the end of a list. Who can be assigned on a shared list is not checked against Households: the web
/// app offers only members, and a stale assignment is cleared when the person leaves (<c>MemberLeft</c>).
/// </summary>
public sealed record CreateTaskCommand(Guid HouseholdId, Guid ListId, TaskInput Task) : ICommand<Guid>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

internal sealed class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator() => RuleFor(c => c.Task).NotNull().SetValidator(new TaskInputValidator());
}

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

/// <summary>The whole form: title, notes, due, priority and assignee.</summary>
public sealed record UpdateTaskCommand(Guid HouseholdId, Guid TaskId, TaskInput Task) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

internal sealed class UpdateTaskValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskValidator() => RuleFor(c => c.Task).NotNull().SetValidator(new TaskInputValidator());
}

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

public sealed record CompleteTaskCommand(Guid HouseholdId, Guid TaskId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

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

public sealed record ReopenTaskCommand(Guid HouseholdId, Guid TaskId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

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

/// <summary>
/// Drag and drop: puts the task on <c>ListId</c> right before <c>BeforeTaskId</c>, or at the end when it is null.
/// Only the moved task changes: its position becomes the midpoint between its new neighbours.
/// </summary>
public sealed record MoveTaskCommand(Guid HouseholdId, Guid TaskId, Guid ListId, Guid? BeforeTaskId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

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

/// <summary>A hard delete: on a shared list by its author or an admin, on a private list by its owner.</summary>
public sealed record DeleteTaskCommand(Guid HouseholdId, Guid TaskId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

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
