namespace Todo.Api.Endpoints;

internal sealed record MoveTaskRequest(Guid ListId, Guid? BeforeTaskId);
