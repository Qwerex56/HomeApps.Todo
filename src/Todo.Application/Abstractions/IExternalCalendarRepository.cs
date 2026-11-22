using Todo.Domain.Events;

namespace Todo.Application.Abstractions;

/// <summary>Loads and stores <see cref="ExternalCalendarImport"/>. Saving is the unit of work's job.</summary>
public interface IExternalCalendarRepository
{
    Task<ExternalCalendarImport?> GetAsync(Guid householdId, Guid id, CancellationToken cancellationToken);

    /// <summary>The user's own imports in one household, paused ones left out.</summary>
    Task<IReadOnlyList<ExternalCalendarImport>> ListOfUserAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);

    /// <summary>Every import of the user, in any household, paused ones included (for <c>MemberLeft</c>, <c>MemberJoined</c>, <c>UserErased</c>).</summary>
    Task<IReadOnlyList<ExternalCalendarImport>> ListAllOfUserAsync(Guid? householdId, Guid userId, CancellationToken cancellationToken);

    /// <summary>Imports the viewer may see in a household: their own, and others' that are not "only me". Paused ones left out. Not tracked.</summary>
    Task<IReadOnlyList<ExternalCalendarImport>> ListVisibleAsync(Guid householdId, Guid viewerId, CancellationToken cancellationToken);

    /// <summary>Imports whose owner has not left and whose last attempt is older than <see cref="ExternalCalendarImport.SyncInterval"/>.</summary>
    Task<IReadOnlyList<ExternalCalendarImport>> ListDueForSyncAsync(DateTimeOffset now, CancellationToken cancellationToken);

    /// <summary>Imports whose owner left at least <see cref="ExternalCalendarImport.KeptAfterOwnerLeft"/> before <paramref name="now"/>.</summary>
    Task<IReadOnlyList<ExternalCalendarImport>> ListDueForPurgeAsync(DateTimeOffset now, CancellationToken cancellationToken);

    void Add(ExternalCalendarImport import);

    void Remove(ExternalCalendarImport import);
}
