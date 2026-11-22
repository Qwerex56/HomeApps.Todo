using HomeApps.Plugins.Hosting;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Events;
using Todo.Domain.Households;
using Todo.Domain.Lists;
using Todo.Domain.Settings;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence;

/// <summary>The plugin's own database (<c>homeapps_todo</c>): lists, tasks and, from the kit, the switch per household.</summary>
public sealed class TodoDbContext(DbContextOptions<TodoDbContext> options, IPublisher publisher) : PluginDbContext(options, publisher)
{
    public DbSet<TodoList> Lists => Set<TodoList>();

    public DbSet<TodoTask> Tasks => Set<TodoTask>();

    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();

    public DbSet<EventSeries> EventSeries => Set<EventSeries>();

    public DbSet<EventOverride> EventOverrides => Set<EventOverride>();

    public DbSet<EventReminderOverride> EventReminderOverrides => Set<EventReminderOverride>();

    public DbSet<PlannerSettings> PlannerSettings => Set<PlannerSettings>();

    public DbSet<CalendarSubscription> CalendarSubscriptions => Set<CalendarSubscription>();

    public DbSet<ExternalCalendarImport> ExternalCalendarImports => Set<ExternalCalendarImport>();

    public DbSet<HouseholdMemberProjection> HouseholdMembers => Set<HouseholdMemberProjection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
    }
}
