using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Application.Services.Song;

public partial class SongService
{
    private async Task ReplaceRolesAsync(Guid songId,
        IReadOnlyCollection<string> desiredRoles,
        CancellationToken cancellationToken)
    {
        var song = await songs.FindByIdWithTopicAndRolesAsync(songId, cancellationToken);

        var currentRoleTitles = song.Roles
            .Select(r => r.RoleTitle)
            .ToHashSet(StringComparer.Ordinal);

        var desiredSet = desiredRoles.ToHashSet(StringComparer.Ordinal);

        var toRemove = song.Roles
            .Where(role => !desiredSet.Contains(role.RoleTitle))
            .ToList();

        var toAdd = desiredSet
            .Where(role => !currentRoleTitles.Contains(role))
            .ToList();

        foreach (var songRole in toRemove)
        {
            if (song.SongTopic != null)
                await new SongServiceTopics(telegramChatService).AnnounceRoleRemovedAsync(song.SongTopic.TopicId,
                    songRole.RoleTitle,
                    songRole.Assignment?.User,
                    cancellationToken);
        }

        foreach (var role in toAdd)
        {
            if (song.SongTopic != null)
                await new SongServiceTopics(telegramChatService).AnnounceRoleAddedAsync(song.SongTopic.TopicId,
                    role,
                    cancellationToken);

            await songRoles.AddAsync(new SongRole
            {
                SongId = songId,
                RoleTitle = role,
            }, cancellationToken);
        }

        foreach (var songRole in toRemove) songRoles.Remove(songRole);
    }
}