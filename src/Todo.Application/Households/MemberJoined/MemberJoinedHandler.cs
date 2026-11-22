using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Contracts.Households;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.MemberJoined;

internal sealed class MemberJoinedHandler(ITodoListRepository lists, IInactiveMarkerRepository markers, IExternalCalendarRepository calendars, ICalendarSubscriptionRepository subscriptions, TimeProvider time) : ICommandHandler<MemberJoinedCommand>
{
    public async Task<Result> Handle(MemberJoinedCommand command, CancellationToken cancellationToken)
    {
        await markers.ClearAsync(command.HouseholdId, command.UserId, cancellationToken);

        var guest = !Enum.TryParse<HouseholdRole>(command.Role, out var role) || role < HouseholdRole.Member;
        foreach (var subscription in await subscriptions.ListAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            subscription.MarkGuest(guest);
        }

        foreach (var list in await lists.ListPrivateOfAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            list.OwnerReturned(time);
        }

        foreach (var calendar in await calendars.ListAllOfUserAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            calendar.OwnerReturned();
        }

        return Result.Ok();
    }
}
