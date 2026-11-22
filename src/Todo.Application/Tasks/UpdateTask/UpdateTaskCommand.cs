using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Tasks.UpdateTask;

/// <summary>The whole form: title, notes, due, priority, assignee and repetition. <c>Scope</c> (ADR 3.7) applies to repeating tasks; none means All.</summary>
public sealed record UpdateTaskCommand(Guid HouseholdId, Guid TaskId, TaskInput Task, string? Scope = null) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
