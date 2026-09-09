namespace CuMusicClub.Domain.Entities;

/// <summary>
/// Заявка группы (песни) на помощь роуди. Таблица <c>roadie_ticket</c>.
/// Статус через nullable-поля (не enum): <c>AcceptedById == null</c> ⇔ заявка открыта.
/// </summary>
public class RoadieTicket
{
    public Guid Id { get; set; }
    public Guid SongId { get; set; }
    public Song Song { get; set; } = null!;
    public Guid CreatedById { get; set; }         // кто вызвал
    public ApplicationUser CreatedBy { get; set; } = null!;
    public Guid? AcceptedById { get; set; }       // кто взял (роуди)
    public ApplicationUser? AcceptedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
}