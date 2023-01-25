using Template.Domain.Interfaces;

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

    public void Commit()
    {
        if(_dbSession.Transaction is null) return;
        _dbSession.Transaction.Commit();
    }

    public void Rollback()
    {
        if(_dbSession.Transaction is null) return;
        _dbSession.Transaction.Rollback();
    }
}