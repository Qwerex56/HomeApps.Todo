using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Households.HouseholdArchived;

/// <summary>Sent by the consumer of Households' <c>HouseholdArchived</c> event, without a user. Idempotent.</summary>
public sealed record HouseholdArchivedCommand(Guid HouseholdId) : ICommand;
