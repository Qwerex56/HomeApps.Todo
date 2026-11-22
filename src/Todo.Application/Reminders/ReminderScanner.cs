using Todo.Domain.Events;
using Todo.Domain.Households;
using Todo.Domain.Reminders;
using Todo.Domain.Tasks;

namespace Todo.Application.Reminders;

/// <summary>
/// ADR 3.8: works out which reminders fire in (from, now]. Skips people who left, are disabled or whose household is
/// archived (<see cref="HouseholdMemberProjection.IsActive"/>) and drops reminders more than 60 minutes late.
/// </summary>
public static class ReminderScanner
{
    /// <summary>How far ahead of an event the longest reminder can be set.</summary>
    public static readonly TimeSpan Horizon = TimeSpan.FromMinutes(EventReminder.MaxMinutes);

    public static ReminderScan Scan(ReminderSources sources, DateTimeOffset from, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(sources);
        var due = new List<DueReminder>();
        var dropped = 0;
        var members = sources.Members.Where(m => m.IsActive).ToLookup(m => m.HouseholdId, m => m.UserId);
        var userSets = sources.UserOverrides.ToDictionary(o => (o.EventId, o.UserId), o => (IReadOnlyList<int>)o.Reminders.Select(r => r.MinutesBefore).ToList());
        var defaults = sources.Settings.ToDictionary(s => (s.HouseholdId, s.UserId), s => (IReadOnlyList<int>)s.DefaultEventReminders.Select(r => r.MinutesBefore).ToList());

        void ForEvent(Guid key, Guid householdId, string title, DateTimeOffset start, IReadOnlyList<int> onEvent, IEnumerable<Guid> recipients, string occurrence)
        {
            var active = members[householdId].ToHashSet();
            foreach (var user in recipients.Where(active.Contains).Distinct())
            {
                userSets.TryGetValue((key, user), out var own);
                defaults.TryGetValue((householdId, user), out var mine);
                foreach (var minutes in ReminderRules.MinutesFor(onEvent, own, mine ?? []).Distinct())
                {
                    switch (ReminderRules.Judge(start.AddMinutes(-minutes), from, now))
                    {
                        case ReminderVerdict.Send:
                            due.Add(new DueReminder(user, householdId, TodoPlugin.EventReminderNotification, title, minutes == 0 ? "Zaczyna się teraz" : $"Zaczyna się za {minutes} min", $"event-{key}-{occurrence}-{user}-{minutes}"));
                            break;
                        case ReminderVerdict.Drop:
                            dropped++;
                            break;
                    }
                }
            }
        }

        foreach (var e in sources.Events.Where(e => e.OwnerLeftAt is null))
        {
            IEnumerable<Guid> recipients = e.IsPrivate ? new[] { e.CreatedBy } : e.IsForWholeHousehold ? members[e.HouseholdId] : e.Attendees;
            ForEvent(e.Id, e.HouseholdId, e.Title.Value, e.StartUtc, e.Reminders.Select(r => r.MinutesBefore).ToList(), recipients, "once");
        }

        foreach (var s in sources.Series.Where(s => s.OwnerLeftAt is null))
        {
            IEnumerable<Guid> recipients = s.IsPrivate ? new[] { s.CreatedBy } : members[s.HouseholdId];
            var changes = sources.SeriesOverrides.Where(o => o.SeriesId == s.Id);
            foreach (var o in s.OccurrencesBetween(from, now.Add(Horizon), changes))
            {
                ForEvent(s.Id, s.HouseholdId, o.Title, o.StartUtc, o.Reminders, recipients, o.OriginalDate.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        foreach (var t in sources.Tasks.Where(t => t.Status == TodoStatus.Open && t.ReminderAt is not null))
        {
            var user = t.AssigneeId ?? t.CreatedBy;
            if (!members[t.HouseholdId].Contains(user))
            {
                continue;
            }

            switch (ReminderRules.Judge(t.ReminderAt!.Value, from, now))
            {
                case ReminderVerdict.Send:
                    due.Add(new DueReminder(user, t.HouseholdId, TodoPlugin.TaskReminderNotification, t.Title.Value, "Termin zadania", $"task-{t.Id}-{t.ReminderAt.Value.ToUnixTimeSeconds()}-{user}"));
                    break;
                case ReminderVerdict.Drop:
                    dropped++;
                    break;
            }
        }

        return new ReminderScan(due, dropped);
    }
}
