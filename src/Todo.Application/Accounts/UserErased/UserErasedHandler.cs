using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Accounts.UserErased;

/// <summary>The account is erased: its feed links and imported calendars are deleted for good. (Private events follow in a later stage.)</summary>
internal sealed class UserErasedHandler(ICalendarSubscriptionRepository subscriptions, IExternalCalendarRepository calendars, IInactiveMarkerRepository markers) : ICommandHandler<UserErasedCommand>
{
    public async Task<Result> Handle(UserErasedCommand command, CancellationToken cancellationToken)
    {
        foreach (var subscription in await subscriptions.ListOfUserAsync(command.UserId, cancellationToken))
        {
            subscriptions.Remove(subscription);
        }

        foreach (var calendar in await calendars.ListAllOfUserAsync(null, command.UserId, cancellationToken))
        {
            calendars.Remove(calendar);
        }

        await markers.MarkAsync(null, command.UserId, cancellationToken);
        return Result.Ok();
    }
}
