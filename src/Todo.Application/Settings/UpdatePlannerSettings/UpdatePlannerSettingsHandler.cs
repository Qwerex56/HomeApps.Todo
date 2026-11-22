using FluentResults;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;
using Todo.Domain.Settings;

namespace Todo.Application.Settings.UpdatePlannerSettings;

internal sealed class UpdatePlannerSettingsHandler(IPlannerSettingsRepository settings, ICurrentUser user, TimeProvider time)
    : ICommandHandler<UpdatePlannerSettingsCommand, PlannerSettingsView>
{
    public async Task<Result<PlannerSettingsView>> Handle(UpdatePlannerSettingsCommand command, CancellationToken cancellationToken)
    {
        var existing = await settings.GetAsync(command.HouseholdId, user.RequiredUserId, cancellationToken);
        var target = existing ?? new PlannerSettings(command.HouseholdId, user.RequiredUserId, time.GetUtcNow());
        var updated = target.Update(command.EventReminders, command.TaskReminders, command.FirstDayOfWeek, time, command.TimeZone);
        if (updated.IsFailed)
        {
            return updated.ToResult<PlannerSettingsView>();
        }

        if (existing is null)
        {
            settings.Add(target);
        }

        return Result.Ok(PlannerSettingsView.Of(target));
    }
}
