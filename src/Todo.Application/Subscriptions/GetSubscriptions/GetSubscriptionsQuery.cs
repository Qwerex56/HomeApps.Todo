using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Subscriptions.GetSubscriptions;

/// <summary>The caller's own feed links in a household.</summary>
public sealed record GetSubscriptionsQuery(Guid HouseholdId) : IQuery<IReadOnlyList<CalendarSubscriptionView>>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}
