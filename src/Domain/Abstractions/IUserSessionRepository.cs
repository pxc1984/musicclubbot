using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий пользовательских сессий.</summary>
public interface IUserSessionRepository : IRepository<UserSession>
{
    Task<UserSession?> FindByRefreshTokenJtiAsync(Guid jti, CancellationToken cancellationToken = default);
}