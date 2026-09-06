using CuMusicClub.Domain.Entities;

namespace CuMusicClub.Domain.Abstractions;

/// <summary>Репозиторий загруженных файлов.</summary>
public interface IDataEntryRepository : IRepository<DataEntry>
{
    Task<DataEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DataEntry?> FindByHashAsync(byte[] hash, CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
}