using System.Net;
using System.Net.Http.Json;

namespace Todo.Api.Tests;

/// <summary>Needs Docker (Testcontainers Postgres). PUT and DELETE /tasks/{id}?scope= (ADR 3.7).</summary>
public sealed class TaskScopeTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    private static async Task<Guid> RecurringAsync(AHousehold h)
    {
        var list = await h.HomeListAsync();
        return await h.AddTaskAsync(h.Member, list.Id, "Smieci", new { dueDate = "2030-03-10", rrule = "FREQ=DAILY" });
    }

    private static object Body(string title) => new { title, priority = "Normal", dueDate = "2030-03-10", rrule = "FREQ=WEEKLY" };

    [Theory]
    [InlineData("ThisInstance")]
    [InlineData("ThisAndFuture")]
    [InlineData("All")]
    public async Task Put_with_a_scope_succeeds(string scope)
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await RecurringAsync(h);

        var response = await h.As(h.Member).PutAsJsonAsync($"{h.Api}/tasks/{id}?scope={scope}", Body("Nowa"));

        Assert.True(response.IsSuccessStatusCode, response.StatusCode.ToString());
    }

    [Fact]
    public async Task Put_this_instance_keeps_the_series_going()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await RecurringAsync(h);

        await h.As(h.Member).PutAsJsonAsync($"{h.Api}/tasks/{id}?scope=ThisInstance", Body("Raz"));

        var titles = (await h.TasksAsync(h.Member)).Select(t => t.Title).ToList();
        Assert.Contains("Smieci", titles);
    }

    [Theory]
    [InlineData("ThisInstance")]
    [InlineData("ThisAndFuture")]
    [InlineData("All")]
    public async Task Delete_with_a_scope_is_204(string scope)
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await RecurringAsync(h);

        var response = await h.As(h.Member).DeleteAsync($"{h.Api}/tasks/{id}?scope={scope}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task A_bad_scope_is_400_planner_invalid_scope()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await RecurringAsync(h);

        var put = await h.As(h.Member).PutAsJsonAsync($"{h.Api}/tasks/{id}?scope=Nope", Body("X"));
        var delete = await h.As(h.Member).DeleteAsync($"{h.Api}/tasks/{id}?scope=Nope");

        Assert.Equal((HttpStatusCode.BadRequest, HttpStatusCode.BadRequest), (put.StatusCode, delete.StatusCode));
        Assert.Equal("planner.invalid_scope", await AHousehold.CodeOf(put));
        Assert.Equal("planner.invalid_scope", await AHousehold.CodeOf(delete));
    }
}
