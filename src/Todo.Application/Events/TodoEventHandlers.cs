using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Application.Common;
using Todo.Domain.Lists;
using Contracts = HomeApps.Todo.Contracts;
using DomainEvents = Todo.Domain.Tasks;

namespace Todo.Application.Events;

/// <summary>The first switch-on in a household creates its shared list "Dom", in the same transaction.</summary>
internal sealed class SeedDefaultList(ITodoListRepository lists, TimeProvider time) : IDomainEventHandler<PluginSwitchedOn>
{
    public Task Handle(DomainEventNotification<PluginSwitchedOn> notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        var e = notification.DomainEvent;
        if (e.FirstTime)
        {
            lists.Add(TodoList.CreateDefault(e.HouseholdId, e.By, time));
        }

        return Task.CompletedTask;
    }
}

/// <summary>Tasks done on shared lists go out as <see cref="Contracts.TaskCompleted"/>; private ones stay inside.</summary>
internal sealed class PublishTaskCompleted(IEventBus bus) : IDomainEventHandler<DomainEvents.TaskCompleted>
{
    public Task Handle(DomainEventNotification<DomainEvents.TaskCompleted> notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        var e = notification.DomainEvent;
        return e.OnSharedList
            ? bus.PublishAsync(new Contracts.TaskCompleted(e.TaskId, e.HouseholdId, e.ListId, e.Title, e.CompletedBy, e.CompletedAt), cancellationToken)
            : Task.CompletedTask;
    }
}
