using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.MemberRoleChanged;

internal sealed class MemberRoleChangedHandler(IHouseholdMemberRepository members) : ICommandHandler<MemberRoleChangedCommand>
{
    public async Task<Result> Handle(MemberRoleChangedCommand command, CancellationToken cancellationToken)
    {
        (await members.GetAsync(command.HouseholdId, command.UserId, cancellationToken))?.ChangeRole(command.Role);
        return Result.Ok();
    }
}
