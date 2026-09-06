using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий назначений пользователей на роли.</summary>
public interface ISongRoleAssignmentRepository : IRepository<SongRoleAssignment>
{
    Task RemoveByIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
}