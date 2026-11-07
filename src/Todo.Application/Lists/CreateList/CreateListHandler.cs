using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Lists;

namespace Todo.Application.Lists.CreateList;

internal sealed class CreateListHandler(ITodoListRepository lists, Actors actors, TimeProvider time) : ICommandHandler<CreateListCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = TodoList.Create(command.HouseholdId, actor, command.Name, command.Visibility, time);
        if (list.IsFailed)
        {
            return list.ToResult<Guid>();
        }

        lists.Add(list.Value);
        return list.Value.Id;
    }
}
