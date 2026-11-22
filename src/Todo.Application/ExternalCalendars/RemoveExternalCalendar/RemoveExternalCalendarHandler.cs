using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Domain.Common;

namespace Todo.Application.ExternalCalendars.RemoveExternalCalendar;

internal sealed class RemoveExternalCalendarHandler(IExternalCalendarRepository calendars, ICurrentUser user) : ICommandHandler<RemoveExternalCalendarCommand>
{
    public async Task<Result> Handle(RemoveExternalCalendarCommand command, CancellationToken cancellationToken)
    {
        var found = await calendars.GetAsync(command.HouseholdId, command.Id, cancellationToken);
        if (found is null)
        {
            return Result.Fail(PlannerErrors.CalendarNotFound);
        }

        if (found.UserId != user.RequiredUserId)
        {
            return Result.Fail(PlannerErrors.NotOwner);
        }

        calendars.Remove(found);
        return Result.Ok();
    }
}
