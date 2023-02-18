using Microsoft.Extensions.Configuration;
using Template.MigrationRunner.Migrator;
using Microsoft.Extensions.Logging;
using Template.Data.Repositories.Shared;
using Template.Data.Contexts;
using Moq;

namespace Template.Data.Tests.Shared;

public abstract class SQLiteDatabaseBuilder<T> : IDisposable where T : class
{
    protected Mock<IConfiguration> _configuration;
    protected Mock<ILogger<T>> _logger;
    protected Runner _runner;
    protected string _dbPath;
    protected IDbSession _dbSession;

    public SQLiteDatabaseBuilder()
    {
        _configuration = new Mock<IConfiguration>();
        _logger = new Mock<ILogger<T>>();

        _dbPath = $"{Guid.NewGuid()}.sqlite";
        var mockConfSection = new Mock<IConfigurationSection>();
        var connectionString = $"Data Source={_dbPath}";
        mockConfSection.SetupGet(m => m["SQLite"]).Returns(connectionString);
        _configuration.SetupGet(x => x["DatabaseType"]).Returns("SQLite");
        _configuration.SetupGet(x => x["ConnectionStrings:SQLite"]).Returns(connectionString);
        _configuration.Setup(a => a.GetSection("ConnectionStrings")).Returns(mockConfSection.Object);

        _runner = new(_configuration.Object);
        _runner.ExecuteOption(MigratorSelect.MigrateUp);

        _dbSession = new SQLiteSession(_configuration.Object);
    }

    public void Dispose()
    {
        _dbSession.Dispose();
        File.Delete(_dbPath);
    }
}
