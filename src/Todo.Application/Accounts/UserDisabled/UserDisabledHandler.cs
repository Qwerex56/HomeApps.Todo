using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Accounts.UserDisabled;

/// <summary>The account is blocked: its feed links stop serving at once and its memberships count as inactive.</summary>
internal sealed class UserDisabledHandler(ICalendarSubscriptionRepository subscriptions, IHouseholdMemberRepository members)
    : ICommandHandler<UserDisabledCommand>
{
    public async Task<Result> Handle(UserDisabledCommand command, CancellationToken cancellationToken)
    {
        foreach (var subscription in await subscriptions.ListOfUserAsync(command.UserId, cancellationToken))
        {
            subscription.Suspend();
        }

        foreach (var member in await members.ListOfUserAsync(command.UserId, cancellationToken))
        {
            member.Disable();
        }

        return Result.Ok();
    }
}
