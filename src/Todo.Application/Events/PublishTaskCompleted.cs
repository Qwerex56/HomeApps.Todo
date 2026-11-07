using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using Contracts = HomeApps.Todo.Contracts;
using DomainEvents = Todo.Domain.Tasks;

namespace Todo.Application.Events;

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
