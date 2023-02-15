using Dapper;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using Template.Data.Repositories.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Data.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    private readonly string UserRoleTable = "user_role";

    public RoleRepository(IDbSession dbSession, ILogger<Role> logger) : base(dbSession, logger)
    {
    }

    public async Task<IEnumerable<Role>> GetAsync(User user)
    {
        var roles = await _query
            .Join(UserRoleTable, $"{Table}.Id", $"{UserRoleTable}.RoleId")
            .Where("UserId", user.Id)
            .GetAsync<Role>();

        return roles;
    }

    public async Task<bool> AddAsync(User user, Role role)
    {
        var affected = await _db.Query(UserRoleTable).InsertAsync(new
        {
            UserId = user.Id,
            RoleId = role.Id,
            CreatedAt = DateTime.Now,
        });

        return affected > 0;
    }

    public async Task<bool> RemoveAsync(User user, Role role)
    {
        var affected = await _db.Query(UserRoleTable)
            .Where("UserId", user.Id)
            .Where("RoleId", role.Id)
            .DeleteAsync();

        return affected > 0;
    }

    public override Task<Role> CreateAsync(Role t)
    {
        throw new NotImplementedException();
    }

    public override Task<Role> UpdateAsync(Role t)
    {
        throw new NotImplementedException();
    }
}
