using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CuMusicClub.Application.Services.Auth;
using CuMusicClub.Application.Services.Telegram;
using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Constants;
using CuMusicClub.Domain.Entities;
using CuMusicClub.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CuMusicClub.Infrastructure.IntegrationTests.Auth;

public partial class AuthServiceTests : TestBase
{
    private sealed class AuthScope : IDisposable
    {
        private readonly IServiceScope _scope;
        public IAuthService Auth { get; }
        public IApplicationUserRepository Users { get; }
        public ITelegramAuthService TelegramAuth { get; }

        public AuthScope()
        {
            _scope = FunctionalTestSetup.ScopeFactory.CreateScope();
            Auth = _scope.ServiceProvider.GetRequiredService<IAuthService>();
            Users = _scope.ServiceProvider.GetRequiredService<IApplicationUserRepository>();
            TelegramAuth = _scope.ServiceProvider.GetRequiredService<ITelegramAuthService>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }

    private static ApplicationDbContext Db()
    {
        var scope = FunctionalTestSetup.ScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    private static async Task<(ApplicationUser AppUser, ClaimsPrincipal Principal)> CreateUserAsync(string username)
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IApplicationUserRepository>();
        var user = new ApplicationUser
        {
            UserName = username,
            DisplayName = $"Display {username}",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        await users.AddAsync(user);
        await users.GrantPermissionsAsync(user.Id, [Permission.ParticipationEditOwn, Permission.SongsEditOwn]);
        await users.SaveChangesAsync();

        var identity = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            ],
            "test");

        return (user, new ClaimsPrincipal(identity));
    }
}
