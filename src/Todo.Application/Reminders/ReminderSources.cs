using Todo.Domain.Events;
using Todo.Domain.Households;
using Todo.Domain.Settings;
using Todo.Domain.Tasks;

namespace Todo.Application.Reminders;

/// <summary>Everything one reminder scan looks at. Events and series of departed owners are already left out.</summary>
public sealed record ReminderSources(
    IReadOnlyList<CalendarEvent> Events,
    IReadOnlyList<EventSeries> Series,
    IReadOnlyList<EventOverride> SeriesOverrides,
    IReadOnlyList<EventReminderOverride> UserOverrides,
    IReadOnlyList<PlannerSettings> Settings,
    IReadOnlyList<HouseholdMemberProjection> Members,
    IReadOnlyList<TodoTask> Tasks);
