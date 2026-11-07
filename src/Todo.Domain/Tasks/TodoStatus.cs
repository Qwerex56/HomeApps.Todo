namespace Todo.Domain.Tasks;

/// <summary>Not called TaskStatus so it never clashes with <see cref="System.Threading.Tasks.TaskStatus"/>.</summary>
public enum TodoStatus
{
    Open = 0,
    Done = 1,
}
