using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.HouseholdArchived;

internal sealed class HouseholdArchivedHandler(IInactiveMarkerRepository markers) : ICommandHandler<HouseholdArchivedCommand>
{
    public async Task<Result> Handle(HouseholdArchivedCommand command, CancellationToken cancellationToken)
    {
        await markers.MarkAsync(command.HouseholdId, null, cancellationToken);
        return Result.Ok();
    }
}
