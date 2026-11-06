namespace HomeApps.Todo.Contracts;

/// <summary>
/// A task on a <b>shared</b> list was ticked off. Tasks on private lists never leave the plugin (D2a: members' data is
/// private). The title is included so a listener (a hub, JARVIS) can say what was done without calling back.
/// </summary>
public sealed record TaskCompleted(
    Guid TaskId,
    Guid HouseholdId,
    Guid ListId,
    string Title,
    Guid CompletedBy,
    DateTimeOffset CompletedAt) : HomeApps.Contracts.IIntegrationEvent;
