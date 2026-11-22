namespace Todo.Application.Reminders;

/// <summary>A user the plugin itself knows in a household (settings, authorship, attendance, assignment, a feed link) and who is not marked inactive.</summary>
public sealed record ReminderRecipient(Guid HouseholdId, Guid UserId);
