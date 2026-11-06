using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Application.Common;

public sealed record ListView(
    Guid Id,
    string Name,
    ListVisibility Visibility,
    Guid OwnerId,
    bool Archived,
    int OpenTasks,
    DateTimeOffset CreatedAt);

/// <summary>People are ids only; the web app adds names from the household's member list (plan 4.2).</summary>
public sealed record TaskView(
    Guid Id,
    Guid ListId,
    string Title,
    string? Notes,
    DateOnly? DueDate,
    TimeOnly? DueTime,
    Guid? AssigneeId,
    TaskPriority Priority,
    TodoStatus Status,
    decimal Position,
    Guid CreatedBy,
    DateTimeOffset CreatedAt,
    Guid? CompletedBy,
    DateTimeOffset? CompletedAt);
