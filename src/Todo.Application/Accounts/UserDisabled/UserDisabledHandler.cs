using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Accounts.UserDisabled;

/// <summary>The account is blocked: its feed links stop serving at once, and reminders for it stop.</summary>
internal sealed class UserDisabledHandler(ICalendarSubscriptionRepository subscriptions, IInactiveMarkerRepository markers)
    : ICommandHandler<UserDisabledCommand>
{
    public async Task<Result> Handle(UserDisabledCommand command, CancellationToken cancellationToken)
    {
        foreach (var subscription in await subscriptions.ListOfUserAsync(command.UserId, cancellationToken))
        {
            subscription.Suspend();
        }

        await markers.MarkAsync(null, command.UserId, cancellationToken);

        return Result.Ok();
    }
}
