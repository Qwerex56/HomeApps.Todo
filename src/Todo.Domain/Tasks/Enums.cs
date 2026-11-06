namespace Todo.Domain.Tasks;

public enum TaskPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
}

/// <summary>Not called TaskStatus so it never clashes with <see cref="System.Threading.Tasks.TaskStatus"/>.</summary>
public enum TodoStatus
{
    Open = 0,
    Done = 1,
}
