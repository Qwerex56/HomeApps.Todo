using HomeApps.Plugins.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application;
using Todo.Application.Common;
using Todo.Infrastructure.Jobs;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Postgres with the outbox and the plugin switch, the repositories, the consumers of Households' events and the T5 clean-up.</summary>
    public static IServiceCollection AddTodoInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHomeAppsPluginInfrastructure<TodoDbContext>(configuration, TodoPlugin.Manifest, typeof(DependencyInjection).Assembly);

        services.AddScoped<ITodoListRepository, TodoListRepository>();
        services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
        services.AddScoped<ITodoReader, TodoReader>();
        services.AddHostedService<LeftListsCleanup>();

        return services;
    }
}
