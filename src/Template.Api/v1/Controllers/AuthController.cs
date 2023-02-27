using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.v1.Controllers.Shared;
using Template.Application.DTO.Auth;
using Template.Application.Services;
using Template.Domain.Interfaces;

namespace Template.Api.v1.Controllers;

public class AuthController : V1ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService, IUser user, IErrorNotificator errorNotificator) : base(user, errorNotificator)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResult<LoginResponse>>> Register(RegisterRequest request) => ResponseFromServiceResult(await _authService.RegisterAsync(request));

    [HttpPost("login")]
    public async Task<ActionResult<ApiResult<LoginResponse>>> Login(LoginRequest request) => ResponseFromServiceResult(await _authService.TryLoginAsync(request));

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResult<LogoutResponse>>> Logout() => ResponseFromServiceResult(await _authService.LogoutAsync(_user.Token!));

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResult<LoginResponse>>> Refresh() => ResponseFromServiceResult(await _authService.RefreshTokenAsync(_user.Token!));
}
