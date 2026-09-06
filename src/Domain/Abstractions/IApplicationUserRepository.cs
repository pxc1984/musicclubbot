using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий пользователей приложения.</summary>
public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> FindByTgUserIdAsync(long tgUserId, CancellationToken cancellationToken = default);

    /// <summary>Пользователи, у которых ещё нет ни одного права (для бэкфилла прав).</summary>
    Task<IReadOnlyList<ApplicationUser>> GetUsersWithoutPermissionsAsync(int limit,
        CancellationToken cancellationToken = default);

    /// <summary>Все права пользователя (строковые значения из таблицы <c>user_permissions</c>).</summary>
    Task<IReadOnlyList<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет права пользователю (идемпотентно — уже существующие не трогает).
    /// Не сохраняет изменения; вызов <see cref="IRepository{TEntity}.SaveChangesAsync"/> — обязанность вызывающего.
    /// </summary>
    Task GrantPermissionsAsync(Guid userId,
        IEnumerable<string> permissions,
        CancellationToken cancellationToken = default);
}