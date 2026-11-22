using HomeApps.BuildingBlocks.Api.Endpoints;
using HomeApps.BuildingBlocks.Api.Results;
using MediatR;
using Todo.Application.ExternalCalendars;
using Todo.Application.ExternalCalendars.GetExternalCalendars;
using Todo.Application.ExternalCalendars.RemoveExternalCalendar;
using Todo.Application.ExternalCalendars.SyncExternalCalendar;

namespace Todo.Api.Endpoints;

/// <summary>The caller's imported external calendars in one household (ADR 3.3).</summary>
internal sealed class ExternalCalendarEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/households/{householdId:guid}/calendar/external").WithTags("Planner");

        group.MapGet("", async (Guid householdId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetExternalCalendarsQuery(householdId), ct)).ToHttpResult())
            .Produces<IReadOnlyList<ExternalCalendarView>>();

        group.MapPost("", async (Guid householdId, AddExternalCalendarRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(body.ToCommand(householdId), ct))
            .ToCreatedHttpResult(created => $"/plugins/todo/api/households/{householdId}/calendar/external/{created.Id}"))
            .Produces<ExternalCalendarView>(StatusCodes.Status201Created);

        group.MapPost("/{id:guid}/sync", async (Guid householdId, Guid id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new SyncExternalCalendarCommand(householdId, id), ct)).ToHttpResult())
            .Produces<ExternalCalendarView>();

        group.MapDelete("/{id:guid}", async (Guid householdId, Guid id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new RemoveExternalCalendarCommand(householdId, id), ct)).ToHttpResult());
    }
}
