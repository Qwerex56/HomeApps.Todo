using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Subscriptions.GetCalendarFeed;

/// <summary>The ICS text for a token. Anonymous: any reason not to serve is the same failure, so nothing leaks.</summary>
public sealed record GetCalendarFeedQuery(string Token) : IQuery<string>;
