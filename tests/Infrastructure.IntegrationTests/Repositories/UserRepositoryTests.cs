using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using CuMusicClub.Infrastructure.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace CuMusicClub.Infrastructure.IntegrationTests.Repositories;

[TestFixture]
public class UserRepositoryTests : TestBase
{
    [Test]
    public async Task GetUsersAsync_ReturnsUsersWithPreferences()
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IApplicationUserRepository>();

        var user = new ApplicationUser
        {
            UserName = "alice",
            DisplayName = "Alice",
        };
        user.Preferences = new UserPreferences
        {
            UserId = user.Id,
            AllowAdding = false,
            AllowRemoving = true,
        };
        await users.AddAsync(user);
        await users.SaveChangesAsync();

        var found = await users.GetUsersAsync("alice");

        var loaded = found.Single(u => u.Id == user.Id);
        loaded.Preferences.ShouldNotBeNull();
        loaded.Preferences.AllowAdding.ShouldBeFalse();
        loaded.Preferences.AllowRemoving.ShouldBeTrue();
    }

    [Test]
    public async Task GetUsersAsync_WithoutQuery_ReturnsAllAndFiltersByName()
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IApplicationUserRepository>();

        await users.AddAsync(new ApplicationUser { UserName = "bob", DisplayName = "Bob" });
        await users.AddAsync(new ApplicationUser { UserName = "carol", DisplayName = "Carol" });
        await users.SaveChangesAsync();

        var all = await users.GetUsersAsync(null);
        all.Count(u => u.UserName == "bob" || u.UserName == "carol").ShouldBe(2);

        var filtered = await users.GetUsersAsync("carol");
        filtered.ShouldContain(u => u.UserName == "carol");
        filtered.ShouldNotContain(u => u.UserName == "bob");
    }
}