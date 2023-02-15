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

        Assert.Multiple(() =>
        {
            Assert.Equal(expected.Id, actual!.Id);
            Assert.Equal(expected.Name, actual!.Name);
            Assert.Equal(expected.Email, actual!.Email);
            Assert.Equal(expected.Password, actual!.Password);
            Assert.Equal(expected.Verified, actual!.Verified);
        });
    }

    [Fact]
    public async Task FindAllAsync_TakesAllUsers()
    {
        await _userRepository.CreateAsync(NewUser(1));
        await _userRepository.CreateAsync(NewUser(2));

        var users = await _userRepository.FindAllAsync();

        Assert.Equal(2, users.Count());
    }

    [Fact]
    public async Task FindAsync_FindsByManyIds()
    {
        var user1 = await _userRepository.CreateAsync(NewUser(1));
        var user2 = await _userRepository.CreateAsync(NewUser(2));

        var users = await _userRepository.FindAsync(user1.Id, user2.Id);

        Assert.Multiple(() =>
        {
            Assert.Equal(2, users.Count());
            Assert.Contains(users, u => u.Id == user1.Id);
            Assert.Contains(users, u => u.Id == user2.Id);
        });
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrueIfDeleted()
    {
        var user1 = await _userRepository.CreateAsync(NewUser(1));

        var deleted = await _userRepository.DeleteAsync(user1.Id);
        var notDeleted = await _userRepository.DeleteAsync(Guid.NewGuid());

        Assert.Multiple(() =>
        {
            Assert.True(deleted);
            Assert.False(notDeleted);
        });
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

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetByEmail_GetsUserByEmail()
    {
        var user1 = await _userRepository.CreateAsync(NewUser(1));

        var actual = await _userRepository.FindByEmailAsync(user1.Email);
        var notFound = await _userRepository.FindByEmailAsync("Unknown");

        Assert.Multiple(() =>
        {
            Assert.Equal(user1.Id, actual!.Id);
            Assert.Null(notFound);
        });
    }

    [Fact]
    public async Task FindByEmailAsync_GetsUserByEmail()
    {
        var user1 = await _userRepository.CreateAsync(NewUser(1));

        var actual = await _userRepository.FindByEmailAsync(user1.Email);
        var notFound = await _userRepository.FindByEmailAsync("Unknown");

        Assert.Multiple(() =>
        {
            Assert.Equal(user1.Id, actual!.Id);
            Assert.Null(notFound);
        });
    }

    [Fact]
    public async Task UpdateAsync_Works()
    {
        var user1 = await _userRepository.CreateAsync(NewUser(1));
        user1.Name = "Updated";

        await _userRepository.UpdateAsync(user1);
        var actual = await _userRepository.FindAsync(user1.Id);

        Assert.Equal(user1.Name, actual!.Name);
    }

    private static User NewUser(int id = 1) => new() {
        Name = $"Test_{id}",
        Email = $"Test_{id}",
        Password = $"Test_{id}",
        Verified = true,
    };
}
