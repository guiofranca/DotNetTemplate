using Microsoft.Extensions.Logging;
using Template.Application.DTO.Auth;
using Template.Application.DTO.Profile;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Application.Services;

public class ProfileService : BaseService<ProfileModel>
{
    private readonly IUserRepository _userRepository;
    private readonly IUser _user;
    public ProfileService(IUnitOfWork unitOfWork,
        IErrorNotificator errorNotificator,
        ICacheService cache,
        ILogger<ProfileService> logger,
        IGlobalizer globalizer,
        IUserRepository userRepository,
        IUser user) : base(unitOfWork, errorNotificator, cache, logger, globalizer)
    {
        _userRepository = userRepository;
        _user = user;
    }

    public async Task<IServiceResult<ProfileModel>> GetAsync()
    {
        var user = await _cache.RememberModelAsync(_user.Id, _userRepository.FindAsync);
        if (user == null) return NotFoundResult(_g["User not found"]);

        var model = MapToProfile(user);

        return OkResult(model);
    }

    public async Task<IServiceResult<ProfileModel>> UpdateAsync(ProfileRequest request)
    {
        var user = await _cache.RememberModelAsync(_user.Id, _userRepository.FindAsync);
        if (user == null) return NotFoundResult(_g["User not found"]);

        user.Name = request.Name;
        user.Email = request.Email;

        await _cache.RememberModelAsync(user, _userRepository.UpdateAsync);

        var model = MapToProfile(user);

        return UpdatedResult(model, _g["Profile updated"]);
    }

    public async Task<IServiceResult<ProfileModel>> UpdatePasswordAsync(PasswordRequest request)
    {
        var user = await _cache.RememberModelAsync(_user.Id, _userRepository.FindAsync);
        if (user == null) return NotFoundResult(_g["User not found"]);

        var success = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);
        if (!success) return FailureResult("Credentials does not match our records.");

        //ReHashPass
        user.Password = HashPassword(request.NewPassword);
        await _cache.RememberModelAsync(user, _userRepository.UpdateAsync);

        var model = MapToProfile(user);

        return UpdatedResult(model, _g["Password updated"]);
    }

    private static ProfileModel MapToProfile(User user) => new()
    {
        Name = user.Name,
        Email = user.Email,
    };

    private string HashPassword(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }
}
