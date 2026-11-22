using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Todo.Domain.Events;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence.Configurations;

/// <summary>Value objects stored as one column. Reading back goes through the domain factory, so bad data fails loudly.</summary>
internal static class Converters
{
    public static readonly ValueConverter<ListName, string> ListName = new(n => n.Value, v => Domain.Lists.ListName.Create(v).Value);

    public static readonly ValueConverter<TaskTitle, string> TaskTitle = new(t => t.Value, v => Domain.Tasks.TaskTitle.Create(v).Value);

    /// <summary>For nullable columns; EF never passes null to a converter, it stores null as is.</summary>
    public static readonly ValueConverter<TaskNotes?, string> TaskNotes = new(n => n!.Value, v => Domain.Tasks.TaskNotes.Create(v).Value!);

    public static readonly ValueConverter<EventTitle, string> EventTitle = new(t => t.Value, v => Domain.Events.EventTitle.Create(v).Value);

    public static readonly ValueConverter<EventTitle?, string> EventTitleOrNull =
        new(t => t!.Value, v => Domain.Events.EventTitle.Create(v).Value!);

    public static readonly ValueConverter<RecurrenceRule, string> RecurrenceRule =
        new(r => r.Value, v => Domain.Events.RecurrenceRule.Create(v).Value);

    public static readonly ValueConverter<RecurrenceRule?, string> RecurrenceRuleOrNull =
        new(r => r!.Value, v => Domain.Events.RecurrenceRule.Create(v).Value!);

    /// <summary>Reminders as a JSON array of minutes, e.g. <c>[10,1440]</c>, for jsonb columns.</summary>
    public static readonly ValueConverter<List<EventReminder>, string> Reminders = new(
        r => JsonSerializer.Serialize(r.Select(x => x.MinutesBefore), (JsonSerializerOptions?)null),
        v => Domain.Events.EventReminder.CreateMany(JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null)).Value);

    public static readonly ValueConverter<List<EventReminder>?, string> RemindersOrNull = new(
        r => JsonSerializer.Serialize(r!.Select(x => x.MinutesBefore), (JsonSerializerOptions?)null),
        v => Domain.Events.EventReminder.CreateMany(JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null)).Value);

    public static readonly ValueComparer<List<EventReminder>?> RemindersOrNullComparer = new(
        (a, b) => a == null ? b == null : b != null && a.SequenceEqual(b),
        r => r == null ? 0 : r.Aggregate(0, (h, x) => HashCode.Combine(h, x.MinutesBefore)),
        r => r == null ? null : r.ToList());

    public static readonly ValueComparer<List<EventReminder>> RemindersComparer = new(
        (a, b) => a!.SequenceEqual(b!),
        r => r.Aggregate(0, (h, x) => HashCode.Combine(h, x.MinutesBefore)),
        r => r.ToList());

    /// <summary>Enums are stored as their names: readable in the database and safe if members are reordered.</summary>
    public const int EnumLength = 16;
}
