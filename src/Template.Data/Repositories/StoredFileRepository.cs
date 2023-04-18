using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using Template.Data.Repositories.Shared;
using Template.Core.Interfaces.Repositories;
using Template.Core.Models;

namespace Template.Data.Repositories;

public class StoredFileRepository : BaseRepository<StoredFile>, IStoredFileRepository
{
    public StoredFileRepository(IDbSession dbSession, ILogger<StoredFileRepository> logger) : base(dbSession, logger)
    {
    }

    public async Task<IEnumerable<StoredFile>> GetAllFromOwnerAsync(string owner, Guid ownerId)
    {
        var storedFiles = await _query
            .Where(nameof(StoredFile.Owner), owner)
            .Where(nameof(StoredFile.OwnerId), ownerId)
            .GetAsync<StoredFile>();

        return storedFiles;
    }

    public async Task<StoredFile?> GetOneFromOwnerAsync(string owner, Guid ownerId)
    {
        var storedFile = await _query
            .Where(nameof(StoredFile.Owner), owner)
            .Where(nameof(StoredFile.OwnerId), ownerId)
            .FirstOrDefaultAsync<StoredFile>();

        return storedFile;
    }

    public async override Task<StoredFile> CreateAsync(StoredFile t)
    {
        await _query.InsertAsync(new
        {
            t.Id,
            t.Name,
            t.Size,
            t.Owner,
            t.OwnerId,
            t.CreatedAt,
            t.UpdatedAt,
        });

        return t;
    }

    public async override Task<StoredFile> UpdateAsync(StoredFile t)
    {
        t.UpdatedAt = DateTime.Now;
        await _query.Where(nameof(t.Id), t.Id).UpdateAsync(new
        {
            t.Owner,
            t.OwnerId,
            t.UpdatedAt,
        });

        return t;
    }
}
