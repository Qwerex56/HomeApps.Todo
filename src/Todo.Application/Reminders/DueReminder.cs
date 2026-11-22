namespace Todo.Application.Reminders;

/// <summary>One notification the job has to send.</summary>
public sealed record DueReminder(Guid UserId, Guid HouseholdId, string Category, string Title, string Body, string Tag);
