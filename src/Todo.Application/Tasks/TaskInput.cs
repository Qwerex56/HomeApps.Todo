using Todo.Domain.Tasks;

namespace Todo.Application.Tasks;

/// <summary>The fields a person edits on a task. A time needs a day.</summary>
public sealed record TaskInput(string Title, string? Notes, DateOnly? DueDate, TimeOnly? DueTime, TaskPriority Priority, Guid? AssigneeId)
{
    public TaskDue? Due => DueDate is { } date ? new TaskDue(date, DueTime) : null;
}
