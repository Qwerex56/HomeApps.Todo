using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Lists.ArchiveList;

internal sealed class ArchiveListHandler(ITodoListRepository lists, Actors actors, TimeProvider time) : ICommandHandler<ArchiveListCommand>
{
    public async Task<Result> Handle(ArchiveListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        return list.IsFailed ? list.ToResult() : list.Value.Archive(actor, time);
    }
}
