using Todo.Domain.Settings;

namespace Todo.Application.Settings;

/// <summary>The caller's planner preferences; reminders are minutes before. <c>TimeZone</c> is IANA or null for the default.</summary>
public sealed record PlannerSettingsView(
    IReadOnlyList<int> EventReminders, IReadOnlyList<int> TaskReminders, DayOfWeek FirstDayOfWeek, string? TimeZone)
{
    public static PlannerSettingsView Of(PlannerSettings s) =>
        new(s.DefaultEventReminders.Select(r => r.MinutesBefore).ToList(), s.DefaultTaskReminders.Select(r => r.MinutesBefore).ToList(),
            s.FirstDayOfWeek, s.TimeZoneId);

    public static PlannerSettingsView Default => new([], [], DayOfWeek.Monday, null);
}
