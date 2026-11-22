using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.Contracts.Households;
using HomeApps.Plugins;
using Todo.Application;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Application.Tasks;
using Todo.Application.Tasks.UpdateTask;
using Todo.Domain.Common;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Api.Tests;

/// <summary>No Docker needed: the handler runs against in-memory repositories.</summary>
public sealed class UpdateTaskScopeHandlerTests
{
    private readonly MovableTime _time = new();

    [Fact]
    public async Task ThisInstance_detaches_the_task_and_opens_the_next_instance()
    {
        var user = Guid.NewGuid();
        var household = Guid.NewGuid();
        var list = TodoList.CreateDefault(household, user, _time);
        var today = DateOnly.FromDateTime(_time.GetUtcNow().UtcDateTime);
        var task = TodoTask.Create(list, Actor.Manager(user), "Smieci", null, new TaskDue(today), TaskPriority.Normal, null, 1m, _time).Value;
        Assert.True(task.EditRecurring(list, Actor.Manager(user), RecurrenceScope.All, "Smieci", null, task.Due, TaskPriority.Normal,
            "FREQ=DAILY", RecurrenceAnchor.FromDueDate, _time).IsSuccess);
        var tasks = new Tasks(task);
        var handler = new UpdateTaskHandler(new Lists(list), tasks, new Actors(new Access(), new User(user), TodoPlugin.Manifest), _time);

        var input = new TaskInput("Raz", null, today, null, TaskPriority.Normal, null);
        var result = await handler.Handle(new UpdateTaskCommand(household, task.Id, input, "ThisInstance"), default);

        Assert.True(result.IsSuccess);
        Assert.False(task.IsRecurring);
        var next = Assert.Single(tasks.Added);
        Assert.True(next.IsRecurring);
    }

    private sealed class Access : IHouseholdAccess
    {
        public Task<HouseholdMembership?> GetMembershipAsync(Guid householdId, CancellationToken ct) =>
            Task.FromResult<HouseholdMembership?>(new HouseholdMembership(HouseholdRole.Member, DateTimeOffset.UnixEpoch, null));
    }

    private sealed class User(Guid id) : ICurrentUser
    {
        public bool IsAuthenticated => true;

        public Guid? UserId => id;

        public string? Email => null;

        public DateTimeOffset? AuthenticatedAt => null;

        public IReadOnlyCollection<string> Roles => [];

        public bool IsInRole(string role) => false;

        public Guid RequiredUserId => id;
    }

    private sealed class Lists(TodoList list) : ITodoListRepository
    {
        public Task<TodoList?> GetAsync(Guid householdId, Guid listId, CancellationToken ct) => Task.FromResult<TodoList?>(list);

        public Task<IReadOnlyList<TodoList>> ListPrivateOfAsync(Guid householdId, Guid ownerId, CancellationToken ct) => Task.FromResult<IReadOnlyList<TodoList>>([]);

        public Task<IReadOnlyList<TodoList>> ListDueForPurgeAsync(DateTimeOffset now, CancellationToken ct) => Task.FromResult<IReadOnlyList<TodoList>>([]);

        public void Add(TodoList l)
        {
        }

        public void Remove(TodoList l)
        {
        }
    }

    private sealed class Tasks(TodoTask task) : ITodoTaskRepository
    {
        public List<TodoTask> Added { get; } = [];

        public Task<TodoTask?> GetAsync(Guid householdId, Guid taskId, CancellationToken ct) => Task.FromResult<TodoTask?>(task);

        public Task<IReadOnlyList<TodoTask>> ListAssignedToAsync(Guid householdId, Guid userId, CancellationToken ct) => Task.FromResult<IReadOnlyList<TodoTask>>([]);

        public Task<decimal?> LastPositionAsync(Guid listId, Guid? exceptTaskId, CancellationToken ct) => Task.FromResult<decimal?>(null);

        public Task<decimal?> PositionBeforeAsync(Guid listId, decimal position, Guid exceptTaskId, CancellationToken ct) => Task.FromResult<decimal?>(null);

        public Task<bool> HasOpenInSeriesAsync(Guid householdId, Guid seriesId, Guid exceptTaskId, CancellationToken ct) => Task.FromResult(false);

        public void Add(TodoTask t) => Added.Add(t);

        public void Remove(TodoTask t)
        {
        }
    }
}
