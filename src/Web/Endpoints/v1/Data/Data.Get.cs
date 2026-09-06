using CuMusicClub.Domain.Abstractions;

namespace CuMusicClub.Web.Endpoints.v1.Data;

public static partial class Data
{
    private static async Task<IResult> Get(IDataEntryRepository dataEntries, Guid dataId, CancellationToken cancellationToken)
    {
        var entry = await dataEntries.FindByIdAsync(dataId, cancellationToken);

        if (entry == null) return TypedResults.NotFound();

        return TypedResults.File(entry.Content, entry.ContentType, enableRangeProcessing: true);
    }
}
