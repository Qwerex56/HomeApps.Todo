using Todo.Domain.Events;
using Todo.Domain.Settings;
using Todo.Domain.Tasks;

namespace Todo.Application.Reminders;

/// <summary>Everything one reminder scan looks at. Events and series of departed owners, and recipients marked inactive, are already left out.</summary>
public sealed record ReminderSources(
    IReadOnlyList<CalendarEvent> Events,
    IReadOnlyList<EventSeries> Series,
    IReadOnlyList<EventOverride> SeriesOverrides,
    IReadOnlyList<EventReminderOverride> UserOverrides,
    IReadOnlyList<PlannerSettings> Settings,
    IReadOnlyList<ReminderRecipient> Members,
    IReadOnlyList<TodoTask> Tasks);
