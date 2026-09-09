namespace CuMusicClub.Application.Services.Roadie;

public sealed record RoadieTicketDto(Guid Id, Guid SongId, string SongTitle, string Artist,
    bool IsOpen, Guid? AcceptedById, DateTimeOffset CreatedAt, DateTimeOffset? AcceptedAt);