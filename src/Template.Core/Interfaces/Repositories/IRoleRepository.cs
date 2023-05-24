using Template.Core.Interfaces.Repositories.Shared;
using Template.Core.Models;

namespace Template.Core.Interfaces.Repositories;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<IEnumerable<Role>> GetAsync(User user);
    Task<bool> AddAsync(User user, Role role);
    Task<bool> RemoveAsync(User user, Role role);
}
