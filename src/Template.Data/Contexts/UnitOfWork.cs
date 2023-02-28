using Template.Data.Repositories.Shared;
using Template.Core.Interfaces;

namespace Template.Data.Contexts;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbSession _dbSession;

    public UnitOfWork(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public void BeginTransaction()
    {
        if(_dbSession.Transaction is not null) return;
        _dbSession.Transaction = _dbSession.Connection.BeginTransaction();
    }

    public void Commit() => _dbSession.Transaction?.Commit();

    public void Rollback() => _dbSession.Transaction?.Rollback();
}