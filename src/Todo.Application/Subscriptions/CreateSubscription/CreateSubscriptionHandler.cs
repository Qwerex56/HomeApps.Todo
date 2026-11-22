using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Domain.Events;

namespace Todo.Application.Subscriptions.CreateSubscription;

internal sealed class CreateSubscriptionHandler(ICalendarSubscriptionRepository subscriptions, ICurrentUser user, TimeProvider time)
    : ICommandHandler<CreateSubscriptionCommand, CreatedSubscriptionView>
{
    public Task<Result<CreatedSubscriptionView>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var token = FeedToken.Generate();
        var subscription = new CalendarSubscription(
            command.HouseholdId, user.RequiredUserId, FeedToken.Hash(token), command.Label, command.IncludeHouseholdEvents,
            command.IncludePrivateEvents, command.TasksMode, time.GetUtcNow());
        subscriptions.Add(subscription);

        return Task.FromResult(Result.Ok(new CreatedSubscriptionView(
            CalendarSubscriptionView.Of(subscription), token, $"/plugins/todo/feed/{token}.ics")));
    }
}
