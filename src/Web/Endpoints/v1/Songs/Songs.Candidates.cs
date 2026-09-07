using System.Security.Claims;
using CuMusicClub.Application.Common.Auth;
using CuMusicClub.Application.Services.Song;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CuMusicClub.Web.Endpoints.v1.Songs;

public static partial class Songs
{
    /// <summary>
    /// Кандидаты для роли песни. Режимы:
    /// <c>assign</c> (по умолчанию) — кого можно назначить на роль (первым — сам пользователь),
    /// <c>remove</c> — кого можно снять с ролей в песне. Учитываются права и приватность.
    /// </summary>
    [EndpointSummary("Get assignable/removable users for a song role")]
    private static async Task<Ok<RoleCandidatesDto>> GetCandidates(ISongService service,
        ClaimsPrincipal claimsPrincipal,
        Guid songId,
        Guid roleId,
        string? query,
        string? mode,
        CancellationToken cancellationToken)
    {
        var result = await service.GetRoleCandidatesAsync(songId,
            roleId,
            query,
            mode,
            claimsPrincipal,
            cancellationToken);

        return TypedResults.Ok(result);
    }
}