namespace Todo.Domain.Events;

/// <summary>Which tasks an ICS subscription feed carries.</summary>
public enum SubscriptionTasksMode
{
    None,
    Assigned,
    All,
}
