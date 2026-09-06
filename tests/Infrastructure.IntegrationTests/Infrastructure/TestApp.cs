using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Constants;
using CuMusicClub.Domain.Entities;
using CuMusicClub.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CuMusicClub.Infrastructure.IntegrationTests.Infrastructure;

public static class TestApp
{
    private static string? _userId;
    private static List<string>? _roles;

    public static string? GetUserId()
    {
        return _userId;
    }

    public static List<string>? GetRoles()
    {
        return _roles;
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", []);
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", [Roles.Administrator,]);
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();

        var users = scope.ServiceProvider.GetRequiredService<IApplicationUserRepository>();

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };
        await users.AddAsync(user);

        // Роли — «сахар»: материализуем их пакеты прав в user_permissions.
        var permissions = roles
            .Where(Permission.ByRole.ContainsKey)
            .SelectMany(r => Permission.ByRole[r]);
        await users.GrantPermissionsAsync(user.Id, permissions);
        await users.SaveChangesAsync();

        _userId = user.Id.ToString();
        _roles = [..roles,];
        return _userId;
    }

    public static async Task ResetState()
    {
        if (FunctionalTestSetup.DbResetter is not null) await FunctionalTestSetup.DbResetter.ResetAsync();

        _userId = null;
        _roles = null;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Add(entity);
        await context.SaveChangesAsync();
    }
}
