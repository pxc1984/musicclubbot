using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий ссылок авторизации в Telegram.</summary>
public interface ITgAuthLinkRepository : IRepository<TgAuthLink>
{
    Task<TgAuthLink?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}