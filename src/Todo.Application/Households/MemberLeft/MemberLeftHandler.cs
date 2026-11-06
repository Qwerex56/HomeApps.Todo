using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.MemberLeft;

internal sealed class MemberLeftHandler(ITodoListRepository lists, ITodoTaskRepository tasks, TimeProvider time) : ICommandHandler<MemberLeftCommand>
{
    public async Task<Result> Handle(MemberLeftCommand command, CancellationToken cancellationToken)
    {
        foreach (var task in await tasks.ListAssignedToAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            task.AssigneeLeft(command.UserId, time);
        }

        foreach (var list in await lists.ListPrivateOfAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            list.OwnerLeft(time);
        }

        return Result.Ok();
    }
}
