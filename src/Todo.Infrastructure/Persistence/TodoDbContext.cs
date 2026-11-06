using HomeApps.Plugins.Hosting;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence;

/// <summary>The plugin's own database (<c>homeapps_todo</c>): lists, tasks and, from the kit, the switch per household.</summary>
public sealed class TodoDbContext(DbContextOptions<TodoDbContext> options, IPublisher publisher) : PluginDbContext(options, publisher)
{
    public DbSet<TodoList> Lists => Set<TodoList>();

    public DbSet<TodoTask> Tasks => Set<TodoTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
    }
}
