using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class SongRoleAssignmentRepository(DbContext dbContext)
    : Repository<SongRoleAssignment>(dbContext), ISongRoleAssignmentRepository
{
    public async Task RemoveByIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        await DbSet.Where(s => s.Id == assignmentId).ExecuteDeleteAsync(cancellationToken);
    }
}