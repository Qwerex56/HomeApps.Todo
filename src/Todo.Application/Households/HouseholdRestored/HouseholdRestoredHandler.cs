using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.HouseholdRestored;

internal sealed class HouseholdRestoredHandler(IHouseholdMemberRepository members) : ICommandHandler<HouseholdRestoredCommand>
{
    public async Task<Result> Handle(HouseholdRestoredCommand command, CancellationToken cancellationToken)
    {
        foreach (var member in await members.ListOfHouseholdAsync(command.HouseholdId, cancellationToken))
        {
            member.Restore();
        }

        return Result.Ok();
    }
}
