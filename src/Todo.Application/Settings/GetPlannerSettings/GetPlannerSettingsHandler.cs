using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Settings.GetPlannerSettings;

internal sealed class GetPlannerSettingsHandler(IPlannerSettingsRepository settings, ICurrentUser user)
    : IQueryHandler<GetPlannerSettingsQuery, PlannerSettingsView>
{
    public async Task<Result<PlannerSettingsView>> Handle(GetPlannerSettingsQuery query, CancellationToken cancellationToken)
    {
        var found = await settings.GetAsync(query.HouseholdId, user.RequiredUserId, cancellationToken);
        return Result.Ok(found is null ? PlannerSettingsView.Default : PlannerSettingsView.Of(found));
    }
}
