using FluentResults;
using HomeApps.BuildingBlocks.Application.Messaging;
using Todo.Application.Abstractions;

namespace Todo.Application.Households.PurgeLeftLists;

internal sealed class PurgeLeftListsHandler(ITodoListRepository lists, IExternalCalendarRepository calendars, TimeProvider time) : ICommandHandler<PurgeLeftListsCommand, int>
{
    public async Task<Result<int>> Handle(PurgeLeftListsCommand command, CancellationToken cancellationToken)
    {
        var now = time.GetUtcNow();
        var due = await lists.ListDueForPurgeAsync(now, cancellationToken);
        foreach (var list in due)
        {
            lists.Remove(list);
        }

        var dueCalendars = await calendars.ListDueForPurgeAsync(now, cancellationToken);
        foreach (var calendar in dueCalendars)
        {
            calendars.Remove(calendar);
        }

        return due.Count + dueCalendars.Count;
    }
}
