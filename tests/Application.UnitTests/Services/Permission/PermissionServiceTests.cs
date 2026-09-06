using CuMusicClub.Application.Services.Permission;
using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Constants;
using CuMusicClub.Domain.Entities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace CuMusicClub.Application.UnitTests.Services.Permission;

[TestFixture]
[TestOf(typeof(PermissionService))]
public class PermissionServiceTests
{
    private Mock<IApplicationUserRepository> _users = null!;
    private PermissionService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _users = new Mock<IApplicationUserRepository>();
        _service = new PermissionService(_users.Object);
    }

    private static ApplicationUser User() => new() { Id = Guid.NewGuid(), UserName = "test" };

    [Test]
    public async Task GetPermissionValues_ReturnsPermissionsFromRepository()
    {
        var user = User();
        var permissions = new[]
        {
            CuMusicClub.Domain.Constants.Permission.ParticipationEditOwn,
            CuMusicClub.Domain.Constants.Permission.SongsEditOwn,
        };
        _users
            .Setup(u => u.GetPermissionsAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(permissions);

        var result = await _service.GetPermissionValuesAsync(user, CancellationToken.None);

        result.Count.ShouldBe(2);
        result.ShouldContain(CuMusicClub.Domain.Constants.Permission.ParticipationEditOwn);
        result.ShouldContain(CuMusicClub.Domain.Constants.Permission.SongsEditOwn);
    }

    [Test]
    public async Task GetPermissionValues_NoPermissions_ReturnsEmpty()
    {
        var user = User();
        _users
            .Setup(u => u.GetPermissionsAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetPermissionValuesAsync(user, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GrantDefault_CallsRepositoryWithDefaultBundle()
    {
        var user = User();

        await _service.GrantDefaultAsync(user, CancellationToken.None);

        _users.Verify(u => u.GrantPermissionsAsync(user.Id,
                CuMusicClub.Domain.Constants.Permission.Default,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GrantPermissions_CallsRepository()
    {
        var user = User();
        var permissions = new[] { CuMusicClub.Domain.Constants.Permission.SongsEditOwn, };

        await _service.GrantPermissionsAsync(user, permissions, CancellationToken.None);

        _users.Verify(u => u.GrantPermissionsAsync(user.Id, permissions, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GrantRole_KnownRole_GrantsBundle()
    {
        var user = User();

        await _service.GrantRoleAsync(user, Roles.Roadie, CancellationToken.None);

        _users.Verify(u => u.GrantPermissionsAsync(user.Id,
                CuMusicClub.Domain.Constants.Permission.Roadie,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GrantRole_UnknownRole_DoesNothing()
    {
        var user = User();

        await _service.GrantRoleAsync(user, "UnknownRole", CancellationToken.None);

        _users.Verify(u => u.GrantPermissionsAsync(It.IsAny<Guid>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}