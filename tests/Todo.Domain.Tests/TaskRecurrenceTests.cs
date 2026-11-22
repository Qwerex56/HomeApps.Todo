using Todo.Domain.Common;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Domain.Tests;

public sealed class TaskRecurrenceTests
{
    private readonly FixedTime _time = new(A.Now);

    private (TodoTask Task, TodoList List, Actor By) Recurring()
    {
        var h = new AHousehold(_time);
        var task = TodoTask.Create(h.Shared, h.Member, "Śmieci", null, new TaskDue(A.Today), TaskPriority.Normal, null, 1m, _time).Value;
        Assert.True(task.EditRecurring(h.Shared, h.Member, RecurrenceScope.All, "Śmieci", null, task.Due, TaskPriority.Normal,
            "FREQ=WEEKLY", RecurrenceAnchor.FromDueDate, _time).IsSuccess);
        return (task, h.Shared, h.Member);
    }

    [Fact]
    public void All_SetsRuleAndSeries()
    {
        var (task, _, _) = Recurring();

        Assert.Equal(("FREQ=WEEKLY", RecurrenceAnchor.FromDueDate, task.Id), (task.Rule!.Value, task.RecurrenceAnchor, task.SeriesId));
    }

    [Fact]
    public void Skip_OpensTheNextInstanceFromTheDueDateWithoutCompleting()
    {
        var (task, _, _) = Recurring();

        var next = task.Skip(_time);

        Assert.Equal((A.Today.AddDays(7), TodoStatus.Open, task.SeriesId), (next!.Due!.Date, next.Status, next.SeriesId));
        Assert.Equal(TodoStatus.Open, task.Status);
    }

    [Fact]
    public void ThisInstance_LeavesTheSeries()
    {
        var (task, list, by) = Recurring();

        task.EditRecurring(list, by, RecurrenceScope.ThisInstance, "Śmieci raz", null, null, TaskPriority.High, "FREQ=DAILY",
            RecurrenceAnchor.FromDueDate, _time);

        Assert.Equal((null, null, "Śmieci raz"), (task.SeriesId, task.Rule, task.Title.Value));
    }

    [Fact]
    public void ThisAndFuture_StartsANewSeries()
    {
        var (task, list, by) = Recurring();
        var old = task.SeriesId;

        task.EditRecurring(list, by, RecurrenceScope.ThisAndFuture, "Śmieci", null, null, TaskPriority.Normal, "FREQ=MONTHLY",
            RecurrenceAnchor.FromCompletionDate, _time);

        Assert.NotEqual(old, task.SeriesId);
        Assert.Equal(("FREQ=MONTHLY", RecurrenceAnchor.FromCompletionDate), (task.Rule!.Value, task.RecurrenceAnchor));
    }

    [Fact]
    public void InvalidRuleOrScope_ChangesNothing()
    {
        var (task, list, by) = Recurring();

        var badRule = task.EditRecurring(list, by, RecurrenceScope.All, "Nowy", null, null, TaskPriority.Normal, "nope",
            RecurrenceAnchor.FromDueDate, _time);
        var badScope = task.EditRecurring(list, by, (RecurrenceScope)9, "Nowy", null, null, TaskPriority.Normal, null,
            RecurrenceAnchor.FromDueDate, _time);

        Assert.Equal(("planner.invalid_rule", "planner.invalid_scope", "Śmieci"), (badRule.Code(), badScope.Code(), task.Title.Value));
    }

    [Fact]
    public void Complete_RemembersWhenARecurringTaskWasDone()
    {
        var (task, list, by) = Recurring();

        task.Complete(list, by, _time);

        Assert.Equal(A.Now, task.LastRecurrenceCompletedAt);
    }
}
