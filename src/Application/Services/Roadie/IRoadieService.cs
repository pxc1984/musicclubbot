using System.Security.Claims;

namespace CuMusicClub.Application.Services.Roadie;

public enum RoadieAcceptResult
{
    Accepted,
    NotARoadie,
    NotFound,
    AlreadyAccepted
}

public interface IRoadieService
{
    Task<RoadieTicketDto> CreateTicketAsync(Guid songId,
        ClaimsPrincipal currentUser,
        RoadieTicketType ticketType,
        CancellationToken cancellationToken = default);

    Task<RoadieAcceptResult> AcceptTicketAsync(long tgUserId,
        Guid ticketId,
        CancellationToken cancellationToken = default);

    Task<int> AutoAssignOpenTicketsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> ListRoadies(CancellationToken cancellationToken = default);
}
