using System.Security.Claims;
using CuMusicClub.Application.Common.Auth;
using CuMusicClub.Application.Services.Auth;
using CuMusicClub.Application.Services.Permission;
using CuMusicClub.Domain.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CuMusicClub.Web.Endpoints.v1.Auth;

public static partial class Auth
{
    [EndpointSummary("Get the current user's profile")]
    private static async Task<Results<Ok<UserProfileDto>, NotFound>> Me(ClaimsPrincipal claimsPrincipal,
        IPermissionService permissionService,
        IApplicationUserRepository users,
        CancellationToken cancellationToken)
    {
        var user = await users.FindByIdAsync(claimsPrincipal.GetUserId());
        if (user is null) return TypedResults.NotFound();

        var profile = new UserProfileDto(user.Id,
            user.DisplayName,
            user.UserName!,
            user.AvatarUrl,
            await permissionService.GetPermissionValuesAsync(user, cancellationToken),
            null,
            user.CreatedAt,
            user.UpdatedAt);

        return TypedResults.Ok(profile);
    }
}
