using CuMusicClub.Application.Common.Options;
using CuMusicClub.Application.Services.Telegram;
using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.Extensions.Options;

namespace CuMusicClub.Web.Backfill;

public sealed class RoadieBackfillHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<TelegramOptions> telegramOptions,
    ILogger<RoadieBackfillHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try { await RunAsync(cancellationToken); }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception ex) { logger.LogError(ex, "Roadie backfill failed"); }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var sendNotifications = !string.IsNullOrWhiteSpace(telegramOptions.Value.RoadieChatId);
        await using var scope = scopeFactory.CreateAsyncScope();

        var songRoadies = scope.ServiceProvider.GetRequiredService<ISongRoadieRepository>();
        var tickets = scope.ServiceProvider.GetRequiredService<IRoadieTicketRepository>();
        var songs = scope.ServiceProvider.GetRequiredService<ISongRepository>();
        var telegram = scope.ServiceProvider.GetRequiredService<ITelegramChatService>();

        var songIds = await songRoadies.GetAssembledSongIdsWithoutRoadieAsync(cancellationToken);
        if (songIds.Count == 0)
        {
            logger.LogDebug("Roadie backfill: no assembled songs without a roadie");
            return;
        }

        var created = 0;
        foreach (var songId in songIds)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var song = await songs.FindByIdAsync(songId);
            if (song is null) continue;
            if (song.CreatedById is not Guid createdById) continue;

            // Открытая заявка от имени создателя песни (системный бекфилл: «собранная» песня нуждается в роуди).
            var ticket = new RoadieTicket
            {
                Id = Guid.NewGuid(),
                SongId = song.Id,
                CreatedById = createdById,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            await tickets.AddAsync(ticket, cancellationToken);
            created++;

            if (sendNotifications)
                await telegram.SendRoadieTicketNotification(ticket.Id, song.Title, song.Artist, cancellationToken);
        }

        await tickets.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Roadie backfill: created {Count} tickets for assembled songs without a roadie", created);
    }
}