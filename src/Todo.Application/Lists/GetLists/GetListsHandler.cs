using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Lists.GetLists;

internal sealed class GetListsHandler(ITodoReader reader, ICurrentUser user) : IQueryHandler<GetListsQuery, IReadOnlyList<ListView>>
{
    public async Task<Result<IReadOnlyList<ListView>>> Handle(GetListsQuery query, CancellationToken cancellationToken) =>
        Result.Ok(await reader.ListsAsync(query.HouseholdId, user.RequiredUserId, cancellationToken));
}
