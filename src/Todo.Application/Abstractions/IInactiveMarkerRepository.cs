namespace Todo.Application.Abstractions;

/// <summary>Stores the <see cref="Todo.Domain.Households.InactiveMarker"/> facts. Saving is the unit of work's job.</summary>
public interface IInactiveMarkerRepository
{
    /// <summary>Adds the marker unless it exists. <c>(household, null)</c> archived, <c>(null, user)</c> disabled or erased, both: left.</summary>
    Task MarkAsync(Guid? householdId, Guid? userId, CancellationToken cancellationToken);

    Task ClearAsync(Guid? householdId, Guid? userId, CancellationToken cancellationToken);

    /// <summary>True when the household is archived, the account is disabled or erased, or the user left the household.</summary>
    Task<bool> IsInactiveAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);
}
