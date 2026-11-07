using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.Application.Households.PurgeLeftLists;

namespace Todo.Infrastructure.Jobs;

/// <summary>
/// T5: every few hours, deletes private lists whose owner left the household 30 days ago. A failed round is logged
/// and tried again next time.
/// </summary>
internal sealed partial class LeftListsCleanup(IServiceScopeFactory scopes, ILogger<LeftListsCleanup> logger) : BackgroundService
{
    public static readonly TimeSpan Interval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            await RunOnceAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    internal async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopes.CreateAsyncScope();
            var removed = await scope.ServiceProvider.GetRequiredService<ISender>().Send(new PurgeLeftListsCommand(), cancellationToken);
            if (removed.IsSuccess && removed.Value > 0)
            {
                LogRemoved(removed.Value);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogFailed(exception);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Deleted {Count} private lists of people who left their household.")]
    private partial void LogRemoved(int count);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Deleting private lists of people who left failed; trying again later.")]
    private partial void LogFailed(Exception exception);
}
