using Todo.Domain.Events;
using Todo.Domain.Reminders;
using Todo.Domain.Tasks;

namespace Todo.Application.Reminders;

/// <summary>
/// ADR 3.8: works out which reminders fire in (from, now]. People who left, are disabled or whose household is archived
/// are already left out of <see cref="ReminderSources.Members"/> by the reader; reminders more than 60 minutes late are dropped.
/// </summary>
public static class ReminderScanner
{
    /// <summary>How far ahead of an event the longest reminder can be set.</summary>
    public static readonly TimeSpan Horizon = TimeSpan.FromMinutes(EventReminder.MaxMinutes);

    public const string DefaultZoneId = "Europe/Warsaw";

    public static ReminderScan Scan(ReminderSources sources, DateTimeOffset from, DateTimeOffset now, string defaultTimeZone = DefaultZoneId)
    {
        ArgumentNullException.ThrowIfNull(sources);
        var due = new List<DueReminder>();
        var dropped = 0;
        var members = sources.Members.ToLookup(m => m.HouseholdId, m => m.UserId);
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

        void Judge(DateTimeOffset at, Guid user, TodoTask t, string body, string key)
        {
            switch (ReminderRules.Judge(at, from, now))
            {
                case ReminderVerdict.Send:
                    due.Add(new DueReminder(user, t.HouseholdId, TodoPlugin.TaskReminderNotification, t.Title.Value, body, $"task-{t.Id}-{key}-{user}"));
                    break;
                case ReminderVerdict.Drop:
                    dropped++;
                    break;
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

        var settingsOf = sources.Settings.ToDictionary(s => (s.HouseholdId, s.UserId));
        var openTasks = sources.Tasks.Where(t => t.Status == TodoStatus.Open).ToList();

        foreach (var t in openTasks)
        {
            var user = t.AssigneeId ?? t.CreatedBy;
            if (!members[t.HouseholdId].Contains(user))
            {
                continue;
            }

            if (t.ReminderAt is not null)
            {
                Judge(t.ReminderAt.Value, user, t, "Termin zadania", t.ReminderAt.Value.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            settingsOf.TryGetValue((t.HouseholdId, user), out var mine);
            if (t.Due?.Time is { } dueTime && mine is not null)
            {
                var zone = ReminderRules.ResolveZone(mine.TimeZoneId, defaultTimeZone);
                var dueAt = ReminderRules.LocalToUtc(t.Due.Date, dueTime, zone);
                foreach (var minutes in mine.DefaultTaskReminders.Select(r => r.MinutesBefore).Distinct())
                {
                    Judge(dueAt.AddMinutes(-minutes), user, t, minutes == 0 ? "Termin zadania: teraz" : $"Termin zadania za {minutes} min", $"due-{minutes}");
                }
            }
        }

        foreach (var group in openTasks.Where(t => t.Due is not null).GroupBy(t => (t.HouseholdId, User: t.AssigneeId ?? t.CreatedBy)))
        {
            if (!members[group.Key.HouseholdId].Contains(group.Key.User))
            {
                continue;
            }

            settingsOf.TryGetValue(group.Key, out var mine);
            var zone = ReminderRules.ResolveZone(mine?.TimeZoneId, defaultTimeZone);
            foreach (var date in new[] { DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(from, zone).Date), DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(now, zone).Date) }.Distinct())
            {
                var count = group.Count(t => t.Due!.Date == date);
                if (count == 0)
                {
                    continue;
                }

                var body = count == 1 ? "Dziś masz 1 zadanie" : $"Dziś masz zadań: {count}";
                var tag = $"digest-{group.Key.HouseholdId}-{date:yyyyMMdd}-{group.Key.User}";
                switch (ReminderRules.Judge(ReminderRules.LocalToUtc(date, ReminderRules.DigestTime, zone), from, now))
                {
                    case ReminderVerdict.Send:
                        due.Add(new DueReminder(group.Key.User, group.Key.HouseholdId, TodoPlugin.TaskReminderNotification, "Zadania na dziś", body, tag));
                        break;
                    case ReminderVerdict.Drop:
                        dropped++;
                        break;
                }
            }
        }

        return new ReminderScan(due, dropped);
    }
}
