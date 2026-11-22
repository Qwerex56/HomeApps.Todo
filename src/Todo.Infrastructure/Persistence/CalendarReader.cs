using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Events;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence;

internal sealed class CalendarReader(TodoDbContext db) : ICalendarReader
{
    public async Task<IReadOnlyList<CalendarEvent>> EventsAsync(
        Guid householdId, Guid userId, bool isGuest, DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken cancellationToken)
    {
        var found = await db.CalendarEvents
            .Where(e => e.HouseholdId == householdId && e.StartUtc < toUtc && e.EndUtc >= fromUtc
                && (!e.IsPrivate || e.CreatedBy == userId) && e.OwnerLeftAt == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return isGuest ? found.Where(e => e.IsVisibleToGuest(userId)).ToList() : found;
    }

    public async Task<IReadOnlyList<(EventSeries Series, IReadOnlyList<EventOverride> Overrides)>> SeriesAsync(
        Guid householdId, Guid userId, bool isGuest, DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken cancellationToken)
    {
        var series = await db.EventSeries
            .Where(s => s.HouseholdId == householdId && s.SeriesStartUtc < toUtc && (s.SeriesEndUtc == null || s.SeriesEndUtc >= fromUtc)
                && (!s.IsPrivate || s.CreatedBy == userId) && s.OwnerLeftAt == null && !(isGuest && s.IsPrivate))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var ids = series.Select(s => s.Id).ToList();
        var overrides = (await db.EventOverrides.Where(o => ids.Contains(o.SeriesId)).AsNoTracking().ToListAsync(cancellationToken))
            .ToLookup(o => o.SeriesId);

        return series.Select(s => (s, (IReadOnlyList<EventOverride>)overrides[s.Id].ToList())).ToList();
    }

    public async Task<IReadOnlyList<TaskView>> TasksDueAsync(
        Guid householdId, Guid userId, DateOnly firstDay, DateOnly lastDay, CancellationToken cancellationToken)
    {
        var lists = db.Lists.Where(l => l.HouseholdId == householdId
            && (l.Visibility == ListVisibility.Shared || (l.OwnerId == userId && l.OwnerLeftAt == null)));
        var found = await db.Tasks
            .Where(t => t.HouseholdId == householdId && t.Due != null && t.Due.Date >= firstDay && t.Due.Date <= lastDay
                && lists.Any(l => l.Id == t.ListId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return found
            .Select(t => new TaskView(
                t.Id, t.ListId, t.Title.Value, t.Notes?.Value, t.Due?.Date, t.Due?.Time, t.AssigneeId, t.Priority, t.Status,
                t.Position, t.CreatedBy, t.CreatedAt, t.CompletedBy, t.CompletedAt))
            .ToList();
    }
}
