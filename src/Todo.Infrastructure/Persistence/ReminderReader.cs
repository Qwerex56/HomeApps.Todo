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
        var members = await db.HouseholdMembers.AsNoTracking().Where(m => m.LeftAt == null && !m.IsDisabled && !m.IsArchived).ToListAsync(cancellationToken);
        var households = members.Select(m => m.HouseholdId).Distinct().ToList();
        var settings = await db.PlannerSettings.AsNoTracking().Where(s => households.Contains(s.HouseholdId)).ToListAsync(cancellationToken);
        var fromDay = DateOnly.FromDateTime(from.UtcDateTime).AddDays(-1);
        var toDay = DateOnly.FromDateTime(horizon.UtcDateTime).AddDays(1);
        var tasks = await db.Tasks.AsNoTracking().Where(t => t.Status == TodoStatus.Open && ((t.ReminderAt > from && t.ReminderAt <= horizon)
            || (t.Due != null && t.Due.Date >= fromDay && t.Due.Date <= toDay))).ToListAsync(cancellationToken);
        return new ReminderSources(events, series, changes, own, settings, members, tasks);
    }
}
