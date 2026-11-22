using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Common;

namespace Todo.Application.Events.UpdateCalendarEvent;

internal sealed class UpdateCalendarEventHandler(ICalendarEventRepository events, Actors actors, TimeProvider time)
    : ICommandHandler<UpdateCalendarEventCommand, CalendarEventDetailView>
{
    public async Task<Result<CalendarEventDetailView>> Handle(UpdateCalendarEventCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var found = await events.GetAsync(command.HouseholdId, command.EventId, cancellationToken);
        if (found is null || !found.CanSee(actor))
        {
            return Result.Fail<CalendarEventDetailView>(PlannerErrors.EventNotFound);
        }

        var input = command.Event;
        var edited = found.Edit(actor, input.Title, input.Description, input.Location, input.TimeZoneId, input.StartUtc, input.EndUtc,
            input.IsAllDay, input.IsPrivate, input.Attendees, input.ReminderMinutes, time);
        return edited.IsFailed ? edited.ToResult<CalendarEventDetailView>() : CalendarEventDetailView.Of(found);
    }
}
