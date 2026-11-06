using System.Collections.Concurrent;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Testing;
using HomeApps.Contracts;
using HomeApps.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Todo.Infrastructure.Persistence;

namespace Todo.Api.Tests;

/// <summary>The Todo plugin on a real Postgres with its migrations, Households faked at the HTTP level.</summary>
public sealed class TodoFactory : HomeAppsApiFactory<Program, TodoDbContext>
{
    public FakeHouseholds Households { get; } = new();

    public MovableTime Time { get; } = new();

    /// <summary>
    /// Every message handed to the outbox. The harness sees outbox deliveries only as serialized bodies, so the tests
    /// read what the plugin published here; the real bus still gets every message.
    /// </summary>
    public ConcurrentQueue<IIntegrationEvent> Published { get; } = new();

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient(nameof(IHouseholdAccess)).ConfigurePrimaryHttpMessageHandler(() => Households);
        services.AddSingleton<TimeProvider>(Time);

        var real = services.Last(d => d.ServiceType == typeof(IEventBus));
        services.Remove(real);
        services.AddScoped(real.ImplementationType!);
        services.AddScoped<IEventBus>(sp => new RecordingEventBus((IEventBus)sp.GetRequiredService(real.ImplementationType!), Published));
    }

    private sealed class RecordingEventBus(IEventBus inner, ConcurrentQueue<IIntegrationEvent> published) : IEventBus
    {
        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : class, IIntegrationEvent
        {
            published.Enqueue(message);
            return inner.PublishAsync(message, cancellationToken);
        }
    }
}
