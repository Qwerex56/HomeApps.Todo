using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Tasks.DeleteTask;

/// <summary>A hard delete (<c>ThisInstance</c> on a repeating task skips the date and opens the next one; ADR 3.7): on a shared list by its author or an admin, on a private list by its owner.</summary>
public sealed record DeleteTaskCommand(Guid HouseholdId, Guid TaskId, string? Scope = null) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
