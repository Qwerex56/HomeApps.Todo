using Microsoft.Extensions.DependencyInjection;
using Todo.Domain.Tasks;
using Todo.Infrastructure.Persistence;

namespace Todo.Api.Tests;

/// <summary>Needs Docker (Testcontainers Postgres).</summary>
public sealed class HasOpenInSeriesTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    [Fact]
    public async Task The_task_being_completed_and_the_root_are_handled_before_save()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        var rootId = await h.AddTaskAsync(h.Owner, home.Id, "Śmieci");
        var secondId = await h.AddTaskAsync(h.Owner, home.Id, "Śmieci");

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var repo = new TodoTaskRepository(db);
        var second = await db.Tasks.FindAsync(secondId);
        typeof(TodoTask).GetProperty(nameof(TodoTask.SeriesId))!.SetValue(second, rootId);
        await db.SaveChangesAsync();

        // Completing the second instance (still Open in the database): the root is open, so no new instance.
        Assert.True(await repo.HasOpenInSeriesAsync(h.Id, rootId, secondId, default));
        // Completing the root while the second is open: reopen case, no duplicate.
        Assert.True(await repo.HasOpenInSeriesAsync(h.Id, rootId, rootId, default));
        // Only the task itself is open: the next instance is needed.
        Assert.False(await repo.HasOpenInSeriesAsync(h.Id, secondId, secondId, default));
    }
}
