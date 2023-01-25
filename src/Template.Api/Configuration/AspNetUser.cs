using System.IdentityModel.Tokens.Jwt;
using Template.Application.Services;
using Template.Domain.Interfaces;

namespace Template.Api.Configuration;

public class AspNetUser : IUser
{
    private readonly IHttpContextAccessor _httpAcessor;
    private readonly JwtTokenService _jwt;

    public Guid Id => GetUserIdFromJwtToken();

    public bool IsAuthenticated => CheckAuthStatus();

    public string? Token => GetJwtToken();

    public AspNetUser(IHttpContextAccessor httpAcessor, JwtTokenService jwt)
    {
        _httpAcessor = httpAcessor;
        _jwt = jwt;
    }

    public bool IsInRole(string role)
    {
        throw new NotImplementedException();
    }

    private string? GetJwtToken()
    {
        var authorizationHeader = _httpAcessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault<string>();
        if (authorizationHeader == null) return null;

        var token = authorizationHeader.Replace("Bearer ", string.Empty);
        return token;
    }

    private Guid GetUserIdFromJwtToken()
    {
        var token = GetJwtToken();
        if(token is null) return Guid.Empty;
        var tokenResult = _jwt.GetPropertyFromToken(token, JwtRegisteredClaimNames.Sub);
        var userId = Guid.Parse(tokenResult.Data);
        return userId;
    }

    private bool CheckAuthStatus()
    {
        var auth = _httpAcessor.HttpContext.User.Identity?.IsAuthenticated;
        return auth ?? false;
    }
}
