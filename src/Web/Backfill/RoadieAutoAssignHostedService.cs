using CuMusicClub.Application.Services.Roadie;

namespace CuMusicClub.Web.Backfill;

public sealed class RoadieAutoAssignHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<RoadieAutoAssignHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RunAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Roadie auto-assign run failed");
            }

            try { await Task.Delay(Interval, cancellationToken); }
            catch (OperationCanceledException) { break; }
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IRoadieService>();
        var assigned = await service.AutoAssignOpenTicketsAsync(cancellationToken);
        if (assigned > 0)
            logger.LogInformation("Roadie auto-assign: {Count} tickets assigned", assigned);
    }
}