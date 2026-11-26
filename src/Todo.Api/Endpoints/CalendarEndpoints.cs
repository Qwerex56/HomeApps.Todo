using HomeApps.BuildingBlocks.Api.Endpoints;
using HomeApps.BuildingBlocks.Api.Results;
using MediatR;
using Todo.Application.Events;
using Todo.Application.Events.DeleteCalendarEvent;
using Todo.Application.Events.DeleteSeriesInstance;
using Todo.Application.Events.GetCalendarEvent;
using Todo.Application.Events.GetCalendarItems;

namespace Todo.Api.Endpoints;

/// <summary>Calendar events and series of one household (ADR 3.3).</summary>
internal sealed class CalendarEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/households/{householdId:guid}/calendar").WithTags("Planner");

        group.MapGet("/events", async (Guid householdId, DateTimeOffset from, DateTimeOffset to, string? layers, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetCalendarItemsQuery(householdId, from, to, layers), ct)).ToHttpResult())
            .Produces<IReadOnlyList<CalendarItemView>>();

        group.MapPost("/events", async (Guid householdId, CreateCalendarEventRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(body.ToCommand(householdId), ct))
            .ToCreatedHttpResult(created => $"/plugins/todo/api/v1/households/{householdId}/calendar/events/{created.Id}"))
            .Produces<CalendarEventDetailView>(StatusCodes.Status201Created);

        group.MapGet("/events/{id:guid}", async (Guid householdId, Guid id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetCalendarEventQuery(householdId, id), ct)).ToHttpResult())
            .Produces<CalendarEventDetailView>();

        group.MapPut("/events/{id:guid}", async (Guid householdId, Guid id, UpdateCalendarEventRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(body.ToCommand(householdId, id), ct)).ToHttpResult())
            .Produces<CalendarEventDetailView>();

        group.MapDelete("/events/{id:guid}", async (Guid householdId, Guid id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new DeleteCalendarEventCommand(householdId, id), ct)).ToHttpResult());

        group.MapPut("/events/{seriesId:guid}/instances/{date:regex(^\\d{{4}}-\\d{{2}}-\\d{{2}}$)}", async (
                Guid householdId, Guid seriesId, DateOnly date, ModifySeriesInstanceRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(body.ToCommand(householdId, seriesId, date), ct)).ToHttpResult())
            .Produces<CalendarEventDetailView>();

        group.MapDelete("/events/{seriesId:guid}/instances/{date:regex(^\\d{{4}}-\\d{{2}}-\\d{{2}}$)}", async (
                Guid householdId, Guid seriesId, DateOnly date, string? scope, ISender sender, CancellationToken ct) =>
            (await sender.Send(new DeleteSeriesInstanceCommand(householdId, seriesId, date, scope), ct)).ToHttpResult());
    }
}
