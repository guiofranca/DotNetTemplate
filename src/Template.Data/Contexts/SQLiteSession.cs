using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;
using Template.Domain.Interfaces;

namespace Template.Data.Contexts
{
    public sealed class SQLiteSession : IDbSession, IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction? Transaction { get; set; }

        public SQLiteSession(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SQLite");
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Connection String Not Found on ConnectionStrings.SQLite");
            Connection = new SqliteConnection(connectionString);

            SqlMapper.AddTypeHandler<Guid>(new SQLiteGuidTypeHandler());

            Connection.Open();
        }

        public void Dispose()
        {
            Connection?.Close();
            SqliteConnection.ClearAllPools();
        }
    }

    public class SQLiteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid) => parameter.Value = guid.ToString();

        public override Guid Parse(object value) => new((string)value);
    }
}
