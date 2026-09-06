using CuMusicClub.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace CuMusicClub.Domain.UnitTests.Entities;

[TestFixture]
[TestOf(typeof(Song))]
public class SongTests
{
    [Test]
    public void DefaultId_IsEmptyGuid()
    {
        new Song().Id.ShouldBe(Guid.Empty);
    }

    [Test]
    public void DefaultTitle_IsEmptyString()
    {
        new Song().Title.ShouldBeEmpty();
    }

    [Test]
    public void DefaultArtist_IsEmptyString()
    {
        new Song().Artist.ShouldBeEmpty();
    }

    [Test]
    public void RolesCollection_IsInitialized()
    {
        new Song().Roles.ShouldNotBeNull();
    }

    [Test]
    public void AssignmentsCollection_IsInitialized()
    {
        new Song().Assignments.ShouldNotBeNull();
    }

    [Test]
    public void SongTopic_IsNullByDefault()
    {
        new Song().SongTopic.ShouldBeNull();
    }

    [Test]
    public void NotFeatured_ByDefault()
    {
        new Song().IsFeatured.ShouldBeFalse();
    }
}

[TestFixture]
[TestOf(typeof(SongRole))]
public class SongRoleTests
{
    [Test]
    public void DefaultRoleTitle_IsEmptyString()
    {
        new SongRole().RoleTitle.ShouldBeEmpty();
    }

    [Test]
    public void Assignment_IsNullByDefault()
    {
        new SongRole().Assignment.ShouldBeNull();
    }
}

[TestFixture]
[TestOf(typeof(SongRoleAssignment))]
public class SongRoleAssignmentTests
{
    [Test]
    public void UserId_IsEmptyGuidByDefault()
    {
        new SongRoleAssignment().UserId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void SongId_IsEmptyGuidByDefault()
    {
        new SongRoleAssignment().SongId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void RoleId_IsEmptyGuidByDefault()
    {
        new SongRoleAssignment().RoleId.ShouldBe(Guid.Empty);
    }
}

[TestFixture]
[TestOf(typeof(RoleTitle))]
public class RoleTitleTests
{
    [Test]
    public void DefaultTitle_IsNull()
    {
        new RoleTitle().Title.ShouldBeNull();
    }

    [Test]
    public void DefaultSvg_IsNull()
    {
        new RoleTitle().Svg.ShouldBeNull();
    }
}

[TestFixture]
[TestOf(typeof(TgAuthLink))]
public class TgAuthLinkTests
{
    [Test]
    public void DefaultTgUserId_IsNull()
    {
        new TgAuthLink().TgUserId.ShouldBeNull();
    }
}

[TestFixture]
[TestOf(typeof(DataEntry))]
public class DataEntryTests
{
    [Test]
    public void DefaultContentType_IsNull()
    {
        new DataEntry().ContentType.ShouldBeNull();
    }

    [Test]
    public void DefaultSize_IsZero()
    {
        new DataEntry().Size.ShouldBe(0L);
    }
}