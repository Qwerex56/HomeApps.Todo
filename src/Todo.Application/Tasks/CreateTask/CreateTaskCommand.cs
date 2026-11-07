using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Tasks.CreateTask;

/// <summary>
/// Adds a task at the end of a list. Who can be assigned on a shared list is not checked against Households: the web
/// app offers only members, and a stale assignment is cleared when the person leaves (<c>MemberLeft</c>).
/// </summary>
public sealed record CreateTaskCommand(Guid HouseholdId, Guid ListId, TaskInput Task) : ICommand<Guid>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
