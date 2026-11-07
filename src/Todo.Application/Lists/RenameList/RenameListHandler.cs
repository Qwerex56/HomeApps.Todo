using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Lists.RenameList;

internal sealed class RenameListHandler(ITodoListRepository lists, Actors actors, TimeProvider time) : ICommandHandler<RenameListCommand>
{
    public async Task<Result> Handle(RenameListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        return list.IsFailed ? list.ToResult() : list.Value.Rename(actor, command.Name, time);
    }
}
