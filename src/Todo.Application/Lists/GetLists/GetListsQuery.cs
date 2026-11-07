using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Application.Common;

namespace Todo.Application.Lists.GetLists;

/// <summary>Shared lists and the caller's own private lists; private lists of others never show, not even to admins.</summary>
public sealed record GetListsQuery(Guid HouseholdId) : IQuery<IReadOnlyList<ListView>>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}
