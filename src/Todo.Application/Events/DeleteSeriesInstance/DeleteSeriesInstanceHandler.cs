using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Common;
using Todo.Domain.Events;
using Todo.Domain.Tasks;

namespace Todo.Application.Events.DeleteSeriesInstance;

internal sealed class DeleteSeriesInstanceHandler(IEventSeriesRepository seriesRepository, Actors actors, TimeProvider time)
    : ICommandHandler<DeleteSeriesInstanceCommand>
{
    public async Task<Result> Handle(DeleteSeriesInstanceCommand command, CancellationToken cancellationToken)
    {
        var scope = ScopeParser.Parse(command.Scope);
        if (scope.IsFailed)
        {
            return scope.ToResult();
        }

        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var series = await seriesRepository.GetAsync(command.HouseholdId, command.SeriesId, cancellationToken);
        if (series is null || !series.CanSee(actor) || series.OccurrenceOn(command.Date, null) is not { } occurrence)
        {
            return Result.Fail(PlannerErrors.SeriesNotFound);
        }

        var allowed = series.CanDelete(actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        if (scope.Value == RecurrenceScope.All || (scope.Value == RecurrenceScope.ThisAndFuture && command.Date <= series.StartDate))
        {
            seriesRepository.Remove(series);
            return Result.Ok();
        }

        var overrides = await seriesRepository.OverridesAsync(series.Id, cancellationToken);
        if (scope.Value == RecurrenceScope.ThisAndFuture)
        {
            seriesRepository.RemoveOverrides(overrides.Where(o => o.OriginalInstanceDate >= command.Date));
            return series.EndAt(occurrence.StartUtc.AddSeconds(-1), time);
        }

        if (overrides.FirstOrDefault(o => o.OriginalInstanceDate == command.Date) is { } existing)
        {
            existing.CancelOccurrence(time);
        }
        else
        {
            seriesRepository.AddOverride(EventOverride.Cancel(series.Id, command.Date, time));
        }

        return Result.Ok();
    }
}
