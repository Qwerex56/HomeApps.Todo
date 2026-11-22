using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Contracts.Households;
using HomeApps.Plugins;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Common;

namespace Todo.Application.Events.GetCalendarEvent;

internal sealed class GetCalendarEventHandler(
    ICalendarEventRepository events, IEventSeriesRepository series, IHouseholdAccess access, Actors actors)
    : IQueryHandler<GetCalendarEventQuery, CalendarEventDetailView>
{
    public async Task<Result<CalendarEventDetailView>> Handle(GetCalendarEventQuery query, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(query.HouseholdId, cancellationToken);
        var guest = await access.GetRoleAsync(query.HouseholdId, cancellationToken) < HouseholdRole.Member;

        var single = await events.GetAsync(query.HouseholdId, query.EventId, cancellationToken);
        if (single is not null)
        {
            return single.CanSee(actor) && (!guest || single.IsVisibleToGuest(actor.UserId))
                ? CalendarEventDetailView.Of(single)
                : Result.Fail<CalendarEventDetailView>(PlannerErrors.EventNotFound);
        }

        var found = await series.GetAsync(query.HouseholdId, query.EventId, cancellationToken);
        return found is not null && found.CanSee(actor) && (!guest || found.IsVisibleToGuest) && found.FirstOccurrence() is { } first
            ? CalendarEventDetailView.Of(found, first)
            : Result.Fail<CalendarEventDetailView>(PlannerErrors.EventNotFound);
    }
}
