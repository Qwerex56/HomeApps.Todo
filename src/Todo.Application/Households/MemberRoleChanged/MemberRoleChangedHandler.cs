using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Contracts.Households;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.MemberRoleChanged;

/// <summary>The new role comes in the event; the plugin keeps only whether the owner of a feed link is a guest.</summary>
internal sealed class MemberRoleChangedHandler(ICalendarSubscriptionRepository subscriptions) : ICommandHandler<MemberRoleChangedCommand>
{
    public async Task<Result> Handle(MemberRoleChangedCommand command, CancellationToken cancellationToken)
    {
        var guest = !Enum.TryParse<HouseholdRole>(command.Role, out var role) || role < HouseholdRole.Member;
        foreach (var subscription in await subscriptions.ListAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            subscription.MarkGuest(guest);
        }

        return Result.Ok();
    }
}
