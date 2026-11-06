using FluentResults;
using Todo.Domain.Common;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Application.Common;

internal static class Loading
{
    /// <summary>The list when the actor may see it; otherwise <c>list.not_found</c>, also for someone else's private list.</summary>
    public static async Task<Result<TodoList>> FindVisibleAsync(
        this ITodoListRepository lists, Guid householdId, Guid listId, Actor actor, CancellationToken cancellationToken)
    {
        var list = await lists.GetAsync(householdId, listId, cancellationToken);
        return list is not null && list.CanSee(actor) ? list : Result.Fail<TodoList>(TodoErrors.ListNotFound);
    }

    /// <summary>The task with its list, when the actor may see that list; otherwise <c>task.not_found</c>.</summary>
    public static async Task<Result<(TodoTask Task, TodoList List)>> FindVisibleAsync(
        this ITodoTaskRepository tasks, ITodoListRepository lists, Guid householdId, Guid taskId, Actor actor,
        CancellationToken cancellationToken)
    {
        var task = await tasks.GetAsync(householdId, taskId, cancellationToken);
        var list = task is null ? null : await lists.GetAsync(householdId, task.ListId, cancellationToken);
        return task is not null && list is not null && list.CanSee(actor)
            ? (task, list)
            : Result.Fail<(TodoTask, TodoList)>(TodoErrors.TaskNotFound);
    }
}
