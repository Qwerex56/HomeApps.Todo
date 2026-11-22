using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Common;

namespace Todo.Application.Events.DeleteCalendarEvent;

internal sealed class DeleteCalendarEventHandler(ICalendarEventRepository events, Actors actors) : ICommandHandler<DeleteCalendarEventCommand>
{
    public async Task<Result> Handle(DeleteCalendarEventCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await events.GetAsync(command.HouseholdId, command.EventId, cancellationToken);
        if (found is null || !found.CanSee(actor))
        {
            return Result.Fail(PlannerErrors.EventNotFound);
        }

        var allowed = found.CanDelete(actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        events.Remove(found);
        return Result.Ok();
    }
}
