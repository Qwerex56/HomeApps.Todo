using Todo.Application.Reminders;

namespace Todo.Application.Abstractions;

/// <summary>Loads what the reminder scan needs (ADR 3.8).</summary>
public interface IReminderReader
{
    /// <summary>Items that may remind in (<paramref name="from"/>, <paramref name="horizon"/>] plus the people they concern.</summary>
    Task<ReminderSources> LoadAsync(DateTimeOffset from, DateTimeOffset horizon, CancellationToken cancellationToken);
}
