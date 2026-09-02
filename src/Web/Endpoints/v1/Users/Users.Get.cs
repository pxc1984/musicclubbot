using System.Security.Claims;
using CuMusicClub.Application.Services.Auth;
using CuMusicClub.Application.Services.Permission;
using CuMusicClub.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace CuMusicClub.Web.Endpoints.v1.Users;

public static partial class Users
{
    [EndpointSummary("Get user info by id")]
    public static async Task<Results<Ok<UserProfileDto>, NotFound>> Get(UserManager<ApplicationUser> userManager,
        IPermissionService permissionService,
        Guid userId,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        // на случай если надо будет добавить авторизацию на этот эндпоинт
        // var requestingUser = await userManager.GetUserAsync(claimsPrincipal);

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return TypedResults.NotFound();

        throw new NotImplementedException();
    }
}
