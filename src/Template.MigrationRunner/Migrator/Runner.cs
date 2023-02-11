using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Template.MigrationRunner.Migrator;

public class Runner
{
    private string DatabaseType;
    private string ConnectionString;

    public Runner(IConfiguration configuration)
    {
        var configuredDatabaseType = configuration["DatabaseType"];
        if(configuredDatabaseType == null) throw new ArgumentNullException("DatabaseType");
        DatabaseType = configuredDatabaseType;

        var connectionStringKey = $"ConnectionStrings:{configuredDatabaseType}";
        var configuredConnectionString = configuration[connectionStringKey];
        if (configuredConnectionString == null) throw new ArgumentNullException(connectionStringKey);
        ConnectionString = configuredConnectionString;
    }

    public void ExecuteOption(MigratorSelect migratorSelect, long version = 0, int rollback = 0) {
        using (var serviceProvider = CreateServices())
        using (var scope = serviceProvider.CreateScope())
        {
            UpdateDatabase(scope.ServiceProvider, migratorSelect, version, rollback);
        }
    }
    private ServiceProvider CreateServices()
    {
        var provider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => {
                if (DatabaseType == "MySql") rb.AddMySql5();
                else if (DatabaseType == "Postgres") rb.AddPostgres();
                else if (DatabaseType == "SQLite") rb.AddSQLite();
                else throw new Exception($"Unknown DatabaseType {DatabaseType}");

                rb.WithGlobalConnectionString(ConnectionString)
                    .ScanIn(typeof(Runner).Assembly).For.Migrations();
            })
            .AddLogging(lb => lb.AddFluentMigratorConsole());
            
        return provider.BuildServiceProvider(false);
    }

    private void UpdateDatabase(IServiceProvider serviceProvider, MigratorSelect migratorSelect, long version = 0, int rollback = 0)
    {
        var _migrationRunner = serviceProvider.GetRequiredService<IMigrationRunner>();

        switch (migratorSelect)
        {
            case MigratorSelect.ListMigrations:
                _migrationRunner.ListMigrations();
                break;
            case MigratorSelect.MigrateUp:
                _migrationRunner.MigrateUp();
                break;
            case MigratorSelect.MigrateDown:
                _migrationRunner.MigrateDown(version);
                break;
            case MigratorSelect.Rollback:
                _migrationRunner.Rollback(rollback);
                break;
        }
    }
}