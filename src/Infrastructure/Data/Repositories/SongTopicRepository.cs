using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class SongTopicRepository(DbContext dbContext) : Repository<SongTopic>(dbContext), ISongTopicRepository
{
    public async Task<SongTopic?> FindBySongIdAsync(Guid songId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(t => t.SongId == songId, cancellationToken);
    }

    public async Task<SongTopic?> FindByTopicIdAsync(long topicId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstAsync(t => t.TopicId == topicId, cancellationToken);
    }
}