using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class UserSessionRepository(DbContext dbContext) : Repository<UserSession>(dbContext), IUserSessionRepository
{
    public async Task<UserSession?> FindByRefreshTokenJtiAsync(Guid jti, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(s => s.RefreshTokenJti == jti, cancellationToken);
    }
}