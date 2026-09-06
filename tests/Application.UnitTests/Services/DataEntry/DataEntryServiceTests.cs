using System.Security.Cryptography;
using CuMusicClub.Application.Services.DataEntry;
using CuMusicClub.Domain.Abstractions;
using Moq;
using NUnit.Framework;
using Shouldly;
using DataEntryEntity = CuMusicClub.Domain.Entities.DataEntry;

namespace CuMusicClub.Application.UnitTests.Services.DataEntry;

[TestFixture]
[TestOf(typeof(DataEntryService))]
public class DataEntryServiceTests
{
    private Mock<IDataEntryRepository> _repository = null!;
    private DataEntryService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IDataEntryRepository>();
        _service = new DataEntryService(_repository.Object);
    }

    [Test]
    public void Create_EmptyContent_Throws()
    {
        Should.Throw<InvalidOperationException>(() => _service.Create([], "image/png", CancellationToken.None));
    }

    [Test]
    public async Task Create_NewContent_AddsAndSaves()
    {
        var content = new byte[] { 1, 2, 3 };
        var hash = SHA256.HashData(content);

        DataEntryEntity? added = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<DataEntryEntity>(), It.IsAny<CancellationToken>()))
            .Callback<DataEntryEntity, CancellationToken>((e, _) => added = e)
            .Returns(Task.CompletedTask);
        _repository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.Create(content, "image/png", CancellationToken.None);

        result.ShouldNotBeNull();
        result.Hash.ShouldBe(hash);
        result.ContentType.ShouldBe("image/png");
        result.Size.ShouldBe(3);
        added.ShouldNotBeNull();
        added!.Id.ShouldBe(result.Id);
    }

    [Test]
    public async Task Create_ExistingHash_ReturnsExistingWithoutAdding()
    {
        var content = new byte[] { 1, 2, 3 };
        var hash = SHA256.HashData(content);
        var existing = new DataEntryEntity
        {
            Id = Guid.NewGuid(),
            Content = content,
            Hash = hash,
            ContentType = "image/png",
            Size = 3,
        };

        _repository
            .Setup(r => r.FindByHashAsync(hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _service.Create(content, "image/png", CancellationToken.None);

        result.ShouldBe(existing);
        _repository.Verify(r => r.AddAsync(It.IsAny<DataEntryEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Create_SameContentTwice_ReturnsSameEntry()
    {
        var content = new byte[] { 9, 9, 9 };

        DataEntryEntity? added = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<DataEntryEntity>(), It.IsAny<CancellationToken>()))
            .Callback<DataEntryEntity, CancellationToken>((e, _) => added = e)
            .Returns(Task.CompletedTask);
        _repository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var first = await _service.Create(content, "image/png", CancellationToken.None);
        // второй вызов вернёт тот же существующий объект из БД
        _repository
            .Setup(r => r.FindByHashAsync(first.Hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(first);

        var second = await _service.Create(content, "image/png", CancellationToken.None);

        second.ShouldBe(first);
        second.Id.ShouldBe(first.Id);
    }
}