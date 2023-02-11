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
    public RoleRepository(IDbSession dbSession, ILogger<Role> logger) : base(dbSession, logger)
    {
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
