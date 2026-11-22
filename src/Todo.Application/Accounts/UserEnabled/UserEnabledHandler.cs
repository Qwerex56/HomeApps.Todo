using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Accounts.UserEnabled;

/// <summary>The account is back: the same feed links work again.</summary>
internal sealed class UserEnabledHandler(ICalendarSubscriptionRepository subscriptions, IInactiveMarkerRepository markers)
    : ICommandHandler<UserEnabledCommand>
{
    public async Task<Result> Handle(UserEnabledCommand command, CancellationToken cancellationToken)
    {
        foreach (var subscription in await subscriptions.ListOfUserAsync(command.UserId, cancellationToken))
        {
            subscription.Resume();
        }

        await markers.ClearAsync(null, command.UserId, cancellationToken);

        return Result.Ok();
    }
}
