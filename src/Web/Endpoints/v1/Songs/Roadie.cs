using System.Security.Claims;
using CuMusicClub.Application.Services.Roadie;
using CuMusicClub.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CuMusicClub.Web.Endpoints.v1.Songs;

public static partial class Songs
{
    [EndpointSummary("Call a roadie for help")]
    private static async Task<Results<Created<RoadieTicketDto>, BadRequest>> CreateTicket(
        IRoadieService service,
        ClaimsPrincipal user,
        Guid songId,
        CancellationToken cancellationToken)
    {
        var ticket = await service.CreateTicketAsync(songId, user, RoadieTicketType.Help, cancellationToken);
        return TypedResults.Created($"/api/v1/songs/{songId}/roadie-ticket", ticket);
    }
}
