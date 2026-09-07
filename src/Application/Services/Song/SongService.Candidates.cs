using System.Security.Claims;
using CuMusicClub.Application.Common.Auth;
using CuMusicClub.Application.Common.Exceptions;
using CuMusicClub.Application.Services.Song;
using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Application.Services.Song;

public partial class SongService
{
    public async Task<RoleCandidatesDto> GetRoleCandidatesAsync(
        Guid songId,
        Guid roleId,
        string? query,
        string? mode,
        ClaimsPrincipal currentUser,
        CancellationToken cancellationToken)
    {
        var requester = await users.FindByIdAsync(currentUser.GetUserId())
                        ?? throw new UnauthorizedAccessException();
        var permissions = await permissionService.GetPermissionValuesAsync(requester, cancellationToken);
        var hasEditOwn = permissions.Contains(Domain.Constants.Permission.ParticipationEditOwn);
        var hasEditAny = permissions.Contains(Domain.Constants.Permission.ParticipationEditAny);

        if (string.Equals(mode, "remove", StringComparison.OrdinalIgnoreCase))
        {
            var removable = await GetRemovableMembersAsync(songId,
                requester.Id,
                query,
                hasEditOwn,
                hasEditAny,
                cancellationToken);
            return new RoleCandidatesDto(removable);
        }

        var assignable = await GetAssignableUsersAsync(songId,
            roleId,
            requester,
            query,
            hasEditOwn,
            hasEditAny,
            cancellationToken);
        return new RoleCandidatesDto(assignable);
    }

    private async Task<IReadOnlyList<SongUserDto>> GetAssignableUsersAsync(
        Guid songId,
        Guid roleId,
        ApplicationUser requester,
        string? query,
        bool hasEditOwn,
        bool hasEditAny,
        CancellationToken cancellationToken)
    {
        var role = await songRoles.FindByIdWithSongAndAssignmentAsync(roleId, cancellationToken);
        if (role == null || role.SongId != songId)
            throw new NotFoundException(roleId.ToString(), nameof(SongRole));
        if (role.Assignment != null) return [];

        // Без права добавлять никого не вернём.
        if (!hasEditOwn && !hasEditAny) return [];

        var memberIds = (await songRoleAssignments.GetMemberUserIdsBySongIdAsync(songId, cancellationToken))
            .ToHashSet();

        var candidates = new List<SongUserDto>();

        // Первым — сам пользователь (за себя можно всегда, если есть право).
        if (hasEditOwn) candidates.Add(MapUser(requester));

        if (hasEditAny)
        {
            var all = await users.GetUsersAsync(query, cancellationToken);
            foreach (var u in all)
            {
                if (u.Id == requester.Id || memberIds.Contains(u.Id)) continue;
                if (u.Preferences is { AllowAdding: false }) continue;
                candidates.Add(MapUser(u));
            }
        }

        return OrderSelfFirst(requester.Id, candidates);
    }

    private async Task<IReadOnlyList<SongUserDto>> GetRemovableMembersAsync(
        Guid songId,
        Guid requesterId,
        string? query,
        bool hasEditOwn,
        bool hasEditAny,
        CancellationToken cancellationToken)
    {
        var all = await users.GetUsersAsync(query, cancellationToken);
        var memberIds = (await songRoleAssignments.GetMemberUserIdsBySongIdAsync(songId, cancellationToken))
            .ToHashSet();

        var removable = new List<SongUserDto>();
        foreach (var u in all)
        {
            if (!memberIds.Contains(u.Id)) continue;

            var isSelf = u.Id == requesterId;
            var canRemove = isSelf
                ? hasEditOwn
                : hasEditAny && u.Preferences is not { AllowRemoving: false };
            if (canRemove) removable.Add(MapUser(u));
        }

        return OrderSelfFirst(requesterId, removable);
    }

    private static IReadOnlyList<SongUserDto> OrderSelfFirst(Guid requesterId, List<SongUserDto> users)
    {
        return users
            .OrderByDescending(u => u.Id == requesterId)
            .ThenBy(u => u.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static SongUserDto MapUser(ApplicationUser user)
    {
        return new SongUserDto(user.Id, user.DisplayName, user.UserName, user.AvatarUrl, user.TgUserId);
    }
}