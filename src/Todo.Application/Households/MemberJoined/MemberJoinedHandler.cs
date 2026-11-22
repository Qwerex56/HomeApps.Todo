using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Domain.Households;

namespace Todo.Application.Households.MemberJoined;

internal sealed class MemberJoinedHandler(ITodoListRepository lists, IHouseholdMemberRepository members, TimeProvider time) : ICommandHandler<MemberJoinedCommand>
{
    public async Task<Result> Handle(MemberJoinedCommand command, CancellationToken cancellationToken)
    {
        var member = await members.GetAsync(command.HouseholdId, command.UserId, cancellationToken);
        if (member is null)
        {
            members.Add(new HouseholdMemberProjection(command.HouseholdId, command.UserId, command.Role, time.GetUtcNow()));
        }
        else
        {
            member.Returned(command.Role, time.GetUtcNow());
        }

        foreach (var list in await lists.ListPrivateOfAsync(command.HouseholdId, command.UserId, cancellationToken))
        {
            list.OwnerReturned(time);
        }

        return Result.Ok();
    }
}
