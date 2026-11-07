using HomeApps.BuildingBlocks.Api.Endpoints;
using HomeApps.BuildingBlocks.Api.Results;
using MediatR;
using Todo.Application.Common;
using Todo.Application.Lists.ArchiveList;
using Todo.Application.Lists.CreateList;
using Todo.Application.Lists.DeleteList;
using Todo.Application.Lists.GetLists;
using Todo.Application.Lists.RenameList;
using Todo.Application.Lists.RestoreList;

namespace Todo.Api.Endpoints;

/// <summary>Lists of one household (plan 4.3). Who may do what is decided by the manifest's permissions and the domain.</summary>
internal sealed class ListEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var lists = app.MapGroup("/households/{householdId:guid}/lists").WithTags("Lists");

        lists.MapGet("/", async (Guid householdId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetListsQuery(householdId), ct)).ToHttpResult())
            .Produces<IReadOnlyList<ListView>>();

        lists.MapPost("/", async (Guid householdId, CreateListRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(new CreateListCommand(householdId, body.Name, body.Visibility), ct))
            .Map(id => new IdResponse(id))
            .ToCreatedHttpResult(created => $"/plugins/todo/api/households/{householdId}/lists/{created.Id}"))
            .Produces<IdResponse>(StatusCodes.Status201Created);

        lists.MapPut("/{listId:guid}", async (Guid householdId, Guid listId, RenameListRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(new RenameListCommand(householdId, listId, body.Name), ct)).ToHttpResult());

        lists.MapPost("/{listId:guid}/archive", async (Guid householdId, Guid listId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new ArchiveListCommand(householdId, listId), ct)).ToHttpResult());

        lists.MapPost("/{listId:guid}/restore", async (Guid householdId, Guid listId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new RestoreListCommand(householdId, listId), ct)).ToHttpResult());

        lists.MapDelete("/{listId:guid}", async (Guid householdId, Guid listId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new DeleteListCommand(householdId, listId), ct)).ToHttpResult());
    }
}
