using HomeApps.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Common;

namespace Todo.Application;

public static class DependencyInjection
{
    /// <summary>MediatR with the kit's behaviors and the household permission check, validators, and <see cref="Actors"/>.</summary>
    public static IServiceCollection AddTodoApplication(this IServiceCollection services)
    {
        services.AddHomeAppsPluginApplication(TodoPlugin.Manifest, typeof(DependencyInjection).Assembly);
        services.AddScoped<Actors>();
        return services;
    }
}
