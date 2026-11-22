using Todo.Domain.Events;

namespace Todo.Application.Subscriptions;

public sealed record CalendarSubscriptionView(
    Guid Id, string Label, bool IncludeHouseholdEvents, bool IncludePrivateEvents, SubscriptionTasksMode TasksMode,
    DateTimeOffset? LastAccessedAt, DateTimeOffset CreatedAt)
{
    public static CalendarSubscriptionView Of(CalendarSubscription s) =>
        new(s.Id, s.Label, s.IncludeHouseholdEvents, s.IncludePrivateEvents, s.TasksMode, s.LastAccessedAt, s.CreatedAt);
}
