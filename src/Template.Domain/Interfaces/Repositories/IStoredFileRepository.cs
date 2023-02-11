using Template.Domain.Interfaces.Repositories.Shared;
using Template.Domain.Models;

namespace Template.Domain.Interfaces.Repositories;

public interface IStoredFileRepository : IBaseRepository<StoredFile>
{
    public Task<IEnumerable<StoredFile>> GetAllFromOwnerAsync(string owner, Guid ownerId);
    public Task<StoredFile?> GetOneFromOwnerAsync(string owner, Guid ownerId);
}
