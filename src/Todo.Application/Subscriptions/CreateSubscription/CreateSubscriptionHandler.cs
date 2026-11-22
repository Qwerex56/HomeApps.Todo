using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Contracts.Households;
using HomeApps.Plugins;
using Todo.Application.Abstractions;
using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Application.Subscriptions.CreateSubscription;

internal sealed class CreateSubscriptionHandler(
    ICalendarSubscriptionRepository subscriptions, IHouseholdAccess access, ICurrentUser user, TimeProvider time)
    : ICommandHandler<CreateSubscriptionCommand, CreatedSubscriptionView>
{
    public async Task<Result<CreatedSubscriptionView>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var membership = await access.GetMembershipAsync(command.HouseholdId, cancellationToken);
        if (membership is null)
        {
            return Result.Fail(PlannerErrors.SubscriptionNotFound);
        }

        var token = FeedToken.Generate();
        var subscription = new CalendarSubscription(
            command.HouseholdId, user.RequiredUserId, FeedToken.Hash(token), command.Label, command.IncludeHouseholdEvents,
            command.IncludePrivateEvents, command.TasksMode, time.GetUtcNow());
        subscription.MarkGuest(membership.Role < HouseholdRole.Member);
        subscriptions.Add(subscription);

        return Result.Ok(new CreatedSubscriptionView(
            CalendarSubscriptionView.Of(subscription), token, $"/plugins/todo/feed/{token}.ics"));
    }
}
