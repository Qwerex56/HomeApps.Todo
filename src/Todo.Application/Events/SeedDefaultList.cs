using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Application.Abstractions;
using Todo.Domain.Lists;

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
