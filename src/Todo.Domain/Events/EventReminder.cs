using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>A reminder this many minutes before an event starts (0 to 4 weeks).</summary>
public sealed record EventReminder
{
    public const int MaxMinutes = 4 * 7 * 24 * 60;

    private EventReminder(int minutesBefore) => MinutesBefore = minutesBefore;

    public int MinutesBefore { get; }

    public static Result<EventReminder> Create(int minutesBefore) =>
        minutesBefore is < 0 or > MaxMinutes
            ? Result.Fail<EventReminder>(PlannerErrors.InvalidReminder)
            : new EventReminder(minutesBefore);

    /// <summary>For reading back from storage and JSON; callers validate with <see cref="Create"/>.</summary>
    public static Result<List<EventReminder>> CreateMany(IEnumerable<int>? minutes)
    {
        var all = (minutes ?? []).Select(Create).ToList();
        var merged = Result.Merge([.. all]);
        return merged.IsFailed ? merged.ToResult<List<EventReminder>>() : all.Select(r => r.Value).Distinct().ToList();
    }
}
