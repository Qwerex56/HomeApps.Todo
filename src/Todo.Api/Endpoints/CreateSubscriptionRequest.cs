using Todo.Application.Subscriptions.CreateSubscription;
using Todo.Domain.Events;

namespace Todo.Api.Endpoints;

internal sealed record CreateSubscriptionRequest(
    string Label, bool IncludeHouseholdEvents = true, bool IncludePrivateEvents = false, SubscriptionTasksMode TasksMode = SubscriptionTasksMode.None)
{
    public CreateSubscriptionCommand ToCommand(Guid householdId) =>
        new(householdId, Label, IncludeHouseholdEvents, IncludePrivateEvents, TasksMode);
}
