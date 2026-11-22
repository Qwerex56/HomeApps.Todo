using Todo.Application.Settings.UpdatePlannerSettings;

namespace Todo.Api.Endpoints;

/// <summary>Body of <c>PUT calendar/settings</c>: reminders as minutes before, first day of the week, IANA time zone.</summary>
internal sealed record UpdatePlannerSettingsRequest(IReadOnlyList<int>? EventReminders, IReadOnlyList<int>? TaskReminders, DayOfWeek FirstDayOfWeek, string? TimeZone)
{
    public UpdatePlannerSettingsCommand ToCommand(Guid householdId) => new(householdId, EventReminders, TaskReminders, FirstDayOfWeek, TimeZone);
}
