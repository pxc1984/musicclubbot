namespace CuMusicClub.Domain.Entities;

public class UserPreferences
{
    public Guid UserId;

    /// <summary>
    /// разрешить меня добавлять на роли без моего вмешательства
    /// </summary>
    public bool AllowAdding = true;

    /// <summary>
    /// разрешить меня снимать с ролей без моего вмешательства
    /// </summary>
    public bool AllowRemoving = true;

    public ApplicationUser User = null!;
}
