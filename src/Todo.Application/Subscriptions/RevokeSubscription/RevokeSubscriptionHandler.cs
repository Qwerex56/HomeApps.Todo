using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Domain.Common;

namespace Todo.Application.Subscriptions.RevokeSubscription;

internal sealed class RevokeSubscriptionHandler(ICalendarSubscriptionRepository subscriptions, ICurrentUser user) : ICommandHandler<RevokeSubscriptionCommand>
{
    public async Task<Result> Handle(RevokeSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var found = await subscriptions.GetAsync(command.HouseholdId, user.RequiredUserId, command.Id, cancellationToken);
        if (found is null)
        {
            return Result.Fail(PlannerErrors.SubscriptionNotFound);
        }

        subscriptions.Remove(found);
        return Result.Ok();
    }
}
