using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.BuildingBlocks.Domain.Errors;
using Todo.Application.Abstractions;
using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Application.ExternalCalendars.AddExternalCalendar;

internal sealed class AddExternalCalendarHandler(
    IExternalCalendarRepository calendars, ExternalCalendarSyncer syncer, ICurrentUser user, TimeProvider time)
    : ICommandHandler<AddExternalCalendarCommand, ExternalCalendarView>
{
    public async Task<Result<ExternalCalendarView>> Handle(AddExternalCalendarCommand command, CancellationToken cancellationToken)
    {
        var url = command.Url?.Trim() ?? string.Empty;
        if (url.Length > ExternalCalendarImport.MaxUrlLength || !Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https") || uri.UserInfo.Length > 0)
        {
            return Result.Fail(PlannerErrors.InvalidUrl);
        }

        var created = ExternalCalendarImport.Create(command.HouseholdId, user.RequiredUserId, url, command.Name, command.Color, command.Visibility, time);
        if (created.IsFailed)
        {
            return created.ToResult();
        }

        // A refused address or a file that is not a calendar is rejected. A server that is merely unreachable now is
        // kept: the job tries again and the owner sees the error and that the data is missing.
        var synced = await syncer.SyncAsync(created.Value, cancellationToken);
        if (synced.IsFailed && synced.Errors[0] is not AppError { Code: "planner.sync_failed" })
        {
            return synced;
        }

        calendars.Add(created.Value);
        return ExternalCalendarView.Of(created.Value);
    }
}
