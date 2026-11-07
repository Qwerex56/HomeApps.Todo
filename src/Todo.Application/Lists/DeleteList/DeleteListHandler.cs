using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Application.Lists.DeleteList;

internal sealed class DeleteListHandler(ITodoListRepository lists, Actors actors) : ICommandHandler<DeleteListCommand>
{
    public async Task<Result> Handle(DeleteListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        if (list.IsFailed)
        {
            return list.ToResult();
        }

        var allowed = list.Value.CanDelete(actor);
        if (allowed.IsSuccess)
        {
            lists.Remove(list.Value);
        }

        return allowed;
    }
}
