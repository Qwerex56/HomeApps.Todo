using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Households.HouseholdRestored;

/// <summary>Sent by the consumer of Households' <c>HouseholdRestored</c> event, without a user. Idempotent.</summary>
public sealed record HouseholdRestoredCommand(Guid HouseholdId) : ICommand;
