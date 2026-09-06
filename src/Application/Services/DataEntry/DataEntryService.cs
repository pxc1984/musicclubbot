using System.Security.Cryptography;
using CuMusicClub.Application.Services.DataEntry;
using CuMusicClub.Domain.Abstractions;

namespace CuMusicClub.Application.Services.DataEntry;

public class DataEntryService(IDataEntryRepository dataEntries) : IDataEntryService
{
    public async Task<Domain.Entities.DataEntry> Create(byte[] content, string contentType, CancellationToken cancellationToken)
    {
        if (content.Length == 0) throw new InvalidOperationException("Data Entry content cannot be empty");

        var hash = SHA256.HashData(content);

        var existing = await dataEntries.FindByHashAsync(hash, cancellationToken);
        if (existing is not null) return existing;

        var entry = new Domain.Entities.DataEntry
        {
            Id = Guid.NewGuid(),
            Content = content,
            Hash = hash,
            ContentType = contentType,
            Size = content.LongLength,
        };

        await dataEntries.AddAsync(entry, cancellationToken);
        await dataEntries.SaveChangesAsync(cancellationToken);

        return entry;
    }
}