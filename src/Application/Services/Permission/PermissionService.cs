using CuMusicClub.Application.Services.Permission;
using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Application.Services.Permission;

public class PermissionService(IApplicationUserRepository users) : IPermissionService
{
    public Task<IReadOnlyList<string>> GetPermissionValuesAsync(ApplicationUser user,
        CancellationToken cancellationToken)
    {
        return users.GetPermissionsAsync(user.Id, cancellationToken);
    }

    public Task GrantPermissionsAsync(ApplicationUser user,
        IEnumerable<string> permissions,
        CancellationToken cancellationToken)
    {
        return users.GrantPermissionsAsync(user.Id, permissions, cancellationToken);
    }

    public Task GrantRoleAsync(ApplicationUser user, string role, CancellationToken cancellationToken)
    {
        // Роли — чистый «сахар»: сам факт роли не хранится, материализуется лишь её пакет прав.
        if (Domain.Constants.Permission.ByRole.TryGetValue(role, out var bundle))
            return GrantPermissionsAsync(user, bundle, cancellationToken);

        return Task.CompletedTask;
    }

    public Task GrantDefaultAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return GrantPermissionsAsync(user, Domain.Constants.Permission.Default, cancellationToken);
    }
}