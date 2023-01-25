using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.MigrationRunner.ConsoleMenu;
using Template.MigrationRunner.Migrator;

namespace Template.MigrationRunner.Configuration;

public static class Configure
{
    public static IHost Build(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false);
#if DEBUG
                builder.AddJsonFile($"appsettings.Development.json", optional: true);
#endif
            })
            .ConfigureServices(services =>
            {
                services.AddScoped<Runner>();
                services.AddScoped<MenuRunner>();
            })
            .Build();

        return host;
    }
}
