using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.ExternalCalendars.GetExternalCalendars;

internal sealed class GetExternalCalendarsHandler(IExternalCalendarRepository calendars, ICurrentUser user)
    : IQueryHandler<GetExternalCalendarsQuery, IReadOnlyList<ExternalCalendarView>>
{
    public async Task<Result<IReadOnlyList<ExternalCalendarView>>> Handle(GetExternalCalendarsQuery query, CancellationToken cancellationToken) =>
        Result.Ok<IReadOnlyList<ExternalCalendarView>>(
            (await calendars.ListOfUserAsync(query.HouseholdId, user.RequiredUserId, cancellationToken)).Select(ExternalCalendarView.Of).ToList());
}
