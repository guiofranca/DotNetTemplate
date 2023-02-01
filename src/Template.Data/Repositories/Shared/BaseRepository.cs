﻿using Microsoft.Extensions.Logging;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories.Shared;
using Template.Domain.Models.Shared;

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

        var compiler = new MySqlCompiler();
        _db = new QueryFactory(_dbSession.Connection, compiler);

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
    protected BaseRepository(IDbSession dbSession, ILogger<T> logger) : base(dbSession, logger)
    {
        //_query = _db.Query(Table);
    }

    public abstract Task<T> CreateAsync(T t);

    public virtual async Task<T?> FindAsync(Guid id)
    {
        T? model = await _query.Where(nameof(Model.Id), id).FirstOrDefaultAsync<T?>();

        return model;
    }

    public virtual async Task<IEnumerable<T>> FindAllAsync()
    {
        var model = await _query.GetAsync<T>();

        return model;
    }

    public virtual async Task<IEnumerable<T>> FindAsync(params Guid[] ids) => 
        await _query.WhereIn(nameof(Model.Id), ids).GetAsync<T>();

    public abstract Task<T> UpdateAsync(T t);

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var affected = await _db.Query(TableName.Of<T>()).Where(nameof(Model.Id), id).DeleteAsync();
        return affected > 0;
    }

}