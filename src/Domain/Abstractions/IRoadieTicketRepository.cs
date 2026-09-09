using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий заявок групп на помощь роуди.</summary>
public interface IRoadieTicketRepository : IRepository<RoadieTicket>
{
    /// <summary>
    /// Заявка по идентификатору с подгрузкой связей <see cref="RoadieTicket.Song"/>,
    /// <see cref="RoadieTicket.CreatedBy"/> и <see cref="RoadieTicket.AcceptedBy"/>.
    /// </summary>
    Task<RoadieTicket?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

    /// <summary>Есть ли открытая (<c>AcceptedById == null</c>) заявка для песни.</summary>
    Task<bool> HasOpenTicketAsync(Guid songId, RoadieTicketType ticketType = RoadieTicketType.Assignment, CancellationToken ct = default);

    /// <summary>
    /// Открытые заявки, созданные раньше <paramref name="olderThan"/>
    /// (для авто-распределения).
    /// </summary>
    Task<IReadOnlyList<RoadieTicket>> GetOpenTicketsOlderThanAsync(
        DateTimeOffset olderThan, CancellationToken ct = default);
}
