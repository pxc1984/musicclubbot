using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий refresh-токенов.</summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> FindActiveByJtiAsync(Guid jti, CancellationToken cancellationToken = default);
}