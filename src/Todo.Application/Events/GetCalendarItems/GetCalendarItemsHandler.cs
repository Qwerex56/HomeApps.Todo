using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Contracts.Households;
using HomeApps.Plugins;
using Todo.Application.Abstractions;

namespace Todo.Application.Events.GetCalendarItems;

internal sealed class GetCalendarItemsHandler(ICalendarReader reader, IHouseholdAccess access, ICurrentUser user)
    : IQueryHandler<GetCalendarItemsQuery, IReadOnlyList<CalendarItemView>>
{
    public async Task<Result<IReadOnlyList<CalendarItemView>>> Handle(GetCalendarItemsQuery query, CancellationToken cancellationToken)
    {
        var layers = string.IsNullOrWhiteSpace(query.Layers) ? ["events", "private", "tasks"] : GetCalendarItemsValidator.Split(query.Layers);
        var showEvents = layers.Contains("events");
        var showPrivate = layers.Contains("private");
        var userId = user.RequiredUserId;
        var guest = await access.GetRoleAsync(query.HouseholdId, cancellationToken) < HouseholdRole.Member;
        var from = query.From.ToUniversalTime();
        var to = query.To.ToUniversalTime();
        var items = new List<(DateTimeOffset At, CalendarItemView View)>();

        if (showEvents || showPrivate)
        {
            var events = await reader.EventsAsync(query.HouseholdId, userId, guest, from, to, cancellationToken);
            items.AddRange(events.Where(e => e.IsPrivate ? showPrivate : showEvents).Select(e => (e.StartUtc, CalendarItemView.Of(e))));

            var all = await reader.SeriesAsync(query.HouseholdId, userId, guest, from, to, cancellationToken);
            foreach (var (series, overrides) in all.Where(s => s.Series.IsPrivate ? showPrivate : showEvents))
            {
                items.AddRange(series.OccurrencesBetween(from, to, overrides).Select(o => (o.StartUtc, CalendarItemView.Of(series, o))));
            }
        }

        if (layers.Contains("tasks"))
        {
            // `to` is exclusive: a midnight `to` leaves out its own day. Task due dates are matched as UTC dates.
            var lastDay = DateOnly.FromDateTime(to.UtcDateTime);
            if (to.UtcDateTime.TimeOfDay == TimeSpan.Zero)
            {
                lastDay = lastDay.AddDays(-1);
            }

            var tasks = await reader.TasksDueAsync(query.HouseholdId, userId, DateOnly.FromDateTime(from.UtcDateTime), lastDay, cancellationToken);
            items.AddRange(tasks.Select(t => (new DateTimeOffset(t.DueDate!.Value.ToDateTime(t.DueTime ?? TimeOnly.MinValue), TimeSpan.Zero), CalendarItemView.Of(t))));
        }

        return Result.Ok<IReadOnlyList<CalendarItemView>>(items.OrderBy(i => i.At).Select(i => i.View).ToList());
    }
}
