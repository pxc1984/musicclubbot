using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class SongRoleRepository(DbContext dbContext) : Repository<SongRole>(dbContext), ISongRoleRepository
{
    public async Task<SongRole?> FindByIdWithSongAndAssignmentAsync(Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Song)
            .Include(r => r.Assignment)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRoleTitlesBySongIdAsync(Guid songId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.SongId == songId)
            .Select(r => r.RoleTitle)
            .ToListAsync(cancellationToken);
    }
}