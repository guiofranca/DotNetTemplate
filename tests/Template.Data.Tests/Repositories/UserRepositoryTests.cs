using Dapper;
using Template.Data.Repositories;
using Template.Data.Tests.Shared;
using Template.Core.Models;

namespace Template.Data.Tests.Repositories;

public class UserRepositoryTests : SQLiteDatabaseBuilder<UserRepository>
{
    private UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _userRepository = new(_dbSession, _logger.Object);
        Seed();
    }

    [Fact]
    public async Task CreateAsync_UserIsPersistedWhenAdded()
    {
        var expected = NewUser(6);
        await _userRepository.CreateAsync(expected);

        var actual = await _userRepository.FindAsync(expected.Id);

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("Test_1", true, false)]
    [InlineData("Test_1", false, true)]
    [InlineData("Unknown", true, false)]
    [InlineData("Unknown", false, false)]
    public async Task EmailExistsAsync_CheckThatEmailExists(string email, bool excludeOwnEmail, bool expected)
    {
        var actual = await _userRepository.EmailExistsAsync(email, excludeOwnEmail);

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task FindByEmailAsync_ReturnsUser()
    {
        var user1 = NewUser(1);

        var actual = await _userRepository.FindByEmailAsync(user1.Email);

        actual!.Id.Should().Be(Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25ca"));
    }

    [Fact]
    public async Task FindByEmailAsync_ReturnsNullWhenNotFound()
    {
        var notFound = await _userRepository.FindByEmailAsync("Unknown");

        notFound.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WorksForSelectedFields()
    {
        var user1 = await _userRepository.FindAsync(Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25ca"));
        user1!.Name = "Updated";
        user1!.Email = "Updated";
        user1!.Password = "Updated";

        await _userRepository.UpdateAsync(user1);
        var actual = await _userRepository.FindAsync(user1!.Id);

        actual.Should().BeEquivalentTo(user1);
    }

    private static User NewUser(int id = 1) => new() {
        Name = $"Test_{id}",
        Email = $"Test_{id}",
        Password = $"Test_{id}",
        Verified = true,
    };

    private void Seed()
    {
        _dbSession.Connection.Execute(
            "INSERT INTO users (Id, Name, Email, Password, Verified, CreatedAt, UpdatedAt) " +
            "VALUES " +
            "(@id1, 'Test_1', 'Test_1', 'Test_1', true, '2023-01-01', '2023-01-01'), " +
            "(@id2, 'Test_2', 'Test_2', 'Test_2', true, '2023-01-01', '2023-01-01'), " +
            "(@id3, 'Test_3', 'Test_3', 'Test_3', true, '2023-01-01', '2023-01-01'), " +
            "(@id4, 'Test_4', 'Test_4', 'Test_4', true, '2023-01-01', '2023-01-01'), " +
            "(@id5, 'Test_5', 'Test_5', 'Test_5', true, '2023-01-01', '2023-01-01');", new
            {
                id1 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25ca"),
                id2 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25cb"),
                id3 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25cc"),
                id4 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25cd"),
                id5 = Guid.Parse("4ce7430e-a99e-4245-b4de-870ce68e25ce"),
            });
    }
}
