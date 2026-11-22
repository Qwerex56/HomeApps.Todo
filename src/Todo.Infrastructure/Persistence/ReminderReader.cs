using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.Reminders;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence;

internal sealed class ReminderReader(TodoDbContext db) : IReminderReader
{
    public async Task<ReminderSources> LoadAsync(DateTimeOffset from, DateTimeOffset horizon, CancellationToken cancellationToken)
    {
        var events = await db.CalendarEvents.AsNoTracking().Where(e => e.OwnerLeftAt == null && e.StartUtc > from && e.StartUtc <= horizon).ToListAsync(cancellationToken);
        var series = await db.EventSeries.AsNoTracking().Where(s => s.OwnerLeftAt == null && s.SeriesStartUtc <= horizon && (s.SeriesEndUtc == null || s.SeriesEndUtc >= from)).ToListAsync(cancellationToken);
        var seriesIds = series.Select(s => s.Id).ToList();
        var changes = await db.EventOverrides.AsNoTracking().Where(o => seriesIds.Contains(o.SeriesId)).ToListAsync(cancellationToken);
        var ids = events.Select(e => e.Id).Concat(seriesIds).ToList();
        var own = await db.EventReminderOverrides.AsNoTracking().Where(o => ids.Contains(o.EventId)).ToListAsync(cancellationToken);
        var fromDay = DateOnly.FromDateTime(from.UtcDateTime).AddDays(-1);
        var toDay = DateOnly.FromDateTime(horizon.UtcDateTime).AddDays(1);
        var tasks = await db.Tasks.AsNoTracking().Where(t => t.Status == TodoStatus.Open && ((t.ReminderAt > from && t.ReminderAt <= horizon)
            || (t.Due != null && t.Due.Date >= fromDay && t.Due.Date <= toDay))).ToListAsync(cancellationToken);
        var households = events.Select(e => e.HouseholdId).Concat(series.Select(s => s.HouseholdId)).Concat(tasks.Select(t => t.HouseholdId)).Distinct().ToList();
        var settings = await db.PlannerSettings.AsNoTracking().Where(s => households.Contains(s.HouseholdId)).ToListAsync(cancellationToken);
        var feedOwners = await db.CalendarSubscriptions.AsNoTracking().Where(s => s.IsActive && households.Contains(s.HouseholdId)).Select(s => new ReminderRecipient(s.HouseholdId, s.UserId)).ToListAsync(cancellationToken);

        // The plugin keeps no membership copy: the people it knows are those in its own data, minus the inactive markers.
        var known = settings.Select(s => new ReminderRecipient(s.HouseholdId, s.UserId))
            .Concat(feedOwners)
            .Concat(events.SelectMany(e => e.Attendees.Append(e.CreatedBy).Select(u => new ReminderRecipient(e.HouseholdId, u))))
            .Concat(series.Select(s => new ReminderRecipient(s.HouseholdId, s.CreatedBy)))
            .Concat(tasks.SelectMany(t => new[] { t.CreatedBy, t.AssigneeId ?? t.CreatedBy }.Distinct().Select(u => new ReminderRecipient(t.HouseholdId, u))))
            .Distinct().ToList();
        if (known.Count == 0)
        {
            return new ReminderSources(events, series, changes, own, settings, [], tasks);
        }

        var userIds = known.Select(k => k.UserId).Distinct().ToList();
        var markers = await db.InactiveMarkers.AsNoTracking().Where(m =>
            (m.HouseholdId != null && households.Contains(m.HouseholdId.Value)) || (m.UserId != null && userIds.Contains(m.UserId.Value))).ToListAsync(cancellationToken);
        var members = known.Where(k => !markers.Any(m =>
            (m.HouseholdId == k.HouseholdId && m.UserId is null) || (m.HouseholdId is null && m.UserId == k.UserId) || (m.HouseholdId == k.HouseholdId && m.UserId == k.UserId))).ToList();
        return new ReminderSources(events, series, changes, own, settings, members, tasks);
    }
}
