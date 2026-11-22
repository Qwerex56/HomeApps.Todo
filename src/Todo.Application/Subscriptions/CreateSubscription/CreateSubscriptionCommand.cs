using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Domain.Events;

namespace Todo.Application.Subscriptions.CreateSubscription;

public sealed record CreateSubscriptionCommand(
    Guid HouseholdId, string Label, bool IncludeHouseholdEvents, bool IncludePrivateEvents, SubscriptionTasksMode TasksMode)
    : ICommand<CreatedSubscriptionView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}
