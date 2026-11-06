using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Lists.DeleteList;

/// <summary>A hard delete with all its tasks (they are not account data).</summary>
public sealed record DeleteListCommand(Guid HouseholdId, Guid ListId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
