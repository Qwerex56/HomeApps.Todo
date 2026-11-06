using Todo.Domain.Common;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Domain.Tests;

public sealed class TaskTests
{
    private readonly FixedTime _time = new(A.Now);

    private static TodoTask NewTask(TodoList list, Actor by, TimeProvider time, Guid? assignee = null, TaskDue? due = null) =>
        TodoTask.Create(list, by, "Kupić mleko", null, due, TaskPriority.Normal, assignee, 1m, time).Value;

    [Fact]
    public void Create_OnSharedList_ByAnyMember()
    {
        var h = new AHousehold(_time);
        var due = new TaskDue(A.Today, new TimeOnly(18, 0));

        var task = TodoTask.Create(h.Shared, h.Member, " Kupić mleko ", "  2 litry\npółtłuste ", due, TaskPriority.High,
            h.Other.UserId, 3.5m, _time).Value;

        Assert.Equal((h.Id, h.Shared.Id, "Kupić mleko", "2 litry\npółtłuste"), (task.HouseholdId, task.ListId, task.Title.Value, task.Notes!.Value));
        Assert.Equal((due, TaskPriority.High, (Guid?)h.Other.UserId, 3.5m), (task.Due, task.Priority, task.AssigneeId, task.Position));
        Assert.Equal((TodoStatus.Open, h.Member.UserId, A.Now), (task.Status, task.CreatedBy, task.CreatedAt));
        Assert.Empty(task.DomainEvents);
    }

    [Fact]
    public void Create_ReportsEveryInvalidField()
    {
        var h = new AHousehold(_time);

        var result = TodoTask.Create(h.Private, h.Member, "", new string('x', TaskNotes.MaxLength + 1), null,
            TaskPriority.Normal, h.Other.UserId, 0m, _time);

        Assert.Equal(["task.title_invalid", "task.notes_invalid", "task.assignee_invalid"],
            result.Errors.Select(e => (string)e.Metadata["Code"]).ToArray());
    }

    [Fact]
    public void Create_BlankNotesAreNoNotes()
    {
        var h = new AHousehold(_time);

        var task = TodoTask.Create(h.Shared, h.Member, "X", "   ", null, TaskPriority.Low, null, 0m, _time).Value;

        Assert.Null(task.Notes);
    }

    [Theory]
    [InlineData("member", null)]
    [InlineData("admin", "list.not_found")]
    [InlineData("other", "list.not_found")]
    public void Create_OnPrivateList_OnlyByOwner(string who, string? error)
    {
        var h = new AHousehold(_time);

        var result = TodoTask.Create(h.Private, h.Of(who), "X", null, null, TaskPriority.Normal, null, 0m, _time);

        Assert.Equal(error, result.IsSuccess ? null : result.Code());
    }

    [Fact]
    public void Create_OnArchivedList_IsRefused()
    {
        var h = new AHousehold(_time);
        h.Shared.Archive(h.Admin, _time);

        var result = TodoTask.Create(h.Shared, h.Admin, "X", null, null, TaskPriority.Normal, null, 0m, _time);

        Assert.Equal("list.archived", result.Code());
    }

