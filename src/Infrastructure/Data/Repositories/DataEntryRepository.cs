using CuMusicClub.Domain.Abstractions;
using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuMusicClub.Infrastructure.Data.Repositories;

public sealed class DataEntryRepository(DbContext dbContext) : Repository<DataEntry>(dbContext), IDataEntryRepository
{
    public async Task<DataEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<DataEntry?> FindByHashAsync(byte[] hash, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Hash == hash, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(d => d.Id == id, cancellationToken);
    }
}