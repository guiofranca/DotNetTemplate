using BCrypt.Net;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Template.Application.DTO.Auth;
using Template.Application.DTO.User;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Application.Services;

public class AuthService : BaseService<LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;

    public AuthService(IUserRepository userRepository,
        JwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IErrorNotificator errorNotificator,
        ICacheService _cache,
        ILogger<AuthService> logger) : base(unitOfWork, errorNotificator, _cache, logger)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<IServiceResult<LoginResponse>> TryLoginAsync(LoginRequest loginRequest)
    {
        var user = await _userRepository.GetByEmail(loginRequest.Email);
        if (user == null) return FailureResult("Credentials does not match our records.");
        var success = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);
        if (!success) return NotFoundResult("Credentials does not match our records.");

        user.Roles = await _userRepository.GetRolesAsync(user);
        var tokenResult = await _jwtTokenService.GenerateTokenAsync(user, user.Roles.Select(r => r.Name));
        if (tokenResult.IsError) return FailureResult(tokenResult.Message);
        var model = new LoginResponse 
        { 
            Token = tokenResult.Data,
            User = new UserModel 
            { 
                Id = user.Id,
                Name = user.Name 
            }
        };

        return FoundResult(model);
    }

    public async Task<IServiceResult<LogoutResponse>> LogoutAsync(string token)
    {
        var result = await _jwtTokenService.LogoutUser(token);
        if(result.IsError) return FailureResult<LogoutResponse>(result.Message);
        return OkResult<LogoutResponse>(new());
    }

    public async Task<IServiceResult<LoginResponse>> RegisterAsync(RegisterRequest registerRequest)
    {

        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password, salt);
        var user = new User() { Name = registerRequest.Name, Email = registerRequest.Email, Password = passwordHash };
        await _userRepository.CreateAsync(user);

        var tokenResult = await _jwtTokenService.GenerateTokenAsync(user, user.Roles.Select(r => r.Name));
        if (tokenResult.IsError) return FailureResult(tokenResult.Message);
        var model = new LoginResponse
        {
            Token = tokenResult.Data,
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
        var userId = new Guid(userIdResult.Data);
        

        var user = await _userRepository.FindAsync(userId);
        if (user == null) return NotFoundResult("User not found");

        user.Roles = await _userRepository.GetRolesAsync(user);
        var newTokenResult = await _jwtTokenService.GenerateTokenAsync(user, user.Roles.Select(r => r.Name), token);
        if (newTokenResult.IsError) return FailureResult(newTokenResult.Message);

        var model = new LoginResponse
        {
            Token = newTokenResult.Data,
            User = new UserModel
            {
                Id = user.Id,
                Name = user.Name
            }
        };
        return FoundResult(model);
    }
}
