using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Domain.Tests;

public sealed class TaskNextInstanceTests
{
    private readonly FixedTime _time = new(A.Now);

    private (TodoTask Task, TodoList List, Domain.Common.Actor By) Done(string rule, RecurrenceAnchor anchor, TaskDue? due)
    {
        var h = new AHousehold(_time);
        var task = TodoTask.Create(h.Shared, h.Member, "Śmieci", "notatka", due, TaskPriority.High, h.Member.UserId, 1m, _time).Value;
        Assert.True(task.EditRecurring(h.Shared, h.Member, RecurrenceScope.All, "Śmieci", "notatka", due, TaskPriority.High, rule, anchor, _time).IsSuccess);
        task.Complete(h.Shared, h.Member, _time);
        return (task, h.Shared, h.Member);
    }

    [Fact]
    public void FromDueDate_CountsFromTheDueDate()
    {
        var (task, _, _) = Done("FREQ=WEEKLY", RecurrenceAnchor.FromDueDate, new TaskDue(A.Today.AddDays(-3), new TimeOnly(8, 0)));

        var next = task.NextInstance(_time)!;

        Assert.Equal(new TaskDue(A.Today.AddDays(4), new TimeOnly(8, 0)), next.Due);
    }

    [Fact]
    public void FromCompletionDate_CountsFromCompletion()
    {
        var (task, _, _) = Done("FREQ=DAILY;INTERVAL=3", RecurrenceAnchor.FromCompletionDate, new TaskDue(A.Today.AddDays(-10)));

        Assert.Equal(A.Today.AddDays(3), task.NextInstance(_time)!.Due!.Date);
    }

    [Fact]
    public void NextInstance_IsAnOpenCopyInTheSameSeries()
    {
        var (task, _, _) = Done("FREQ=DAILY", RecurrenceAnchor.FromDueDate, null);

        var next = task.NextInstance(_time)!;

        Assert.Equal((TodoStatus.Open, task.SeriesId, task.ListId, task.AssigneeId, TaskPriority.High, "Śmieci", null), (next.Status, next.SeriesId, next.ListId, next.AssigneeId, next.Priority, next.Title.Value, next.CompletedAt));
        Assert.NotEqual(task.Id, next.Id);
        Assert.Equal(A.Today.AddDays(1), next.Due!.Date);
    }

    [Fact]
    public void NextInstance_IsNullWhenFinishedRuleOrNotDone()
    {
        var (task, _, _) = Done("FREQ=DAILY;COUNT=1", RecurrenceAnchor.FromDueDate, new TaskDue(A.Today));
        Assert.Null(task.NextInstance(_time));

        var (open, openList, openBy) = Done("FREQ=DAILY", RecurrenceAnchor.FromDueDate, null);
        open.Reopen(openList, openBy, _time);
        Assert.Null(open.NextInstance(_time));
    }

    [Fact]
    public void NextInstance_IsNullForATaskThatDoesNotRepeat()
    {
        var h = new AHousehold(_time);
        var task = TodoTask.Create(h.Shared, h.Member, "Raz", null, null, TaskPriority.Normal, null, 1m, _time).Value;
        task.Complete(h.Shared, h.Member, _time);

        Assert.Null(task.NextInstance(_time));
    }

    [Fact]
    public void Count_ShrinksWithEachInstanceSoTheSeriesEnds()
    {
        var (task, list, by) = Done("FREQ=DAILY;COUNT=2", RecurrenceAnchor.FromDueDate, new TaskDue(A.Today));

        var second = task.NextInstance(_time)!;
        second.Complete(list, by, _time);

        Assert.Equal("FREQ=DAILY;COUNT=1", second.Rule!.Value);
        Assert.Null(second.NextInstance(_time));
    }
}
