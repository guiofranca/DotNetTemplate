using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Interfaces;

namespace Template.Data.Contexts
{
    public sealed class PostgresSession : IDbSession, IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction? Transaction { get; set; }

        public PostgresSession(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Connection String Not Found on ConnectionStrings.Postgres");
            Connection = new NpgsqlConnection(connectionString);
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}
