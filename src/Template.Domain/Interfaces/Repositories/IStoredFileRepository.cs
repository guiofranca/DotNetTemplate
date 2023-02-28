using Template.Core.Interfaces.Repositories.Shared;
using Template.Core.Models;

namespace Template.Core.Interfaces.Repositories;

public interface IStoredFileRepository : IBaseRepository<StoredFile>
{
    public Task<IEnumerable<StoredFile>> GetAllFromOwnerAsync(string owner, Guid ownerId);
    public Task<StoredFile?> GetOneFromOwnerAsync(string owner, Guid ownerId);
}
