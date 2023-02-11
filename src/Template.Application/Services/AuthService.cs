using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Template.Application.DTO.Auth;
using Template.Application.DTO.User;
using Template.Application.Interfaces;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Application.Services;

public class AuthService : BaseService<LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IErrorNotificator errorNotificator,
        ICacheService _cache,
        ILogger<AuthService> logger,
        IGlobalizer globalizer) : base(unitOfWork, errorNotificator, _cache, logger, globalizer)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<IServiceResult<LoginResponse>> TryLoginAsync(LoginRequest loginRequest)
    {
        var user = await _userRepository.GetByEmail(loginRequest.Email);
        if (user == null) return FailureResult(_g["Credentials does not match our records."]);
        var success = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);
        if (!success) return FailureResult(_g["Credentials does not match our records."]);

        if (BCrypt.Net.BCrypt.PasswordNeedsRehash(user.Password, 12))
        {
            user.Password = HashPassword(loginRequest.Password);
            await _userRepository.UpdateAsync(user);
        }

        user.Roles = await _userRepository.GetRolesAsync(user);
        var tokenResult = await _jwtTokenService.GenerateTokenAsync(user, user.Roles.Select(r => r.Name));
        if (tokenResult.IsError) return FailureResult(tokenResult.Message);
        var model = new LoginResponse 
        { 
            Token = tokenResult.Data!,
            User = new UserModel 
            { 
                Id = user.Id,
                Name = user.Name 
            }
        };

        return OkResult(model);
    }

    public async Task<IServiceResult<LogoutResponse>> LogoutAsync(string token)
    {
        var result = await _jwtTokenService.LogoutUser(token);
        if(result.IsError) return FailureResult<LogoutResponse>(result.Message);
        return OkResult<LogoutResponse>(new());
    }

    public async Task<IServiceResult<LoginResponse>> RegisterAsync(RegisterRequest registerRequest)
    {
        var passwordHash = HashPassword(registerRequest.Password);

        var taken = await _userRepository.GetByEmail(registerRequest.Email);
        if (taken is not null) return FailureResult(_g["This e-mail has already been taken"]);

        var user = new User() { Name = registerRequest.Name, Email = registerRequest.Email, Password = passwordHash, Verified = true };
        await _cache.RememberModelAsync(user, _userRepository.CreateAsync);

        var tokenResult = await _jwtTokenService.GenerateTokenAsync(user, user.Roles.Select(r => r.Name));
        if (tokenResult.IsError) return FailureResult(tokenResult.Message);
        var model = new LoginResponse
        {
            Token = tokenResult.Data!,
            User = new UserModel
            {
                Id = user.Id,
                Name = user.Name
            }
        };
        return CreatedResult(model);
    }

    public async Task<IServiceResult<LoginResponse>> RefreshTokenAsync(string token)
    {
        var userIdResult = _jwtTokenService.GetPropertyFromToken(token, JwtRegisteredClaimNames.Sub);
        if (userIdResult.IsError) return FailureResult(userIdResult.Message);
        var userId = new Guid(userIdResult.Data!);
        

        var user = await _cache.RememberModelAsync(userId, _userRepository.FindAsync);
        if (user == null) return NotFoundResult(_g["User not found"]);

        user.Roles = await _userRepository.GetRolesAsync(user);
        var newTokenResult = await _jwtTokenService.GenerateTokenAsync(user, user.Roles.Select(r => r.Name), token);
        if (newTokenResult.IsError) return FailureResult(newTokenResult.Message);

        var model = new LoginResponse
        {
            Token = newTokenResult.Data!,
            User = new UserModel
            {
                Id = user.Id,
                Name = user.Name
            }
        };
        return OkResult(model);
    }

    private string HashPassword(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }
}
