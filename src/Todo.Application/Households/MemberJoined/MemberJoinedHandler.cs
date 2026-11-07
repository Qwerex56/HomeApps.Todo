using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.MemberJoined;

internal sealed class MemberJoinedHandler(ITodoListRepository lists, TimeProvider time) : ICommandHandler<MemberJoinedCommand>
{
    public async Task<Result> Handle(MemberJoinedCommand command, CancellationToken cancellationToken)
    {
        foreach (var list in await lists.ListPrivateOfAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            list.OwnerReturned(time);
        }

        return Result.Ok();
    }
}
