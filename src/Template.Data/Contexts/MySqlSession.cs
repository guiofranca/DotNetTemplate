using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Template.Domain.Interfaces;

namespace Template.Data.Contexts
{
    public sealed class MySqlSession : IDbSession, IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction? Transaction { get; set; }

        public MySqlSession(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MySql");
            if(string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Connection String Not Found on ConnectionStrings.MySql");
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}