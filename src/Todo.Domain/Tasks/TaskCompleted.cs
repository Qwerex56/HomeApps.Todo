using HomeApps.BuildingBlocks.Domain;

namespace Todo.Domain.Tasks;

/// <summary>A task was ticked off. Mapped to the <c>TaskCompleted</c> contract for tasks on shared lists only (private stays private).</summary>
public sealed record TaskCompleted(
    Guid TaskId,
    Guid HouseholdId,
    Guid ListId,
    bool OnSharedList,
    string Title,
    Guid CompletedBy,
    DateTimeOffset CompletedAt) : IDomainEvent;
