using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Template.Application.Interfaces;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Core.Interfaces;
using Template.Core.Models;

namespace Template.Application.Services;

public class JwtTokenService : BaseService<string>, IJwtTokenService
{
    private byte[] Secret { get; set; }
    private int TokenExpirationInMinutes { get; set; }
    private int RefreshTokenExpiryInDays { get; set; }

    //https://busk.blog/2022/01/31/authentication-with-jwt-in-net-6/
    //https://dev.to/moe23/refresh-jwt-with-refresh-tokens-in-asp-net-core-5-rest-api-step-by-step-3en5
    //https://www.c-sharpcorner.com/article/jwt-authentication-with-refresh-tokens-in-net-6-0/

    public JwtTokenService(IConfiguration config,
        IUnitOfWork unitOfWork,
        ICacheService cache,
        IErrorNotificator errorNotificator,
        ILogger<JwtTokenService> logger,
        IGlobalizer globalizer) : base(unitOfWork, errorNotificator, cache, logger, globalizer)
    {
        var jwtSecret = config["JWT:Secret"];
        var jwtTokenExpiration = config["JWT:TokenExpirationInMinutes"];
        var jwtRefreshExpiration = config["JWT:RefreshTokenExpiryInDays"];
        if (jwtSecret == null) throw new ArgumentNullException("appSettings:JWT:Secret");
        if (jwtTokenExpiration == null) throw new ArgumentNullException("appSettings:JWT:TokenExpirationInMinutes");
        if (jwtRefreshExpiration == null) throw new ArgumentNullException("appSettings:JWT:RefreshTokenExpiryInDays");

        Secret = Encoding.UTF8.GetBytes(jwtSecret);
        TokenExpirationInMinutes = int.Parse(jwtTokenExpiration);
        RefreshTokenExpiryInDays = int.Parse(jwtRefreshExpiration);
    }

    public async Task<IServiceResult<string>> GenerateTokenAsync(User user, IEnumerable<string> roles, string? oldToken = null)
    {
        var banResult = await CheckIfTokenIsAbleToRefresh(oldToken);
        if (banResult.IsError) return FailureResult(banResult.Message);

        var jti = await GenerateJtiAsync();

        string expirationTimestamp = new DateTimeOffset(DateTime.Now.AddMinutes(TokenExpirationInMinutes)).ToUnixTimeSeconds().ToString();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, expirationTimestamp),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim("roles", role));
            claims.Add(new Claim("roles", role));
        }

        var header = new JwtHeader(
            new SigningCredentials(
                new SymmetricSecurityKey(Secret),
                    SecurityAlgorithms.HmacSha256));

        var payload = new JwtPayload(claims);

        var token = new JwtSecurityToken(header, payload);

        var textToken = new JwtSecurityTokenHandler().WriteToken(token);

        return OkResult(textToken);
    }

    private async Task<IServiceResult<string>> CheckIfTokenIsAbleToRefresh(string? oldToken)
    {
        if (oldToken is null) return OkResult("Fresh token generation");

        var oldJtiResult = GetPropertyFromToken(oldToken, JwtRegisteredClaimNames.Jti);
        if (oldJtiResult.IsError) return FailureResult(oldJtiResult.Message);

        var isValid = await _cache.GetStringAsync($"{JwtRegisteredClaimNames.Jti}.{oldJtiResult.Data}");
        await _cache.RemoveKey($"{JwtRegisteredClaimNames.Jti}.{oldJtiResult.Data}");
        if (isValid is null) return FailureResult(_g["This token can no longer be refreshed. Login again."]);
        return OkResult(_g["Token valid"]);
    }

    private async Task<string> GenerateJtiAsync()
    {
        var jti = Guid.NewGuid().ToString();
        await _cache.SetStringAsync($"{JwtRegisteredClaimNames.Jti}.{jti}", "valid", TimeSpan.FromDays(RefreshTokenExpiryInDays));
        return jti;
    }

    public IServiceResult<string> GetPropertyFromToken(string? token, string claim)
    {
        if (token is null) return FailureResult(_g["Please provide a Bearer token"]);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Secret),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        //https://stackoverflow.com/questions/42036810/asp-net-core-jwt-mapping-role-claims-to-claimsidentity
        tokenHandler.InboundClaimTypeMap.Clear();
        tokenHandler.InboundClaimTypeMap.Add(ClaimTypes.NameIdentifier, "sub");

        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return FailureResult(_g["Invalid token"]);
            }

        }
        catch (Exception)
        {
            return FailureResult(_g["Invalid token"]);
        }

        if (!principal.HasClaim(c => c.Type == claim))
        {
            return FailureResult(_g["Invalid token"]);
        }

        var value = principal.Claims.First(c => c.Type == claim).Value;
        return OkResult(value);
    }

    public async Task<IServiceResult<string>> LogoutUser(string token)
    {
        var jtiResult = GetPropertyFromToken(token, JwtRegisteredClaimNames.Jti);
        if (jtiResult.IsError) return FailureResult(jtiResult.Message);
        await _cache.RemoveKey($"{JwtRegisteredClaimNames.Jti}.{jtiResult.Data}");
        return OkResult(_g["Logged out"]);
    }
}

