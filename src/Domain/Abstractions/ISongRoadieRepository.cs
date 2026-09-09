using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий закреплений роуди за песнями.</summary>
public interface ISongRoadieRepository : IRepository<SongRoadie>
{
    /// <summary>Закрепление роуди по идентификатору песни (одно на песню).</summary>
    Task<SongRoadie?> FindBySongIdAsync(Guid songId, CancellationToken ct = default);

    /// <summary>
    /// Id собранных песен без роуди и без открытой заявки (для бекфилла).
    /// «Собранная» = у песни есть <see cref="SongTopic"/>. Исключаются песни, у которых
    /// уже есть <c>song_roadie</c> ИЛИ открытый <c>roadie_ticket</c>.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetAssembledSongIdsWithoutRoadieAsync(CancellationToken ct = default);
}