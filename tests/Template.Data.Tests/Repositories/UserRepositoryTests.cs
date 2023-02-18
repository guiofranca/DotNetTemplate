using FluentAssertions;
using Template.Data.Repositories;
using Template.Data.Tests.Shared;
using Template.Domain.Models;

namespace Template.Data.Tests.Repositories;

public class UserRepositoryTests : SQLiteDatabaseBuilder<UserRepository>
{
    private UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _userRepository = new(_dbSession, _logger.Object);
    }

    [Fact]
    public async Task CreateAsync_UserIsPersistedWhenAdded()
    {
        var expected = NewUser();

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
        await _userRepository.CreateAsync(NewUser(1));
        var actual = await _userRepository.EmailExistsAsync(email, excludeOwnEmail);

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task FindByEmailAsync()
    {
        var user1 = await _userRepository.CreateAsync(NewUser(1));

        var actual = await _userRepository.FindByEmailAsync(user1.Email);

        actual!.Id.Should().Be(user1.Id);
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
        var user1 = await _userRepository.CreateAsync(NewUser(1));
        user1.Name = "Updated";
        user1.Email = "Updated";
        user1.Password = "Updated";

        await _userRepository.UpdateAsync(user1);
        var actual = await _userRepository.FindAsync(user1.Id);

        actual.Should().BeEquivalentTo(user1);
    }

    private static User NewUser(int id = 1) => new() {
        Name = $"Test_{id}",
        Email = $"Test_{id}",
        Password = $"Test_{id}",
        Verified = true,
    };
}
