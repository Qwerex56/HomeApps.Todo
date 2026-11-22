using HomeApps.BuildingBlocks.Api.Endpoints;
using HomeApps.BuildingBlocks.Api.Results;
using MediatR;
using Todo.Application.Settings;
using Todo.Application.Settings.GetPlannerSettings;

namespace Todo.Api.Endpoints;

/// <summary>The caller's own planner preferences in one household (ADR 3.3).</summary>
internal sealed class PlannerSettingsEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/households/{householdId:guid}/calendar/settings").WithTags("Planner");

        group.MapGet("", async (Guid householdId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetPlannerSettingsQuery(householdId), ct)).ToHttpResult())
            .Produces<PlannerSettingsView>();

        group.MapPut("", async (Guid householdId, UpdatePlannerSettingsRequest body, ISender sender, CancellationToken ct) =>
            (await sender.Send(body.ToCommand(householdId), ct)).ToHttpResult())
            .Produces<PlannerSettingsView>();
    }
}
