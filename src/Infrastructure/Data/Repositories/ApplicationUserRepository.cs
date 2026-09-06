using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class ApplicationUserRepository(ApplicationDbContext dbContext)
    : Repository<ApplicationUser>(dbContext), IApplicationUserRepository
{
    public async Task<ApplicationUser?> FindByTgUserIdAsync(long tgUserId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.TgUserId == tgUserId, cancellationToken);
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetUsersWithoutPermissionsAsync(int limit,
        string permissionClaimType,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Where(u => !dbContext.UserClaims.Any(c =>
                c.UserId == u.Id && c.ClaimType == permissionClaimType))
            .OrderBy(u => u.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}