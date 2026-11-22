using HomeApps.BuildingBlocks.Api.Endpoints;
using MediatR;
using Todo.Application.Subscriptions.GetCalendarFeed;

namespace Todo.Api.Endpoints;

/// <summary>The public ICS feed (ADR 3.5). Outside the <c>/api</c> group and anonymous, as phone calendars send no token.</summary>
internal sealed class FeedEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/feed/{token}.ics", async (string token, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetCalendarFeedQuery(token), ct);
                return result.IsSuccess ? Results.Content(result.Value, "text/calendar; charset=utf-8") : Results.NotFound();
            })
            .AllowAnonymous()
            .WithName("GetCalendarFeedIcs");
}
