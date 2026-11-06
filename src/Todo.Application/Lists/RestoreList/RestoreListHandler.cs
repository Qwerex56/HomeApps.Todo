using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Lists.RestoreList;

internal sealed class RestoreListHandler(ITodoListRepository lists, Actors actors, TimeProvider time) : ICommandHandler<RestoreListCommand>
{
    public async Task<Result> Handle(RestoreListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        return list.IsFailed ? list.ToResult() : list.Value.Restore(actor, time);
    }
}
