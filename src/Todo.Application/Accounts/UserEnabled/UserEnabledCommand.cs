using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Accounts.UserEnabled;

/// <summary>Sent by the consumer of Accounts' <c>UserEnabled</c> event, without a user. Idempotent.</summary>
public sealed record UserEnabledCommand(Guid UserId) : ICommand;
