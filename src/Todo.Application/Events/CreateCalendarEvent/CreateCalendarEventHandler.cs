using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Events;

namespace Todo.Application.Events.CreateCalendarEvent;

internal sealed class CreateCalendarEventHandler(
    ICalendarEventRepository events, IEventSeriesRepository series, Actors actors, TimeProvider time)
    : ICommandHandler<CreateCalendarEventCommand, CalendarEventDetailView>
{
    public async Task<Result<CalendarEventDetailView>> Handle(CreateCalendarEventCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var input = command.Event;

        if (!string.IsNullOrWhiteSpace(input.Rrule))
        {
            var created = EventSeries.Create(command.HouseholdId, actor, input.Title, input.Description, input.Location, input.TimeZoneId,
                input.StartUtc, input.EndUtc, input.IsAllDay, input.IsPrivate, input.Rrule, null, time);
            if (created.IsFailed)
            {
                return created.ToResult<CalendarEventDetailView>();
            }

            series.Add(created.Value);
            var first = created.Value.FirstOccurrence();
            return first is null
                ? Result.Fail<CalendarEventDetailView>(Todo.Domain.Common.PlannerErrors.InvalidRule)
                : CalendarEventDetailView.Of(created.Value, first);
        }

        var single = CalendarEvent.Create(command.HouseholdId, actor, input.Title, input.Description, input.Location, input.TimeZoneId,
            input.StartUtc, input.EndUtc, input.IsAllDay, input.IsPrivate, input.Attendees, input.ReminderMinutes, time);
        if (single.IsFailed)
        {
            return single.ToResult<CalendarEventDetailView>();
        }

        events.Add(single.Value);
        return CalendarEventDetailView.Of(single.Value);
    }
}
