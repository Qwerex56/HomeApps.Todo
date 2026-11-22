using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Application.Abstractions;
using Todo.Domain.Common;
using Todo.Domain.Events;
using Todo.Domain.Households;

namespace Todo.Application.Subscriptions.CreateSubscription;

internal sealed class CreateSubscriptionHandler(
    ICalendarSubscriptionRepository subscriptions, IHouseholdMemberRepository members, IHouseholdAccess access, ICurrentUser user, TimeProvider time)
    : ICommandHandler<CreateSubscriptionCommand, CreatedSubscriptionView>
{
    public async Task<Result<CreatedSubscriptionView>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        // A member the plugin never saw joining has no projection, and the feed would answer 404 for them: add it from the verified role.
        if (await members.GetAsync(command.HouseholdId, user.RequiredUserId, cancellationToken) is null)
        {
            var role = await access.GetRoleAsync(command.HouseholdId, cancellationToken);
            if (role is null)
            {
                return Result.Fail(PlannerErrors.SubscriptionNotFound);
            }

            members.Add(new HouseholdMemberProjection(command.HouseholdId, user.RequiredUserId, role.Value.ToString(), time.GetUtcNow()));
        }

        var token = FeedToken.Generate();
        var subscription = new CalendarSubscription(
            command.HouseholdId, user.RequiredUserId, FeedToken.Hash(token), command.Label, command.IncludeHouseholdEvents,
            command.IncludePrivateEvents, command.TasksMode, time.GetUtcNow());
        subscriptions.Add(subscription);

        return Result.Ok(new CreatedSubscriptionView(
            CalendarSubscriptionView.Of(subscription), token, $"/plugins/todo/feed/{token}.ics"));
    }
}
