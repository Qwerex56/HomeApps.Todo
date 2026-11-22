using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.MemberJoined;

internal sealed class MemberJoinedHandler(ITodoListRepository lists, IInactiveMarkerRepository markers, IExternalCalendarRepository calendars, TimeProvider time) : ICommandHandler<MemberJoinedCommand>
{
    public async Task<Result> Handle(MemberJoinedCommand command, CancellationToken cancellationToken)
    {
        await markers.ClearAsync(command.HouseholdId, command.UserId, cancellationToken);

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
