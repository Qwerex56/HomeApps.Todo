using FluentResults;
using HomeApps.BuildingBlocks.Domain;
using Todo.Domain.Common;
using Todo.Domain.Lists;

namespace Todo.Domain.Tasks;

/// <summary>
/// One task on a list (plan 4.2). A separate aggregate so long lists never load whole. Every change takes the task's
/// list and asks it whether the actor may change tasks there (<see cref="TodoList.CanChangeTasks"/>): shared lists
/// for everyone with <c>tasks.write</c>, private lists for their owner, archived lists for nobody.
/// </summary>
public sealed class TodoTask : IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    private TodoTask() { }

    private TodoTask(Guid id, TodoList list, TaskTitle title, Guid createdBy, decimal position, DateTimeOffset now)
    {
        Id = id;
        HouseholdId = list.HouseholdId;
        ListId = list.Id;
        Title = title;
        Status = TodoStatus.Open;
        Priority = TaskPriority.Normal;
        Position = position;
        CreatedBy = createdBy;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }

    public Guid HouseholdId { get; private set; }

    public Guid ListId { get; private set; }

    public TaskTitle Title { get; private set; } = null!;

    public TaskNotes? Notes { get; private set; }

    public TaskDue? Due { get; private set; }

    /// <summary>A household member; names come from Households in the GUI (plan 4.2).</summary>
    public Guid? AssigneeId { get; private set; }

    public TaskPriority Priority { get; private set; }

    public TodoStatus Status { get; private set; }

    /// <summary>Manual order inside the list, ascending. A fraction, so a move between two tasks touches only the moved one.</summary>
    public decimal Position { get; private set; }

    public Guid CreatedBy { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Guid? CompletedBy { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    public void ClearDomainEvents() => _domainEvents.Clear();

    public static Result<TodoTask> Create(
        TodoList list,
        Actor actor,
        string? title,
        string? notes,
        TaskDue? due,
        TaskPriority priority,
        Guid? assigneeId,
        decimal position,
        TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentNullException.ThrowIfNull(time);
        var allowed = list.CanChangeTasks(actor);
        if (allowed.IsFailed)
        {
            return allowed.ToResult<TodoTask>();
        }

        var validTitle = TaskTitle.Create(title);
        var validNotes = TaskNotes.Create(notes);
        var check = Result.Merge(validTitle, validNotes, CheckAssignee(list, assigneeId));
        if (check.IsFailed)
        {
            return check.ToResult<TodoTask>();
        }

        var now = time.GetUtcNow();
        return new TodoTask(Guid.CreateVersion7(now), list, validTitle.Value, actor.UserId, position, now)
        {
            Notes = validNotes.Value,
            Due = due,
            Priority = priority,
            AssigneeId = assigneeId,
        };
    }

    /// <summary>Changes what the task says and when it is due. Assignment, status and place have their own methods.</summary>
    public Result Edit(TodoList list, Actor actor, string? title, string? notes, TaskDue? due, TaskPriority priority, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = Allowed(list, actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        var validTitle = TaskTitle.Create(title);
        var validNotes = TaskNotes.Create(notes);
        var check = Result.Merge(validTitle, validNotes);
        if (check.IsFailed)
        {
            return check;
        }

        Title = validTitle.Value;
        Notes = validNotes.Value;
        Due = due;
        Priority = priority;
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    /// <summary>
    /// Assigns the task, or clears the assignment with null. On a shared list the Application layer checks that the
    /// person is a household member; on a private list only its owner can be assigned.
    /// </summary>
    public Result Assign(TodoList list, Actor actor, Guid? assigneeId, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = Result.Merge(Allowed(list, actor), CheckAssignee(list, assigneeId));
        if (allowed.IsFailed)
        {
            return allowed;
        }

        if (AssigneeId != assigneeId)
        {
            AssigneeId = assigneeId;
            UpdatedAt = time.GetUtcNow();
        }

        return Result.Ok();
    }

    /// <summary>Ticks the task off. Idempotent: a second tap (two people at the wall) changes nothing and raises nothing.</summary>
    public Result Complete(TodoList list, Actor actor, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = Allowed(list, actor);
        if (allowed.IsFailed || Status == TodoStatus.Done)
        {
            return allowed;
        }

        var now = time.GetUtcNow();
        Status = TodoStatus.Done;
        CompletedBy = actor.UserId;
        CompletedAt = UpdatedAt = now;
        _domainEvents.Add(new TaskCompleted(Id, HouseholdId, ListId, list.IsShared, Title.Value, actor.UserId, now));
        return Result.Ok();
    }

    /// <summary>Opens a done task again. Idempotent like <see cref="Complete"/>.</summary>
    public Result Reopen(TodoList list, Actor actor, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = Allowed(list, actor);
        if (allowed.IsFailed || Status == TodoStatus.Open)
        {
            return allowed;
        }

        Status = TodoStatus.Open;
        CompletedBy = null;
        CompletedAt = null;
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    /// <summary>
    /// Moves the task to <paramref name="position"/> on <paramref name="target"/> (its own list or another list of the
    /// household). The actor must be allowed to change tasks on both lists. Moving onto a private list drops an
    /// assignment to anyone but its owner.
    /// </summary>
    public Result Move(TodoList current, TodoList target, Actor actor, decimal position, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(time);
        var allowed = Allowed(current, actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        if (target.HouseholdId != HouseholdId)
        {
            return Result.Fail(TodoErrors.ListNotFound);
        }

        var check = target.CanChangeTasks(actor);
        if (check.IsFailed)
        {
            return check;
        }

        ListId = target.Id;
        Position = position;
        if (!target.IsShared && AssigneeId is { } assignee && assignee != target.OwnerId)
        {
            AssigneeId = null;
        }

        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    /// <summary>
    /// A hard delete. On a shared list only the task's author or an admin; on a private list its owner. Only checks;
    /// the handler removes.
    /// </summary>
    public Result CanDelete(TodoList list, Actor actor)
    {
        var allowed = Allowed(list, actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        return list.IsShared && CreatedBy != actor.UserId && !actor.Manages
            ? Result.Fail(TodoErrors.TaskForbidden)
            : Result.Ok();
    }

    /// <summary>Plan 4.2: someone left the household (<c>MemberLeft</c>); their assignments on shared lists go.</summary>
    public void AssigneeLeft(Guid userId, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        if (AssigneeId == userId)
        {
            AssigneeId = null;
            UpdatedAt = time.GetUtcNow();
        }
    }

    /// <summary>Open and due on <paramref name="today"/> or before: what "Na dziś" and the display show.</summary>
    public bool IsDueBy(DateOnly today) => Status == TodoStatus.Open && Due is not null && Due.IsDueBy(today);

    private Result Allowed(TodoList list, Actor actor)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(actor);
        if (list.Id != ListId)
        {
            throw new ArgumentException("The list is not the task's list.", nameof(list));
        }

        return list.CanChangeTasks(actor);
    }

    private static Result CheckAssignee(TodoList list, Guid? assigneeId) =>
        assigneeId == Guid.Empty || (!list.IsShared && assigneeId is { } id && id != list.OwnerId)
            ? Result.Fail(TodoErrors.AssigneeInvalid)
            : Result.Ok();
}
