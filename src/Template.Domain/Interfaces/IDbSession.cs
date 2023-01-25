using System.Data;

namespace Template.Domain.Interfaces;

public interface IDbSession
{
     public IDbConnection Connection { get; }
     public IDbTransaction? Transaction { get; set; }
}
