namespace CuMusicClub.Domain.Entities;

/// <summary>
/// Это не все возможные роли которые добавлены в песни, это именно названия базовых ролей в песнях, которые считаются
/// общепринятыми ролями и под которые можно выставлять предпочтения в своем профиле
/// </summary>
public class RoleTitle
{
    /// <summary>
    /// Здесь используется Id long, тк эти роли по сути константы, и чтоб проще было воспроизводить поведение именно long, а не случайный Guid
    /// </summary>
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    /// <summary>
    /// Именно вот HTML записанный svg иконки, изображающей роль
    /// </summary>
    public string? Svg { get; set; }

    public ICollection<ApplicationUser> Users = [];
}
