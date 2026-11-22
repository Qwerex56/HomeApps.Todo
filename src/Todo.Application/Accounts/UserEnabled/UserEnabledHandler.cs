using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Accounts.UserEnabled;

/// <summary>The account is back: the same feed links work again.</summary>
internal sealed class UserEnabledHandler(ICalendarSubscriptionRepository subscriptions, IHouseholdMemberRepository members)
    : ICommandHandler<UserEnabledCommand>
{
    public async Task<Result> Handle(UserEnabledCommand command, CancellationToken cancellationToken)
    {
        foreach (var subscription in await subscriptions.ListOfUserAsync(command.UserId, cancellationToken))
        {
            subscription.Resume();
        }

        foreach (var member in await members.ListOfUserAsync(command.UserId, cancellationToken))
        {
            member.Enable();
        }

        return Result.Ok();
    }
}
