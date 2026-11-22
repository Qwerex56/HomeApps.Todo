using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.HouseholdArchived;

internal sealed class HouseholdArchivedHandler(IHouseholdMemberRepository members) : ICommandHandler<HouseholdArchivedCommand>
{
    public async Task<Result> Handle(HouseholdArchivedCommand command, CancellationToken cancellationToken)
    {
        foreach (var member in await members.ListOfHouseholdAsync(command.HouseholdId, cancellationToken))
        {
            member.Archive();
        }

        return Result.Ok();
    }
}
