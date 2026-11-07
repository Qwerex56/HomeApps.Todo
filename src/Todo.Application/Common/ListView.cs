using Todo.Domain.Lists;

namespace Todo.Application.Common;

public sealed record ListView(
    Guid Id,
    string Name,
    ListVisibility Visibility,
    Guid OwnerId,
    bool Archived,
    int OpenTasks,
    DateTimeOffset CreatedAt);
