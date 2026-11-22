using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Accounts.UserDisabled;

/// <summary>Sent by the consumer of Accounts' <c>UserDisabled</c> event, without a user. Idempotent.</summary>
public sealed record UserDisabledCommand(Guid UserId) : ICommand;
