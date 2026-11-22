using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Households.MemberRoleChanged;

/// <summary>A member's role changed; the projection (and so the feed's guest filter) follows. Sent by the consumer, without a user.</summary>
public sealed record MemberRoleChangedCommand(Guid HouseholdId, Guid UserId, string Role) : ICommand;
