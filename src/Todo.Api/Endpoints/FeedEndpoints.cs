using MediatR;
using Todo.Application.Subscriptions.GetCalendarFeed;

namespace Todo.Api.Endpoints;

/// <summary>
/// The public ICS feed (ADR 3.5). Outside the <c>/api</c> group and anonymous, as phone calendars send no token. Not an
/// <c>IEndpoint</c>: the kit maps those under <c>/api</c>, so <c>Program</c> maps this one directly.
/// </summary>
internal static class FeedEndpoints
{
    public static IEndpointRouteBuilder MapFeed(this IEndpointRouteBuilder app)
    {
        app.MapGet("/feed/{token}.ics", async (string token, HttpContext http, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetCalendarFeedQuery(token), ct);
                if (result.IsFailed)
                {
                    return Results.NotFound();
                }

                http.Response.Headers.CacheControl = "private, no-store";
                http.Response.Headers["Referrer-Policy"] = "no-referrer";
                return Results.Content(result.Value, "text/calendar; charset=utf-8");
            })
            .AllowAnonymous()
            .WithName("GetCalendarFeedIcs");
        return app;
    }
}
