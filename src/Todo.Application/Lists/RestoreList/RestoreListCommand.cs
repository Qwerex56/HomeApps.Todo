using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Lists.RestoreList;

public sealed record RestoreListCommand(Guid HouseholdId, Guid ListId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
