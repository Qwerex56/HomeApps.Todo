using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Households.MemberLeft;

/// <summary>
/// Plan 4.2 and T5: someone left a household. Their assignments go; their private lists there are hidden and
/// deleted after <see cref="Domain.Lists.TodoList.KeptAfterOwnerLeft"/> unless they come back.
/// Sent by the consumer of Households' events, without a user, so it is not an <c>IHouseholdRequest</c>.
/// </summary>
public sealed record MemberLeftCommand(Guid HouseholdId, Guid UserId) : ICommand;
