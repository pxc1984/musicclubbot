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
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Where(u => !dbContext.UserPermissions.Any(p => p.UserId == u.Id))
            .OrderBy(u => u.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserPermissions
            .Where(p => p.UserId == userId)
            .Select(p => p.Permission)
            .ToListAsync(cancellationToken);
    }

    public async Task GrantPermissionsAsync(Guid userId,
        IEnumerable<string> permissions,
        CancellationToken cancellationToken = default)
    {
        var distinct = permissions.Distinct(StringComparer.Ordinal).ToList();
        if (distinct.Count == 0) return;

        var existing = await dbContext.UserPermissions
            .Where(p => p.UserId == userId)
            .Select(p => p.Permission)
            .ToListAsync(cancellationToken);
        var existingSet = existing.ToHashSet(StringComparer.Ordinal);

        foreach (var permission in distinct)
        {
            if (existingSet.Contains(permission)) continue;
            await dbContext.UserPermissions.AddAsync(new UserPermission
            {
                UserId = userId,
                Permission = permission,
            }, cancellationToken);
        }
    }
}