using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using Template.Data.Repositories.Shared;
using Template.Core.Interfaces.Repositories;
using Template.Core.Models;

namespace Template.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IDbSession dbSession, ILogger<UserRepository> logger) : base(dbSession, logger)
    {
    }

    public override async Task<User> CreateAsync(User user)
    {
        await _db.Query(Table).InsertAsync(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.Password,
            user.Verified,
            user.CreatedAt,
            user.UpdatedAt,
        });
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email, bool excludeOwnEmail = false)
    {
        var query = _db.Query(Table).Where(nameof(User.Email), email);
        if(excludeOwnEmail)
        {
            query.WhereNot(nameof(User.Email), email);
        }
        var exists = await query.FirstOrDefaultAsync();
        return exists is not null;
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        var user = await _db.Query(Table)
            .Where(nameof(User.Email), email)
            .FirstOrDefaultAsync<User?>();

        return user;
    }

    public override async Task<User> UpdateAsync(User t)
    {
        t.UpdatedAt = DateTime.Now;
        await _query.Where(nameof(t.Id), t.Id).UpdateAsync(new
        {
            t.Name,
            t.Email,
            t.Password,
            t.UpdatedAt,
        });

        return t;
    }
}
