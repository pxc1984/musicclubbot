namespace CuMusicClub.Domain.Entities;

/// <summary>
/// Право пользователя (одна строка на право). Аналог go-based таблицы
/// <c>user_permissions</c>, но со строковыми правами вместо boolean-колонок.
/// </summary>
public class UserPermission
{
    public Guid UserId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}