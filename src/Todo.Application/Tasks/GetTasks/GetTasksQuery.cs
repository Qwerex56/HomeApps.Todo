using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Application.Common;
using Todo.Domain.Tasks;

namespace Todo.Application.Tasks.GetTasks;

/// <summary>
/// Tasks on the lists the caller may see. <c>AssignedToMe</c> and <c>DueBy</c> make "moje" and "na dziś"; the day
/// comes from the client because only it knows the household's "today".
/// </summary>
public sealed record GetTasksQuery(Guid HouseholdId, Guid? ListId, bool AssignedToMe, DateOnly? DueBy, TodoStatus? Status)
    : IQuery<IReadOnlyList<TaskView>>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}
