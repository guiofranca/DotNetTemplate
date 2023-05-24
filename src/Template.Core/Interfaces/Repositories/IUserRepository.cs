using Template.Core.Interfaces.Repositories.Shared;
using Template.Core.Models;

namespace Template.Core.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, bool excludeOwnEmail = false);
}
