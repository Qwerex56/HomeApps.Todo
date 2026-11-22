namespace Todo.Domain.Tasks;

/// <summary>Which occurrences of a recurring task or event an edit or delete reaches.</summary>
public enum RecurrenceScope
{
    ThisInstance,
    ThisAndFuture,
    All,
}
