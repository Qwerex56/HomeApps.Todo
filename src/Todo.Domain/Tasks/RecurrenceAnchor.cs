namespace Todo.Domain.Tasks;

/// <summary>What the next due date of a recurring task counts from (Q2).</summary>
public enum RecurrenceAnchor
{
    FromDueDate,
    FromCompletionDate,
}
