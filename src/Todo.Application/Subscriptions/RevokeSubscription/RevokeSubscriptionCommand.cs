using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Subscriptions.RevokeSubscription;

/// <summary>Removes the caller's own subscription; the link stops working.</summary>
public sealed record RevokeSubscriptionCommand(Guid HouseholdId, Guid Id) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}
