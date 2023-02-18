using SqlKata.Compilers;
using System.Data;

namespace Template.Data.Repositories.Shared;

public interface IDbSession : IDisposable
{
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; set; }
    public Compiler Compiler { get; }
}
