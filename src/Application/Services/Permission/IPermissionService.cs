using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Application.Services.Permission;

/// <summary>
/// Central place for reading/writing user permissions.
/// Permissions always live as individual rows in the <c>user_permissions</c> table;
/// roles are only sugar bundles that materialize those rows.
/// </summary>
public interface IPermissionService
{
    Task<IReadOnlyList<string>> GetPermissionValuesAsync(ApplicationUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Writes the given permission values as individual <c>user_permissions</c> rows for the user
    /// (idempotent — existing permissions are left untouched).
    /// </summary>
    Task GrantPermissionsAsync(ApplicationUser user,
        IEnumerable<string> permissions,
        CancellationToken cancellationToken);

    /// <summary>
    /// Assigns a role to the user. The role itself is pure sugar: its permission bundle is
    /// materialized as individual <c>user_permissions</c> rows on the user.
    /// </summary>
    Task GrantRoleAsync(ApplicationUser user, string role, CancellationToken cancellationToken);

    /// <summary>
    /// Grants the default permission bundle to the user.
    /// </summary>
    Task GrantDefaultAsync(ApplicationUser user, CancellationToken cancellationToken);
}
