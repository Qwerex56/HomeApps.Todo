using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Tasks.GetTasks;

internal sealed class GetTasksHandler(ITodoReader reader, ITodoListRepository lists, Actors actors, ICurrentUser user)
    : IQueryHandler<GetTasksQuery, IReadOnlyList<TaskView>>
{
    public async Task<Result<IReadOnlyList<TaskView>>> Handle(GetTasksQuery query, CancellationToken cancellationToken)
    {
        if (query.ListId is { } listId)
        {
            var actor = await actors.GetAsync(query.HouseholdId, cancellationToken);
            var list = await lists.FindVisibleAsync(query.HouseholdId, listId, actor, cancellationToken);
            if (list.IsFailed)
            {
                return list.ToResult<IReadOnlyList<TaskView>>();
            }
        }

        var filter = new TaskFilter(query.ListId, query.AssignedToMe ? user.RequiredUserId : null, query.DueBy, query.Status);
        return Result.Ok(await reader.TasksAsync(query.HouseholdId, user.RequiredUserId, filter, cancellationToken));
    }
}
