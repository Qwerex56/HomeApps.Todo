using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Common;
using Todo.Domain.Events;
using Todo.Domain.Tasks;

namespace Todo.Application.Events.ModifySeriesInstance;

internal sealed class ModifySeriesInstanceHandler(IEventSeriesRepository seriesRepository, Actors actors, TimeProvider time)
    : ICommandHandler<ModifySeriesInstanceCommand, CalendarEventDetailView>
{
    public async Task<Result<CalendarEventDetailView>> Handle(ModifySeriesInstanceCommand command, CancellationToken cancellationToken)
    {
        var scope = ScopeParser.Parse(command.Scope);
        if (scope.IsFailed)
        {
            return scope.ToResult<CalendarEventDetailView>();
        }

        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var series = await seriesRepository.GetAsync(command.HouseholdId, command.SeriesId, cancellationToken);
        if (series is null || !series.CanSee(actor))
        {
            return Result.Fail<CalendarEventDetailView>(PlannerErrors.SeriesNotFound);
        }

        var allowed = series.CanEdit(actor);
        var overrides = await seriesRepository.OverridesAsync(series.Id, cancellationToken);
        var change = overrides.FirstOrDefault(o => o.OriginalInstanceDate == command.Date);
        var occurrence = series.OccurrenceOn(command.Date, change);
        if (allowed.IsFailed || occurrence is null)
        {
            return allowed.IsFailed ? allowed.ToResult<CalendarEventDetailView>() : Result.Fail<CalendarEventDetailView>(PlannerErrors.SeriesNotFound);
        }

        // All / ThisAndFuture take base times from the series, not from an instance override.
        var baseOccurrence = scope.Value == RecurrenceScope.ThisInstance ? occurrence : series.OccurrenceOn(command.Date, null) ?? occurrence;
        var start = command.StartUtc ?? baseOccurrence.StartUtc;
        var end = command.EndUtc ?? (command.StartUtc is { } s ? s + (baseOccurrence.EndUtc - baseOccurrence.StartUtc) : baseOccurrence.EndUtc);
        if (end < start)
        {
            return Result.Fail<CalendarEventDetailView>(PlannerErrors.InvalidDateRange);
        }

        if (scope.Value == RecurrenceScope.ThisInstance)
        {
            return ThisInstance(series, change, command, time);
        }

        if (scope.Value == RecurrenceScope.ThisAndFuture && command.Date > series.StartDate)
        {
            return ThisAndFuture(series, overrides, baseOccurrence, command, start, end);
        }

        var timeChanged = (command.Rrule is not null && command.Rrule != series.Rule.Value) || EventTime.LocalTime(start, series.TimeZoneId) != series.LocalStartTime
            || EventTime.LocalTime(end, series.TimeZoneId) != series.LocalEndTime;
        var edited = series.Edit(actor, command.Title ?? series.Title.Value, command.Description ?? series.Description, command.Location ?? series.Location,
            EventTime.LocalTime(start, series.TimeZoneId), EventTime.LocalTime(end, series.TimeZoneId), series.IsAllDay,
            command.Rrule ?? series.Rule.Value, time);
        if (edited.IsFailed)
        {
            return edited.ToResult<CalendarEventDetailView>();
        }

        if (timeChanged)
        {
            // Moved or rescheduled occurrences no longer match the new rule or time; cancellations stay.
            seriesRepository.RemoveOverrides(overrides.Where(o => !o.IsCancelled));
            change = null;
        }

        return Show(series, series.OccurrenceOn(command.Date, change) ?? series.FirstOccurrence());
    }

    private Result<CalendarEventDetailView> ThisInstance(EventSeries series, EventOverride? change, ModifySeriesInstanceCommand command, TimeProvider clock)
    {
        if (change is null)
        {
            var created = EventOverride.Change(series.Id, command.Date, command.Title, command.StartUtc, command.EndUtc, command.ReminderMinutes, clock);
            if (created.IsFailed)
            {
                return created.ToResult<CalendarEventDetailView>();
            }

            seriesRepository.AddOverride(created.Value);
            change = created.Value;
        }
        else
        {
            var replaced = change.Replace(command.Title, command.StartUtc, command.EndUtc, command.ReminderMinutes, clock);
            if (replaced.IsFailed)
            {
                return replaced.ToResult<CalendarEventDetailView>();
            }
        }

        return Show(series, series.OccurrenceOn(command.Date, change));
    }

    private Result<CalendarEventDetailView> ThisAndFuture(
        EventSeries series, IReadOnlyList<EventOverride> overrides, EventOccurrence occurrence, ModifySeriesInstanceCommand command,
        DateTimeOffset start, DateTimeOffset end)
    {
        var next = EventSeries.Create(series.HouseholdId, new Actor(series.CreatedBy, false), command.Title ?? series.Title.Value,
            command.Description ?? series.Description, command.Location ?? series.Location, series.TimeZoneId, start, end, series.IsAllDay,
            series.IsPrivate, command.Rrule ?? series.Rule.Value, series.SeriesEndUtc, time);
        if (next.IsFailed)
        {
            return next.ToResult<CalendarEventDetailView>();
        }

        var ended = series.EndAt(occurrence.StartUtc.AddSeconds(-1), time);
        if (ended.IsFailed)
        {
            return ended.ToResult<CalendarEventDetailView>();
        }

        seriesRepository.RemoveOverrides(overrides.Where(o => o.OriginalInstanceDate >= command.Date));
        seriesRepository.Add(next.Value);
        return Show(next.Value, next.Value.FirstOccurrence());
    }

    private static Result<CalendarEventDetailView> Show(EventSeries series, EventOccurrence? occurrence) =>
        occurrence is null ? Result.Fail<CalendarEventDetailView>(PlannerErrors.SeriesNotFound) : CalendarEventDetailView.Of(series, occurrence);
}
