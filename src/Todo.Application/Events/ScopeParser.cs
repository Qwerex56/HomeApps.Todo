using FluentResults;
using Todo.Domain.Common;
using Todo.Domain.Tasks;

namespace Todo.Application.Events;

internal static class ScopeParser
{
    /// <summary>ThisInstance, ThisAndFuture or All (any case); anything else, or nothing, is <c>planner.invalid_scope</c>.</summary>
    public static Result<RecurrenceScope> Parse(string? scope) =>
        Enum.TryParse<RecurrenceScope>(scope, true, out var parsed) && Enum.IsDefined(parsed)
            ? parsed
            : Result.Fail<RecurrenceScope>(PlannerErrors.InvalidScope);
}
