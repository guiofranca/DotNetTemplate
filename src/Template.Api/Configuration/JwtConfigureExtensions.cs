using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Template.Api.Configuration;

public static class JwtConfigureExtensions
{
    public static IServiceCollection ConfigureJwt(this IServiceCollection services, ConfigurationManager configuration) =>
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSecret = configuration["JWT:Secret"];
            if (jwtSecret == null)
            {
                throw new ArgumentNullException("appSettings.JTW.Secret");
            }

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromDays(0),
                RequireExpirationTime = true,
                RoleClaimType = "roles", //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/claims?view=aspnetcore-6.0
                //NameClaimType = "sub", //does not work. Why? :(
            };
        }).Services;
}
