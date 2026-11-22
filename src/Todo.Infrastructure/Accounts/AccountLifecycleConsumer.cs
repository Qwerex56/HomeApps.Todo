using FluentResults;
using HomeApps.Contracts.Accounts;
using MassTransit;
using MediatR;
using Todo.Application.Accounts.UserDisabled;
using Todo.Application.Accounts.UserEnabled;
using Todo.Application.Accounts.UserErased;

namespace Todo.Infrastructure.Accounts;

/// <summary>
/// Follows the account's state (ADR decision 16). Each message only sends an idempotent command; a failed result throws,
/// so the message is retried. Public because MassTransit's scan finds public consumers only.
/// </summary>
public sealed class AccountLifecycleConsumer(ISender sender) : IConsumer<UserDisabled>, IConsumer<UserEnabled>, IConsumer<UserErased>
{
    public async Task Consume(ConsumeContext<UserDisabled> context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ThrowIfFailed(await sender.Send(new UserDisabledCommand(context.Message.UserId), context.CancellationToken), context.Message);
    }

    public async Task Consume(ConsumeContext<UserEnabled> context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ThrowIfFailed(await sender.Send(new UserEnabledCommand(context.Message.UserId), context.CancellationToken), context.Message);
    }

    public async Task Consume(ConsumeContext<UserErased> context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ThrowIfFailed(await sender.Send(new UserErasedCommand(context.Message.UserId), context.CancellationToken), context.Message);
    }

    private static void ThrowIfFailed(Result result, object message)
    {
        if (result.IsFailed)
        {
            throw new InvalidOperationException($"Handling {message} failed: {string.Join("; ", result.Errors.Select(e => e.Message))}");
        }
    }
}
