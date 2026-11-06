using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Lists;

/// <summary>
/// A list of tasks in one household (plan 4.2). Shared lists belong to the household: members add and tick off tasks,
/// admins rename, archive and delete the list. A private list is its owner's alone. Tasks are a separate aggregate
/// (<see cref="Tasks.TodoTask"/>) and ask the list who may change them.
/// </summary>
public sealed class TodoList
{
    /// <summary>The name of the shared list every household gets when the plugin is switched on for the first time.</summary>
    public const string DefaultName = "Dom";

    /// <summary>T5: how long the private lists of someone who left are kept, hidden, in case they come back.</summary>
    public static readonly TimeSpan KeptAfterOwnerLeft = TimeSpan.FromDays(30);

    private TodoList() { }

    private TodoList(Guid id, Guid householdId, ListName name, ListVisibility visibility, Guid ownerId, DateTimeOffset now)
    {
        Id = id;
        HouseholdId = householdId;
        Name = name;
        Visibility = visibility;
        OwnerId = ownerId;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }

    public Guid HouseholdId { get; private set; }

    public ListName Name { get; private set; } = null!;

    public ListVisibility Visibility { get; private set; }

    /// <summary>Who created the list; for a private list the only person who sees it.</summary>
    public Guid OwnerId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? ArchivedAt { get; private set; }

    /// <summary>T5: set on a private list while its owner is out of the household; the list is hidden meanwhile.</summary>
    public DateTimeOffset? OwnerLeftAt { get; private set; }

    public bool IsArchived => ArchivedAt is not null;

    public bool IsShared => Visibility == ListVisibility.Shared;

    /// <summary>The shared list "Dom", created when the plugin is switched on for the first time.</summary>
    public static TodoList CreateDefault(Guid householdId, Guid switchedOnBy, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var now = time.GetUtcNow();
        return new TodoList(Guid.CreateVersion7(now), householdId, ListName.Create(DefaultName).Value, ListVisibility.Shared,
            switchedOnBy, now);
    }

    /// <summary>A shared list needs an admin; any member may keep private lists.</summary>
    public static Result<TodoList> Create(Guid householdId, Actor actor, string? name, ListVisibility visibility, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentNullException.ThrowIfNull(time);
        if (householdId == Guid.Empty)
        {
            throw new ArgumentException("The household id cannot be empty.", nameof(householdId));
        }

        if (visibility == ListVisibility.Shared && !actor.Manages)
        {
            return Result.Fail<TodoList>(TodoErrors.ListForbidden);
        }

        var validName = ListName.Create(name);
        if (validName.IsFailed)
        {
            return validName.ToResult<TodoList>();
        }

        var now = time.GetUtcNow();
        return new TodoList(Guid.CreateVersion7(now), householdId, validName.Value, visibility, actor.UserId, now);
    }

    /// <summary>Shared lists for everyone, a private list only for its owner and not while they are out of the household.</summary>
    public bool CanSee(Actor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);
        return IsShared || (actor.UserId == OwnerId && OwnerLeftAt is null);
    }

    /// <summary>Who may add, edit, tick off and move tasks here: anyone who sees the list, while it is not archived.</summary>
    public Result CanChangeTasks(Actor actor)
    {
        if (!CanSee(actor))
        {
            return Result.Fail(TodoErrors.ListNotFound);
        }

        return IsArchived ? Result.Fail(TodoErrors.ListArchived) : Result.Ok();
    }

    /// <summary>Who may rename, archive and delete the list itself: admins for a shared list, the owner for a private one.</summary>
    public Result CanManage(Actor actor)
    {
        if (!CanSee(actor))
        {
            return Result.Fail(TodoErrors.ListNotFound);
        }

        return IsShared && !actor.Manages ? Result.Fail(TodoErrors.ListForbidden) : Result.Ok();
    }

    public Result Rename(Actor actor, string? name, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = CanManage(actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        if (IsArchived)
        {
            return Result.Fail(TodoErrors.ListArchived);
        }

        var validName = ListName.Create(name);
        if (validName.IsFailed)
        {
            return validName.ToResult();
        }

        Name = validName.Value;
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    /// <summary>An archived list is read-only and kept with its tasks until it is restored or deleted.</summary>
    public Result Archive(Actor actor, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = CanManage(actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        if (IsArchived)
        {
            return Result.Fail(TodoErrors.ListArchived);
        }

        ArchivedAt = UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    public Result Restore(Actor actor, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = CanManage(actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        if (!IsArchived)
        {
            return Result.Fail(TodoErrors.ListNotArchived);
        }

        ArchivedAt = null;
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    /// <summary>A hard delete (it is not account data); the list's tasks go with it. Only checks; the handler removes.</summary>
    public Result CanDelete(Actor actor) => CanManage(actor);

    /// <summary>T5: the owner left the household (<c>MemberLeft</c>). Only private lists are affected.</summary>
    public void OwnerLeft(TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        if (!IsShared && OwnerLeftAt is null)
        {
            OwnerLeftAt = UpdatedAt = time.GetUtcNow();
        }
    }

    /// <summary>T5: the owner joined again within <see cref="KeptAfterOwnerLeft"/> (<c>MemberJoined</c>): the list is back.</summary>
    public void OwnerReturned(TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        if (OwnerLeftAt is not null)
        {
            OwnerLeftAt = null;
            UpdatedAt = time.GetUtcNow();
        }
    }

    /// <summary>T5: a private list whose owner has been gone for <see cref="KeptAfterOwnerLeft"/>; a job deletes it.</summary>
    public bool IsDueForPurge(DateTimeOffset now) => OwnerLeftAt is { } left && now - left >= KeptAfterOwnerLeft;
}
