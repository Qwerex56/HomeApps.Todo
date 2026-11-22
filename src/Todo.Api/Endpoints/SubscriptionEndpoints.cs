using HomeApps.BuildingBlocks.Api.Endpoints;
using HomeApps.BuildingBlocks.Api.Results;
using MediatR;
using Todo.Application.Subscriptions;
using Todo.Application.Subscriptions.GetSubscriptions;
using Todo.Application.Subscriptions.RevokeSubscription;

namespace Todo.Api.Endpoints;

/// <summary>The caller's ICS feed links in one household (ADR 3.3).</summary>
internal sealed class SubscriptionEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/households/{householdId:guid}/calendar/subscriptions").WithTags("Planner");

        group.MapGet("", async (Guid householdId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetSubscriptionsQuery(householdId), ct)).ToHttpResult())
            .Produces<IReadOnlyList<CalendarSubscriptionView>>();

        group.MapPost("", async (Guid householdId, CreateSubscriptionRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(body.ToCommand(householdId), ct))
            .ToCreatedHttpResult(created => $"/plugins/todo/api/households/{householdId}/calendar/subscriptions/{created.Subscription.Id}"))
            .Produces<CreatedSubscriptionView>(StatusCodes.Status201Created);

        group.MapDelete("/{id:guid}", async (Guid householdId, Guid id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new RevokeSubscriptionCommand(householdId, id), ct)).ToHttpResult());
    }
}
