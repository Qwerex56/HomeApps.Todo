using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Events.GetCalendarItems;

/// <summary>
/// Events, series occurrences and tasks with a due day in [From, To), by start. <c>Layers</c> is a comma separated
/// subset of <c>events</c> (household events), <c>private</c> (own private events), <c>tasks</c> and <c>external</c>
/// (imported calendars, as their owners allow; not for guests); empty means all four. Month, week, day and agenda views are all this query
/// with a different window of at most <see cref="MaxDays"/> days.
/// </summary>
public sealed record GetCalendarItemsQuery(Guid HouseholdId, DateTimeOffset From, DateTimeOffset To, string? Layers)
    : IQuery<IReadOnlyList<CalendarItemView>>, IHouseholdRequest
{
    public const int MaxDays = 366;

    public string Permission => TodoPlugin.Read;
}
