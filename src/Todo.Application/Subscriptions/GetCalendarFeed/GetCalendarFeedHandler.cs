using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Application.Subscriptions.GetCalendarFeed;

internal sealed class GetCalendarFeedHandler(
    ICalendarSubscriptionRepository subscriptions, IInactiveMarkerRepository markers, ICalendarReader reader,
    ICalendarFeedGenerator generator, TimeProvider time) : IQueryHandler<GetCalendarFeedQuery, string>
{
    private const int TokenLength = 43;
    private static readonly TimeSpan Past = TimeSpan.FromDays(30);
    private static readonly TimeSpan Future = TimeSpan.FromDays(366);

    public async Task<Result<string>> Handle(GetCalendarFeedQuery query, CancellationToken cancellationToken)
    {
        if (query.Token.Length != TokenLength || !query.Token.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_'))
        {
            return Result.Fail(PlannerErrors.SubscriptionNotFound);
        }

        var subscription = await subscriptions.FindByTokenHashAsync(FeedToken.Hash(query.Token), cancellationToken);
        if (subscription is not { Serves: true })
        {
            return Result.Fail(PlannerErrors.SubscriptionNotFound);
        }

        if (await markers.IsInactiveAsync(subscription.HouseholdId, subscription.UserId, cancellationToken))
        {
            return Result.Fail(PlannerErrors.SubscriptionNotFound);
        }

        var now = time.GetUtcNow();
        var from = now - Past;
        var to = now + Future;
        var guest = subscription.IsGuest;
        var wanted = (Func<bool, bool>)(isPrivate => isPrivate ? subscription.IncludePrivateEvents : subscription.IncludeHouseholdEvents);

        var events = (await reader.EventsAsync(subscription.HouseholdId, subscription.UserId, guest, from, to, cancellationToken))
            .Where(e => wanted(e.IsPrivate)).ToList();
        var occurrences = (await reader.SeriesAsync(subscription.HouseholdId, subscription.UserId, guest, from, to, cancellationToken))
            .Where(s => wanted(s.Series.IsPrivate))
            .SelectMany(s => s.Series.OccurrencesBetween(from, to, s.Overrides).Select(o => (s.Series, o)))
            .ToList();

        IReadOnlyList<TaskView> tasks = [];
        if (subscription.TasksMode != SubscriptionTasksMode.None)
        {
            var found = await reader.TasksDueAsync(
                subscription.HouseholdId, subscription.UserId, DateOnly.FromDateTime(from.UtcDateTime), DateOnly.FromDateTime(to.UtcDateTime), cancellationToken);
            tasks = subscription.TasksMode == SubscriptionTasksMode.Assigned ? found.Where(t => t.AssigneeId == subscription.UserId).ToList() : found;
        }

        if (subscription.LastAccessedAt is null || now - subscription.LastAccessedAt > TimeSpan.FromMinutes(1))
        {
            await subscriptions.TouchAsync(subscription.Id, now, cancellationToken);
        }

        return Result.Ok(generator.Generate(subscription.Label, events, occurrences, tasks, now));
    }
}
