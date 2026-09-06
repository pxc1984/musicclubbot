using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий пользователей приложения.</summary>
public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> FindByTgUserIdAsync(long tgUserId, CancellationToken cancellationToken = default);

    /// <summary>Пользователи, у которых ещё нет permission-claim'ов (для бэкфилла прав).</summary>
    Task<IReadOnlyList<ApplicationUser>> GetUsersWithoutPermissionsAsync(int limit,
        string permissionClaimType,
        CancellationToken cancellationToken = default);
}