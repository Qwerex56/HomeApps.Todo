using FluentResults;
using HomeApps.Contracts.Households;
using MassTransit;
using MediatR;
using Todo.Application.Households;

namespace Todo.Infrastructure.Households;

/// <summary>
/// Follows who leaves and joins households (plan 4.2, T5). Each message only sends a command; the commands are
/// idempotent. A failed result throws, so the message is retried and ends in the <c>_error</c> queue rather than lost.
/// Public because MassTransit's scan finds public consumers only.
/// </summary>
public sealed class HouseholdMembersConsumer(ISender sender) : IConsumer<MemberLeft>, IConsumer<MemberJoined>
{
    public async Task Consume(ConsumeContext<MemberLeft> context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var m = context.Message;
        ThrowIfFailed(await sender.Send(new MemberLeftCommand(m.HouseholdId, m.UserId), context.CancellationToken), m);
    }

    public async Task Consume(ConsumeContext<MemberJoined> context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var m = context.Message;
        ThrowIfFailed(await sender.Send(new MemberJoinedCommand(m.HouseholdId, m.UserId), context.CancellationToken), m);
    }

    private static void ThrowIfFailed(Result result, object message)
    {
        if (result.IsFailed)
        {
            throw new InvalidOperationException($"Handling {message} failed: {string.Join("; ", result.Errors.Select(e => e.Message))}");
        }
    }
}
