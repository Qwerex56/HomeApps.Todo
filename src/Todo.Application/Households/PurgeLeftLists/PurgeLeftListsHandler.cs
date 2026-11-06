using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.PurgeLeftLists;

internal sealed class PurgeLeftListsHandler(ITodoListRepository lists, TimeProvider time) : ICommandHandler<PurgeLeftListsCommand, int>
{
    public async Task<Result<int>> Handle(PurgeLeftListsCommand command, CancellationToken cancellationToken)
    {
        var due = await lists.ListDueForPurgeAsync(time.GetUtcNow(), cancellationToken);
        foreach (var list in due)
        {
            lists.Remove(list);
        }

        return due.Count;
    }
}
