namespace Todo.Domain.Events;

/// <summary>Q4: who sees an imported calendar and how much.</summary>
public enum ExternalCalendarVisibility
{
    OnlyMe,
    Busy,
    Details,
}
