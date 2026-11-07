using Todo.Domain.Lists;

namespace Todo.Api.Endpoints;

internal sealed record CreateListRequest(string Name, ListVisibility Visibility);
