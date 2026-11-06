using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Households.MemberJoined;

/// <summary>T5: someone joined (again); private lists they left behind here come back. Sent by the consumer of Households' events.</summary>
public sealed record MemberJoinedCommand(Guid HouseholdId, Guid UserId) : ICommand;
