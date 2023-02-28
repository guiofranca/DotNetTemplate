using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Npgsql;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Data.Repositories.Shared;
using Template.Core.Interfaces;

namespace Template.Data.Contexts
{
    public sealed class PostgresSession : IDbSession, IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction? Transaction { get; set; }
        public Compiler Compiler { get; }

        public PostgresSession(IConfiguration configuration)
        {
            Compiler = new PostgresCompiler();
            var connectionString = configuration.GetConnectionString("Postgres");
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Connection String Not Found on ConnectionStrings.Postgres");
            Connection = new NpgsqlConnection(connectionString);
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}
