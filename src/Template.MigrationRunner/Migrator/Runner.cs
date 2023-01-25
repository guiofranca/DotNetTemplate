using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Template.MigrationRunner.Migrator;

public class Runner
{
    private readonly IConfiguration _configuration;

    public Runner(IConfiguration configuration)
    {
        _configuration = configuration;
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
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddMySql5()
                .WithGlobalConnectionString(_configuration.GetConnectionString("Default"))
                .ScanIn(typeof(Runner).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
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