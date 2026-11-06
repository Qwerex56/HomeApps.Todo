using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Common;

namespace Todo.Application.Households;

// Sent by the consumers of Households' events, without a user: no permission check, the transaction behavior saves.

/// <summary>
/// Plan 4.2 and T5: someone left a household. Their assignments go; their private lists there are hidden and
/// deleted after <see cref="Domain.Lists.TodoList.KeptAfterOwnerLeft"/> unless they come back.
/// </summary>
public sealed record MemberLeftCommand(Guid HouseholdId, Guid UserId) : ICommand;

internal sealed class MemberLeftHandler(ITodoListRepository lists, ITodoTaskRepository tasks, TimeProvider time) : ICommandHandler<MemberLeftCommand>
{
    public async Task<Result> Handle(MemberLeftCommand command, CancellationToken cancellationToken)
    {
        foreach (var task in await tasks.ListAssignedToAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            task.AssigneeLeft(command.UserId, time);
        }

        foreach (var list in await lists.ListPrivateOfAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            list.OwnerLeft(time);
        }

        return Result.Ok();
    }
}

/// <summary>T5: someone joined (again); private lists they left behind here come back.</summary>
public sealed record MemberJoinedCommand(Guid HouseholdId, Guid UserId) : ICommand;

internal sealed class MemberJoinedHandler(ITodoListRepository lists, TimeProvider time) : ICommandHandler<MemberJoinedCommand>
{
    public async Task<Result> Handle(MemberJoinedCommand command, CancellationToken cancellationToken)
    {
        foreach (var list in await lists.ListPrivateOfAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            list.OwnerReturned(time);
        }

        return Result.Ok();
    }
}

/// <summary>T5: deletes private lists (with their tasks) whose owner has been gone for 30 days. Returns how many.</summary>
public sealed record PurgeLeftListsCommand : ICommand<int>;

internal sealed class PurgeLeftListsHandler(ITodoListRepository lists, TimeProvider time) : ICommandHandler<PurgeLeftListsCommand, int>
{
    public async Task<Result<int>> Handle(PurgeLeftListsCommand command, CancellationToken cancellationToken)
    {
        var due = await lists.ListDueForPurgeAsync(time.GetUtcNow(), cancellationToken);
        foreach (var list in due)
        {
            lists.Remove(list);
        }

        return due.Count;
    }
}
