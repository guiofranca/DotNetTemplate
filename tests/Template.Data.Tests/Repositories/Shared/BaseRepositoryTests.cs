using Dapper;
using Microsoft.Extensions.Logging;
using Template.Data.Repositories.Shared;
using Template.Data.Tests.Shared;
using Template.Core.Models.Components;

namespace Template.Data.Tests.Repositories.Shared;

public class BaseRepositoryTests : SQLiteDatabaseBuilder<DummyRepository>
{
    private readonly DummyRepository _dummyRepository;
    public BaseRepositoryTests()
    {
        _dbSession.Connection.Execute("""CREATE TABLE "dummys" ("Id" UNIQUEIDENTIFIER NOT NULL, "Name" TEXT NOT NULL, "CreatedAt" DATETIME NOT NULL, "UpdatedAt" DATETIME NOT NULL, CONSTRAINT "PK_dummys" PRIMARY KEY ("Id"));""");
        Seed();
        _dummyRepository = new(_dbSession, _logger.Object);
    }

    [Theory]
    [InlineData("4ce7430e-a99e-4245-b4de-870ce68e25ca", "Dummy1")]
    [InlineData("4ce7430e-a99e-4245-b4de-870ce68e25cc", "Dummy3")]
    public async Task FindAsync_SingleId(string guidString, string expectedName)
    {
        var actual = await _dummyRepository.FindAsync(Guid.Parse(guidString));

        actual!.Name.Should().Be(expectedName);
    }

    [Fact]
    public async Task FindAsync_SingleId_ReturnNullWhenNotFound()
    {
        var actual = await _dummyRepository.FindAsync(Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68AAAAA"));

        actual.Should().BeNull();
    }

    [Fact]
    public async Task FindAsync_ManyIds()
    {
        var id1 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25ca");
        var id2 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25cb");
        var id3 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25cc");

        var actual = await _dummyRepository.FindAsync(id1, id2, id3);

        actual.Count().Should().Be(3);
    }

    [Fact]
    public async Task FindAsync_ManyIds_ReturnsEmptyListWhenNoneFound()
    {
        var id1 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25fa");
        var id2 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25fb");
        var id3 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25fc");

        var actual = await _dummyRepository.FindAsync(id1, id2, id3);

        actual.Count().Should().Be(0);
    }

    [Fact]
    public async Task FindAllAsync()
    {
        var actual = await _dummyRepository.FindAllAsync();

        actual.Count().Should().Be(5);
    }

    [Theory]
    [InlineData("4ce7430e-a99e-4245-b4de-870ce68e25ca", true)]
    [InlineData("4ce7430e-a99e-4245-b4de-870ce68e25da", false)]
    public async Task DeleteAsync(string guidString, bool expected)
    {
        var id = new Guid(guidString);
        var deleted = await _dummyRepository.DeleteAsync(id);
        deleted.Should().Be(expected);
    }

    private void Seed()
    {
        _dbSession.Connection.Execute(
            "INSERT INTO dummys (Id, Name, CreatedAt, UpdatedAt) " +
            "VALUES " +
            "(@id1, 'Dummy1', '2023-01-01', '2023-01-01'), " +
            "(@id2, 'Dummy2', '2023-01-01', '2023-01-01'), " +
            "(@id3, 'Dummy3', '2023-01-01', '2023-01-01'), " +
            "(@id4, 'Dummy4', '2023-01-01', '2023-01-01'), " +
            "(@id5, 'Dummy5', '2023-01-01', '2023-01-01');", new
            {
                id1 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25ca"),
                id2 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25cb"),
                id3 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25cc"),
                id4 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25cd"),
                id5 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25ce"),
            });
    }
}

public class DummyRepository : BaseRepository<Dummy>
{
    public DummyRepository(IDbSession dbSession, ILogger<DummyRepository> logger) : base(dbSession, logger)
    {
    }

    public override Task<Dummy> CreateAsync(Dummy t) => throw new NotImplementedException();

    public override Task<Dummy> UpdateAsync(Dummy t) => throw new NotImplementedException();
}

public class Dummy : IModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}