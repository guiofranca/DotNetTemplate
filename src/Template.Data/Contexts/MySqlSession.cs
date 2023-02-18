using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using SqlKata.Compilers;
using Template.Data.Repositories.Shared;

namespace Template.Data.Contexts
{
    public sealed class MySqlSession : IDbSession, IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction? Transaction { get; set; }
        public Compiler Compiler { get; }

        public MySqlSession(IConfiguration configuration)
        {
            Compiler = new MySqlCompiler();
            var connectionString = configuration.GetConnectionString("MySql");
            if(string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Connection String Not Found on ConnectionStrings.MySql");
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}