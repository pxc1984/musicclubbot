using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class TgAuthLinkRepository(DbContext dbContext) : Repository<TgAuthLink>(dbContext), ITgAuthLinkRepository
{
    public async Task<TgAuthLink?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
}