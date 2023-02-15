using Template.Domain.Interfaces.Repositories.Shared;
using Template.Domain.Models;

namespace Template.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, bool excludeOwnEmail = false);
}
