using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Tasks.CompleteTask;

public sealed record CompleteTaskCommand(Guid HouseholdId, Guid TaskId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
