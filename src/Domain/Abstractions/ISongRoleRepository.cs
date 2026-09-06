using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий ролей песен.</summary>
public interface ISongRoleRepository : IRepository<SongRole>
{
    Task<SongRole?> FindByIdWithSongAndAssignmentAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleTitlesBySongIdAsync(Guid songId, CancellationToken cancellationToken = default);
}