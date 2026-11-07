using Todo.Domain.Tasks;

namespace Todo.Application.Abstractions;

/// <param name="ListId">Only this list.</param>
/// <param name="AssigneeId">Only tasks assigned to this person.</param>
/// <param name="DueBy">Only open tasks due on this day or before (the household's "today" comes from the client).</param>
/// <param name="Status">Only open or only done tasks.</param>
public sealed record TaskFilter(Guid? ListId = null, Guid? AssigneeId = null, DateOnly? DueBy = null, TodoStatus? Status = null);
