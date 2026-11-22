namespace Todo.Application.Subscriptions;

/// <summary>The only time the token and so the feed address is shown.</summary>
public sealed record CreatedSubscriptionView(CalendarSubscriptionView Subscription, string Token, string FeedPath);
