using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Domain.Common;

namespace Todo.Application.ExternalCalendars.SyncExternalCalendar;

internal sealed class SyncExternalCalendarHandler(IExternalCalendarRepository calendars, ExternalCalendarSyncer syncer, ICurrentUser user)
    : ICommandHandler<SyncExternalCalendarCommand, ExternalCalendarView>
{
    public async Task<Result<ExternalCalendarView>> Handle(SyncExternalCalendarCommand command, CancellationToken cancellationToken)
    {
        // Somebody else's import is as good as missing: its address is a secret.
        var found = await calendars.GetAsync(command.HouseholdId, command.Id, cancellationToken);
        if (found is null || found.UserId != user.RequiredUserId || found.IsPaused)
        {
            return Result.Fail(PlannerErrors.CalendarNotFound);
        }

        var synced = await syncer.SyncAsync(found, cancellationToken);
        return synced.IsFailed ? Result.Fail(PlannerErrors.SyncFailed.CausedBy(synced.Errors[0])) : ExternalCalendarView.Of(found);
    }
}
