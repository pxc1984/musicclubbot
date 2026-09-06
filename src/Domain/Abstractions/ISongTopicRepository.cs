using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий топиков песен в Telegram.</summary>
public interface ISongTopicRepository : IRepository<SongTopic>
{
    Task<SongTopic?> FindBySongIdAsync(Guid songId, CancellationToken cancellationToken = default);

    Task<SongTopic?> FindByTopicIdAsync(long topicId, CancellationToken cancellationToken = default);
}