using System.Security.Claims;
using CuMusicClub.Application.Services.Permission;
using CuMusicClub.Domain.Constants;
using CuMusicClub.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace CuMusicClub.Application.UnitTests.Services.Permission;

[TestFixture]
[TestOf(typeof(PermissionService))]
public class PermissionServiceTests
{
    private Mock<UserManager<ApplicationUser>> _userManager = null!;
    private Mock<RoleManager<IdentityRole<Guid>>> _roleManager = null!;
    private PermissionService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!,
            Mock.Of<IPasswordHasher<ApplicationUser>>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<Microsoft.Extensions.Logging.ILogger<UserManager<ApplicationUser>>>());
        _roleManager = new Mock<RoleManager<IdentityRole<Guid>>>(
            Mock.Of<IRoleStore<IdentityRole<Guid>>>(),
            Array.Empty<IRoleValidator<IdentityRole<Guid>>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<Microsoft.Extensions.Logging.ILogger<RoleManager<IdentityRole<Guid>>>>());

        _service = new PermissionService(_userManager.Object, _roleManager.Object);
    }

    private static ApplicationUser User() => new() { Id = Guid.NewGuid(), UserName = "test" };

    [Test]
    public async Task GetPermissionValues_ReturnsOnlyPermissionClaims()
    {
        var user = User();
        var claims = new[]
        {
            new Claim(PermissionClaimTypes.Permission, CuMusicClub.Domain.Constants.Permission.ParticipationEditOwn),
            new Claim(PermissionClaimTypes.Permission, CuMusicClub.Domain.Constants.Permission.SongsEditOwn),
            new Claim("other", "value"),
        };
        _userManager
            .Setup(u => u.GetClaimsAsync(user))
            .ReturnsAsync(claims);

        var result = await _service.GetPermissionValuesAsync(user, CancellationToken.None);

        result.Count.ShouldBe(2);
        result.ShouldContain(CuMusicClub.Domain.Constants.Permission.ParticipationEditOwn);
        result.ShouldContain(CuMusicClub.Domain.Constants.Permission.SongsEditOwn);
    }

    [Test]
    public async Task GetPermissionValues_NoClaims_ReturnsEmpty()
    {
        var user = User();
        _userManager
            .Setup(u => u.GetClaimsAsync(user))
            .ReturnsAsync([]);

        var result = await _service.GetPermissionValuesAsync(user, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GrantDefault_GrantsDefaultBundle()
    {
        var user = User();
        _userManager
            .Setup(u => u.GetClaimsAsync(user))
            .ReturnsAsync([]);

        await _service.GrantDefaultAsync(user, CancellationToken.None);

        foreach (var permission in CuMusicClub.Domain.Constants.Permission.Default)
            _userManager.Verify(u => u.AddClaimAsync(user,
                    It.Is<Claim>(c => c.Type == PermissionClaimTypes.Permission && c.Value == permission)),
                Times.Once);
    }

    [Test]
    public async Task GrantPermissions_IsIdempotent()
    {
        var user = User();
        _userManager
            .Setup(u => u.GetClaimsAsync(user))
            .ReturnsAsync([
                new Claim(PermissionClaimTypes.Permission, CuMusicClub.Domain.Constants.Permission.ParticipationEditOwn),
            ]);

        await _service.GrantPermissionsAsync(user, CuMusicClub.Domain.Constants.Permission.Default, CancellationToken.None);

        // ParticipationEditOwn уже есть — не должен быть добавлен повторно
        _userManager.Verify(u => u.AddClaimAsync(user,
                It.Is<Claim>(c => c.Value == CuMusicClub.Domain.Constants.Permission.ParticipationEditOwn)),
            Times.Never);
        // SongsEditOwn добавляется
        _userManager.Verify(u => u.AddClaimAsync(user,
                It.Is<Claim>(c => c.Value == CuMusicClub.Domain.Constants.Permission.SongsEditOwn)),
            Times.Once);
    }

    [Test]
    public async Task GrantRole_CreatesRoleIfMissing_AndAssigns()
    {
        var user = User();
        _roleManager
            .Setup(r => r.RoleExistsAsync(Roles.Roadie))
            .ReturnsAsync(false);
        _roleManager
            .Setup(r => r.CreateAsync(It.IsAny<IdentityRole<Guid>>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManager
            .Setup(u => u.IsInRoleAsync(user, Roles.Roadie))
            .ReturnsAsync(false);
        _userManager
            .Setup(u => u.AddToRoleAsync(user, Roles.Roadie))
            .ReturnsAsync(IdentityResult.Success);
        _userManager
            .Setup(u => u.GetClaimsAsync(user))
            .ReturnsAsync([]);

        await _service.GrantRoleAsync(user, Roles.Roadie, CancellationToken.None);

        _roleManager.Verify(r => r.CreateAsync(It.IsAny<IdentityRole<Guid>>()), Times.Once);
        _userManager.Verify(u => u.AddToRoleAsync(user, Roles.Roadie), Times.Once);
        // Roadie bundle материализуется в claims
        _userManager.Verify(u => u.AddClaimAsync(user,
                It.Is<Claim>(c => c.Type == PermissionClaimTypes.Permission && c.Value == CuMusicClub.Domain.Constants.Permission.ParticipationEditAny)),
            Times.Once);
    }

    [Test]
    public async Task GrantRole_ExistingRole_DoesNotRecreate()
    {
        var user = User();
        _roleManager
            .Setup(r => r.RoleExistsAsync(Roles.Default))
            .ReturnsAsync(true);
        _userManager
            .Setup(u => u.IsInRoleAsync(user, Roles.Default))
            .ReturnsAsync(true);
        _userManager
            .Setup(u => u.GetClaimsAsync(user))
            .ReturnsAsync([]);

        await _service.GrantRoleAsync(user, Roles.Default, CancellationToken.None);

        _roleManager.Verify(r => r.CreateAsync(It.IsAny<IdentityRole<Guid>>()), Times.Never);
        _userManager.Verify(u => u.AddToRoleAsync(user, Roles.Default), Times.Never);
    }
}