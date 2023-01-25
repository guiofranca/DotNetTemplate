using Template.Domain.Interfaces.Repositories.Shared;
using Template.Domain.Models;

namespace Template.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmail(string email);
    Task<bool> EmailExistsAsync(string email, bool excludeOwnEmail = false);
    Task<IEnumerable<Role>> GetRolesAsync(User user);
    Task<bool> AddRoleAsync(User user, Role role);
    Task<bool> RemoveRoleAsync(User user, Role role);
}
