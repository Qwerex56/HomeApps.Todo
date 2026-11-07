using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Lists.RenameList;

/// <summary>Shared lists by admins, private ones by their owner (the domain decides).</summary>
public sealed record RenameListCommand(Guid HouseholdId, Guid ListId, string Name) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
