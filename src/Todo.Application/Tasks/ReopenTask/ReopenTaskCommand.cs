using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Tasks.ReopenTask;

public sealed record ReopenTaskCommand(Guid HouseholdId, Guid TaskId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
