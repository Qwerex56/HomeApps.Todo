using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Accounts.UserErased;

/// <summary>Sent by the consumer of Accounts' <c>UserErased</c> event, without a user. Idempotent.</summary>
public sealed record UserErasedCommand(Guid UserId) : ICommand;
