using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Tasks.UpdateTask;

/// <summary>The whole form: title, notes, due, priority and assignee.</summary>
public sealed record UpdateTaskCommand(Guid HouseholdId, Guid TaskId, TaskInput Task) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
