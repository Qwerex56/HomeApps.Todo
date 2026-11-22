using HomeApps.Plugins.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Application.ExternalCalendars;
using Todo.Infrastructure.ExternalCalendars;
using Todo.Infrastructure.Feeds;
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
        services.AddScoped<ICalendarEventRepository, CalendarEventRepository>();
        services.AddScoped<IEventSeriesRepository, EventSeriesRepository>();
        services.AddScoped<ICalendarReader, CalendarReader>();
        services.AddScoped<IReminderReader, ReminderReader>();
        services.AddScoped<ICalendarSubscriptionRepository, CalendarSubscriptionRepository>();
        services.AddScoped<IPlannerSettingsRepository, PlannerSettingsRepository>();
        services.AddScoped<IExternalCalendarRepository, ExternalCalendarRepository>();
        services.AddSingleton<ISsrfSafeClient, SsrfSafeHttpClient>();
        services.AddScoped<ExternalCalendarSyncer>();
        services.AddScoped<IInactiveMarkerRepository, InactiveMarkerRepository>();
        services.AddSingleton<ICalendarFeedGenerator, IcsFeedGenerator>();
        services.AddHostedService<LeftListsCleanup>();
        services.AddHostedService<ExternalCalendarSyncJob>();
        services.AddHostedService<CalendarReminderJob>();

        return services;
    }
}
