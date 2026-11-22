using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.HouseholdRestored;

internal sealed class HouseholdRestoredHandler(IInactiveMarkerRepository markers) : ICommandHandler<HouseholdRestoredCommand>
{
    public async Task<Result> Handle(HouseholdRestoredCommand command, CancellationToken cancellationToken)
    {
        await markers.ClearAsync(command.HouseholdId, null, cancellationToken);
        return Result.Ok();
    }
}
