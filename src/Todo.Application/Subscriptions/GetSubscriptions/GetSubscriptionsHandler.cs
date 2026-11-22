using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Subscriptions.GetSubscriptions;

internal sealed class GetSubscriptionsHandler(ICalendarSubscriptionRepository subscriptions, ICurrentUser user)
    : IQueryHandler<GetSubscriptionsQuery, IReadOnlyList<CalendarSubscriptionView>>
{
    public async Task<Result<IReadOnlyList<CalendarSubscriptionView>>> Handle(GetSubscriptionsQuery query, CancellationToken cancellationToken) =>
        Result.Ok<IReadOnlyList<CalendarSubscriptionView>>(
            (await subscriptions.ListAsync(query.HouseholdId, user.RequiredUserId, cancellationToken)).Select(CalendarSubscriptionView.Of).ToList());
}
