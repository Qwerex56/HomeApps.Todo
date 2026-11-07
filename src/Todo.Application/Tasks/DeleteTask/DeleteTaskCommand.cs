using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Tasks.DeleteTask;

/// <summary>A hard delete: on a shared list by its author or an admin, on a private list by its owner.</summary>
public sealed record DeleteTaskCommand(Guid HouseholdId, Guid TaskId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
