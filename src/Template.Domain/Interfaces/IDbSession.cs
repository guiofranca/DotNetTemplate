using System.Data;

namespace Template.Domain.Interfaces;

public interface IDbSession : IDisposable
{
     public IDbConnection Connection { get; }
     public IDbTransaction? Transaction { get; set; }
}
