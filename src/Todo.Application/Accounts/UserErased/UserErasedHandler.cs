using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Accounts.UserErased;

/// <summary>The account is erased: its feed links are deleted for good. (Private events and imports follow in later stages.)</summary>
internal sealed class UserErasedHandler(ICalendarSubscriptionRepository subscriptions) : ICommandHandler<UserErasedCommand>
{
    public async Task<Result> Handle(UserErasedCommand command, CancellationToken cancellationToken)
    {
        foreach (var subscription in await subscriptions.ListOfUserAsync(command.UserId, cancellationToken))
        {
            subscriptions.Remove(subscription);
        }

        return Result.Ok();
    }
}
