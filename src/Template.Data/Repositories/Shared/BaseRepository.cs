using Microsoft.Extensions.Logging;
using SqlKata;
using SqlKata.Execution;
using Template.Core.Interfaces.Repositories.Shared;
using Template.Core.Models.Shared;

namespace Template.Data.Repositories.Shared;

public abstract class BaseRepository
{
    protected readonly IDbSession _dbSession;
    protected readonly QueryFactory _db;
    protected readonly ILogger _logger;

    public BaseRepository(IDbSession dbSession, ILogger logger)
    {
        _dbSession = dbSession;
        _logger = logger;

        _db = new QueryFactory(_dbSession.Connection, _dbSession.Compiler);

        _db.Logger = compiled =>
        {
#if DEBUG
            _logger.LogDebug(compiled.ToString());
#else
            _logger.LogDebug(compiled.RawSql);
#endif
        };
    }
}

public abstract class BaseRepository<T> : BaseRepository, IBaseRepository<T> where T : Model
{
    protected readonly string Table = TableName.Of<T>();
    protected Query _query => _db.Query(Table);
    protected BaseRepository(IDbSession dbSession, ILogger logger) : base(dbSession, logger)
    {
        
    }

    public abstract Task<T> CreateAsync(T t);

    public virtual async Task<T?> FindAsync(Guid id) 
        => await _query.Where(nameof(Model.Id), id).FirstOrDefaultAsync<T?>();

    public virtual async Task<IEnumerable<T>> FindAllAsync() 
        => await _query.GetAsync<T>();

    public virtual async Task<IEnumerable<T>> FindAsync(params Guid[] ids) 
        => await _query.WhereIn(nameof(Model.Id), ids).GetAsync<T>();

    public abstract Task<T> UpdateAsync(T t);

    public virtual async Task<bool> DeleteAsync(Guid id)
        => await _query.Where(nameof(Model.Id), id).DeleteAsync() > 0;
}