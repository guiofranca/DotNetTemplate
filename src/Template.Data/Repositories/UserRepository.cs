using Dapper;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using Template.Data.Repositories.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly string RoleTable = TableName.Of<Role>();
    private readonly string UserRoleTable = "user_role";
    public UserRepository(IDbSession dbSession, ILogger<User> logger) : base(dbSession, logger)
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
        var query = _db.Query(Table).Where("Email", email);
        if(excludeOwnEmail)
        {
            query.WhereNot("Email", email);
        }
        var exists = await query.FirstOrDefaultAsync();
        return exists is not null;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _db.Query(Table)
            .Where("Email", email)
            .FirstOrDefaultAsync<User?>();

        return user;
    }

    public async Task<IEnumerable<Role>> GetRolesAsync(User user)
    {
        var roles = await _db.Query(RoleTable)
            .Join(UserRoleTable, $"{RoleTable}.Id", $"{UserRoleTable}.RoleId")
            .Where("UserId", user.Id)
            .GetAsync<Role>();

        return roles;
    }

    public Task<bool> AddRoleAsync(User user, Role role)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveRoleAsync(User user, Role role)
    {
        throw new NotImplementedException();
    }

    public override async Task<User> UpdateAsync(User t)
    {
        t.Update();
        await _query.Where(nameof(t.Id), t.Id).UpdateAsync(new
        {
            t.Password,
            t.UpdatedAt,
        });

        return t;
    }
}
