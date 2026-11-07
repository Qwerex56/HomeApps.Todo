using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Domain.Lists;

namespace Todo.Application.Lists.CreateList;

/// <summary>
/// Any member keeps private lists; a shared list needs <see cref="TodoPlugin.Manage"/> (the domain says
/// <c>list.forbidden</c>), so the request itself only asks for <see cref="TodoPlugin.Write"/>.
/// </summary>
public sealed record CreateListCommand(Guid HouseholdId, string Name, ListVisibility Visibility) : ICommand<Guid>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
