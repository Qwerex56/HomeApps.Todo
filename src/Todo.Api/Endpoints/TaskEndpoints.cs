using HomeApps.BuildingBlocks.Api.Endpoints;
using HomeApps.BuildingBlocks.Api.Results;
using MediatR;
using Todo.Application.Common;
using Todo.Application.Tasks;
using Todo.Domain.Tasks;

namespace Todo.Api.Endpoints;

internal sealed record MoveTaskRequest(Guid ListId, Guid? BeforeTaskId);

/// <summary>Tasks of one household (plan 4.3).</summary>
internal sealed class TaskEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var household = app.MapGroup("/households/{householdId:guid}").WithTags("Tasks");

        // assignee=me and dueBy=YYYY-MM-DD (the household's today, from the client) make the "moje" and "na dziś" views.
        household.MapGet("/tasks", async (Guid householdId, Guid? list, string? assignee, DateOnly? dueBy, TodoStatus? status,
                ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetTasksQuery(householdId, list, string.Equals(assignee, "me", StringComparison.OrdinalIgnoreCase), dueBy, status), ct))
            .ToHttpResult())
            .Produces<IReadOnlyList<TaskView>>();

        household.MapPost("/lists/{listId:guid}/tasks", async (Guid householdId, Guid listId, TaskInput body, ISender sender, CancellationToken ct) =>
            (await sender.Send(new CreateTaskCommand(householdId, listId, body), ct))
            .Map(id => new IdResponse(id))
            .ToCreatedHttpResult(created => $"/plugins/todo/api/households/{householdId}/tasks/{created.Id}"))
            .Produces<IdResponse>(StatusCodes.Status201Created);

        household.MapPut("/tasks/{taskId:guid}", async (Guid householdId, Guid taskId, TaskInput body, ISender sender, CancellationToken ct) =>
            (await sender.Send(new UpdateTaskCommand(householdId, taskId, body), ct)).ToHttpResult());

        household.MapPost("/tasks/{taskId:guid}/complete", async (Guid householdId, Guid taskId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new CompleteTaskCommand(householdId, taskId), ct)).ToHttpResult());

        household.MapPost("/tasks/{taskId:guid}/reopen", async (Guid householdId, Guid taskId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new ReopenTaskCommand(householdId, taskId), ct)).ToHttpResult());

        household.MapPost("/tasks/{taskId:guid}/move", async (Guid householdId, Guid taskId, MoveTaskRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(new MoveTaskCommand(householdId, taskId, body.ListId, body.BeforeTaskId), ct)).ToHttpResult());

        household.MapDelete("/tasks/{taskId:guid}", async (Guid householdId, Guid taskId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new DeleteTaskCommand(householdId, taskId), ct)).ToHttpResult());
    }
}
