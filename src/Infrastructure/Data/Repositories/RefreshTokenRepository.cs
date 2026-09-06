using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class RefreshTokenRepository(DbContext dbContext) : Repository<RefreshToken>(dbContext), IRefreshTokenRepository
{
    public async Task<RefreshToken?> FindActiveByJtiAsync(Guid jti, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            t => t.Jti == jti && !t.Revoked && t.Exp > DateTimeOffset.UtcNow,
            cancellationToken);
    }
}