    [Fact]
    public void Edit_ChangesContent_NotStatusOrPlace()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);
        _time.Advance(TimeSpan.FromMinutes(1));
        var due = new TaskDue(A.Today.AddDays(1));

        var result = task.Edit(h.Shared, h.Other, "Kupić chleb", "razowy", due, TaskPriority.Low, _time);

        Assert.True(result.IsSuccess);
        Assert.Equal(("Kupić chleb", "razowy", due, TaskPriority.Low, _time.Now), (task.Title.Value, task.Notes!.Value, task.Due, task.Priority, task.UpdatedAt));
        Assert.Equal((TodoStatus.Open, h.Shared.Id, 1m), (task.Status, task.ListId, task.Position));
    }

    [Fact]
    public void Edit_InvalidTitle_ChangesNothing()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);

        var result = task.Edit(h.Shared, h.Member, " ", "notatka", null, TaskPriority.High, _time);

        Assert.Equal("task.title_invalid", result.Code());
        Assert.Equal(("Kupić mleko", null, TaskPriority.Normal), (task.Title.Value, task.Notes, task.Priority));
    }

    [Fact]
    public void Edit_WithAnotherListThanTheTasks_IsAProgrammingError()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);

        Assert.Throws<ArgumentException>(() => task.Edit(h.Private, h.Member, "X", null, null, TaskPriority.Normal, _time));
    }

    [Fact]
    public void Assign_OnSharedList_AnyoneOrNobody()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);

        Assert.True(task.Assign(h.Shared, h.Member, h.Other.UserId, _time).IsSuccess);
        Assert.Equal(h.Other.UserId, task.AssigneeId);
        Assert.True(task.Assign(h.Shared, h.Member, null, _time).IsSuccess);
        Assert.Null(task.AssigneeId);
        Assert.Equal("task.assignee_invalid", task.Assign(h.Shared, h.Member, Guid.Empty, _time).Code());
    }

    [Fact]
    public void Assign_OnPrivateList_OnlyTheOwner()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Private, h.Member, _time);

        Assert.True(task.Assign(h.Private, h.Member, h.Member.UserId, _time).IsSuccess);
        Assert.Equal("task.assignee_invalid", task.Assign(h.Private, h.Member, h.Other.UserId, _time).Code());
        Assert.Equal(h.Member.UserId, task.AssigneeId);
    }

    [Fact]
    public void Complete_RecordsWhoAndWhen_AndRaisesTaskCompletedOnce()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);
        _time.Advance(TimeSpan.FromHours(1));

        Assert.True(task.Complete(h.Shared, h.Other, _time).IsSuccess);
        Assert.True(task.Complete(h.Shared, h.Member, _time).IsSuccess);

        Assert.Equal((TodoStatus.Done, (Guid?)h.Other.UserId, (DateTimeOffset?)_time.Now), (task.Status, task.CompletedBy, task.CompletedAt));
        var completed = Assert.IsType<TaskCompleted>(Assert.Single(task.DomainEvents));
        Assert.Equal(new TaskCompleted(task.Id, h.Id, h.Shared.Id, true, "Kupić mleko", h.Other.UserId, _time.Now), completed);
    }

    [Fact]
    public void Complete_OnPrivateList_IsMarkedPrivate()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Private, h.Member, _time);

        task.Complete(h.Private, h.Member, _time);

        Assert.False(Assert.IsType<TaskCompleted>(Assert.Single(task.DomainEvents)).OnSharedList);
    }

    [Fact]
    public void Reopen_ClearsCompletion_AndIsIdempotent()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);
        task.Complete(h.Shared, h.Member, _time);

        Assert.True(task.Reopen(h.Shared, h.Other, _time).IsSuccess);
        Assert.True(task.Reopen(h.Shared, h.Other, _time).IsSuccess);

        Assert.Equal((TodoStatus.Open, null, null), (task.Status, task.CompletedBy, task.CompletedAt));
    }

    [Fact]
    public void Complete_OnArchivedList_IsRefused()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);
        h.Shared.Archive(h.Admin, _time);

        Assert.Equal("list.archived", task.Complete(h.Shared, h.Member, _time).Code());
        Assert.Equal(TodoStatus.Open, task.Status);
    }

    [Fact]
    public void Move_WithinList_ChangesPosition()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);

        Assert.True(task.Move(h.Shared, h.Shared, h.Other, 0.5m, _time).IsSuccess);

        Assert.Equal((h.Shared.Id, 0.5m), (task.ListId, task.Position));
    }

    [Fact]
    public void Move_ToOwnPrivateList_DropsAssignmentToSomeoneElse()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time, assignee: h.Other.UserId);

        Assert.True(task.Move(h.Shared, h.Private, h.Member, 2m, _time).IsSuccess);

        Assert.Equal((h.Private.Id, 2m, (Guid?)null), (task.ListId, task.Position, task.AssigneeId));
    }

    [Fact]
    public void Move_KeepsAssignmentToThePrivateListsOwner()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time, assignee: h.Member.UserId);

        task.Move(h.Shared, h.Private, h.Member, 2m, _time);

        Assert.Equal(h.Member.UserId, task.AssigneeId);
    }

    [Fact]
    public void Move_ToSomeoneElsesPrivateList_OrAnArchivedOne_OrAnotherHousehold_IsRefused()
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Other, _time);
        var archived = TodoList.Create(h.Id, h.Admin, "Stara", ListVisibility.Shared, _time).Value;
        archived.Archive(h.Admin, _time);
        var elsewhere = TodoList.Create(Guid.NewGuid(), h.Admin, "Obca", ListVisibility.Shared, _time).Value;

        Assert.Equal("list.not_found", task.Move(h.Shared, h.Private, h.Other, 1m, _time).Code());
        Assert.Equal("list.archived", task.Move(h.Shared, archived, h.Other, 1m, _time).Code());
        Assert.Equal("list.not_found", task.Move(h.Shared, elsewhere, h.Admin, 1m, _time).Code());
        Assert.Equal(h.Shared.Id, task.ListId);
    }

    [Theory]
    [InlineData("member", null)]
    [InlineData("admin", null)]
    [InlineData("other", "task.forbidden")]
    public void Delete_OnSharedList_AuthorOrAdmin(string who, string? error)
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Shared, h.Member, _time);

        var result = task.CanDelete(h.Shared, h.Of(who));

        Assert.Equal(error, result.IsSuccess ? null : result.Code());
    }

    [Theory]
    [InlineData("member", null)]
    [InlineData("admin", "list.not_found")]
    public void Delete_OnPrivateList_OnlyOwner(string who, string? error)
    {
        var h = new AHousehold(_time);
        var task = NewTask(h.Private, h.Member, _time);

        var result = task.CanDelete(h.Private, h.Of(who));

        Assert.Equal(error, result.IsSuccess ? null : result.Code());
    }

    [Fact]
    public void AssigneeLeft_ClearsOnlyTheirAssignment()
    {
        var h = new AHousehold(_time);
        var theirs = NewTask(h.Shared, h.Member, _time, assignee: h.Other.UserId);
        var mine = NewTask(h.Shared, h.Member, _time, assignee: h.Member.UserId);

        theirs.AssigneeLeft(h.Other.UserId, _time);
        mine.AssigneeLeft(h.Other.UserId, _time);

        Assert.Null(theirs.AssigneeId);
        Assert.Equal(h.Member.UserId, mine.AssigneeId);
    }

    [Fact]
    public void IsDueBy_OpenTasksDueTodayOrOverdue()
    {
        var h = new AHousehold(_time);
        var overdue = NewTask(h.Shared, h.Member, _time, due: new TaskDue(A.Today.AddDays(-2)));
        var today = NewTask(h.Shared, h.Member, _time, due: new TaskDue(A.Today, new TimeOnly(23, 59)));
        var tomorrow = NewTask(h.Shared, h.Member, _time, due: new TaskDue(A.Today.AddDays(1)));
        var someday = NewTask(h.Shared, h.Member, _time);
        var done = NewTask(h.Shared, h.Member, _time, due: new TaskDue(A.Today));
        done.Complete(h.Shared, h.Member, _time);

        Assert.Equal([true, true, false, false, false],
            new[] { overdue, today, tomorrow, someday, done }.Select(t => t.IsDueBy(A.Today)).ToArray());
    }
}
