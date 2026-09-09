namespace CuMusicClub.Domain.Entities;

/// <summary>
/// Закрепление роуди за песней (один роуди на песню). Таблица <c>song_roadie</c>.
/// </summary>
public class SongRoadie
{
    public Guid SongId { get; set; }              // PK
    public Song Song { get; set; } = null!;
    public Guid RoadieId { get; set; }            // FK -> app_user
    public ApplicationUser Roadie { get; set; } = null!;
    public DateTimeOffset AssignedAt { get; set; }
}