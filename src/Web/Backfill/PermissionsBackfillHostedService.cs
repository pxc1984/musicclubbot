using CuMusicClub.Application.Services.Permission;
using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Constants;
using CuMusicClub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CuMusicClub.Web.Backfill;

public sealed class PermissionsBackfillHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<PermissionsBackfillHostedService> logger) : BackgroundService
{
    private const int BatchSize = 50;
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RunBatchAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Permissions backfill run failed");
            }

            try
            {
                await Task.Delay(Interval, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task RunBatchAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting permissions backfill run");

        var succeeded = 0;
        var skipped = 0;

        await using var scope = scopeFactory.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<IApplicationUserRepository>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var usersWithoutPermissions = await users.GetUsersWithoutPermissionsAsync(BatchSize,
                PermissionClaimTypes.Permission,
                cancellationToken);

            if (usersWithoutPermissions.Count == 0) break;

            logger.LogDebug("Processing batch of {Count} users without permissions", usersWithoutPermissions.Count);

            foreach (var user in usersWithoutPermissions)
            {
                if (cancellationToken.IsCancellationRequested) break;

                try
                {
                    await permissionService.GrantDefaultAsync(user, cancellationToken);
                    succeeded++;
                    logger.LogDebug("Granted default permissions to user {UserId} ({UserName})", user.Id, user.UserName);
                }
                catch (Exception ex)
                {
                    skipped++;
                    logger.LogError(ex,
                        "Failed to grant default permissions to user {UserId} ({UserName})",
                        user.Id,
                        user.UserName);
                }
            }
        }

        if (succeeded != 0)
            logger.LogInformation("Permissions backfill run finished. Granted: {Succeeded}, Failed: {Skipped}",
                succeeded,
                skipped);
        else
            logger.LogDebug("Permissions backfill run finished. Granted: {Succeeded}, Failed: {Skipped}",
                succeeded,
                skipped);
    }
}
