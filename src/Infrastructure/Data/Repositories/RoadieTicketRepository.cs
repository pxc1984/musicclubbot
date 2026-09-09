using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class RoadieTicketRepository(DbContext dbContext)
    : Repository<RoadieTicket>(dbContext), IRoadieTicketRepository
{
    public async Task<RoadieTicket?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(t => t.Song)
            .Include(t => t.CreatedBy)
            .Include(t => t.AcceptedBy)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<bool> HasOpenTicketAsync(Guid songId,
        RoadieTicketType ticketType = RoadieTicketType.Assignment,
        CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(t => t.SongId == songId && t.AcceptedById == null && t.RoadieTicketType == ticketType, ct);
    }

    public async Task<IReadOnlyList<RoadieTicket>> GetOpenTicketsOlderThanAsync(
        DateTimeOffset olderThan, CancellationToken ct = default)
    {
        return await DbSet
            .Where(t => t.AcceptedById == null && t.CreatedAt < olderThan)
            .ToListAsync(ct);
    }
}
