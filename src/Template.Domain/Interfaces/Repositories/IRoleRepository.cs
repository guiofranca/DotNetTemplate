using Template.Domain.Interfaces.Repositories.Shared;
using Template.Domain.Models;

namespace Template.Domain.Interfaces.Repositories;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<IEnumerable<Role>> GetAsync(User user);
    Task<bool> AddAsync(User user, Role role);
    Task<bool> RemoveAsync(User user, Role role);
}
