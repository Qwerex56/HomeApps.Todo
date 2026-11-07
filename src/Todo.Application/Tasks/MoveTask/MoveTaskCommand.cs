using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Tasks.MoveTask;

/// <summary>
/// Drag and drop: puts the task on <c>ListId</c> right before <c>BeforeTaskId</c>, or at the end when it is null.
/// Only the moved task changes: its position becomes the midpoint between its new neighbours.
/// </summary>
public sealed record MoveTaskCommand(Guid HouseholdId, Guid TaskId, Guid ListId, Guid? BeforeTaskId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
