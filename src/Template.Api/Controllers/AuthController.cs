using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.Shared;
using Template.Application.DTO.Auth;
using Template.Application.Services;

namespace Template.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : TemplateController
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResult<LoginResponse>>> Register(RegisterRequest request) => ResponseFromServiceResult(await _authService.RegisterAsync(request));

    [HttpPost("login")]
    public async Task<ActionResult<ApiResult<LoginResponse>>> Login(LoginRequest request) => ResponseFromServiceResult(await _authService.TryLoginAsync(request));

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResult<LogoutResponse>>> Logout() => ResponseFromServiceResult(await _authService.LogoutAsync(_user.Token));

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResult<LoginResponse>>> Refresh() => ResponseFromServiceResult(await _authService.RefreshTokenAsync(_user.Token));
}
