namespace Todo.Application.Reminders;

/// <summary>Result of one scan: what to send and how many reminders were dropped for being over an hour late.</summary>
public sealed record ReminderScan(IReadOnlyList<DueReminder> Due, int Dropped);
