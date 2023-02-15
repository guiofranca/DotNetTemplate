using Microsoft.AspNetCore.Mvc.Infrastructure;
using Template.Application.Interfaces;
using Template.Application.Services;
using Template.Data.Contexts;
using Template.Data.Repositories;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Infrastructure.Cache;
using Template.Infrastructure.FileStorage;

namespace Template.Api.Configuration;

public static class ConfigureDependenciesExtension
{
    public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
        => services.AddScoped<IUnitOfWork, UnitOfWork>()
                //.AddScoped<IDbSession, MySqlSession>()
                .AddScoped<IDbSession, PostgresSession>()
                //.AddScoped<IDbSession, SQLiteSession>()
                .AddScoped<IBlogPostRepository, BlogPostRepository>()
                .AddScoped<IBlogCommentRepository, BlogCommentRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IErrorNotificator, ErrorNotificator>()
                .AddScoped<ICacheService, CacheService>()
                .AddScoped<IUser, AspNetUser>()
                .AddScoped<IJwtTokenService, JwtTokenService>()
                .AddScoped<BlogPostService>()
                .AddScoped<BlogCommentService>()
                .AddScoped<AuthService>()
                .AddScoped<RoleService>()
                .AddScoped<StoredFileService>()
                .AddScoped<UserService>()
                .AddScoped<ProfileService>()
                .AddScoped<IFileStorage, FileSystemStorage>()
                .AddScoped<IStoredFileRepository, StoredFileRepository>()
                .AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>()
                .AddSingleton<IGlobalizer, Globalizer>();
}
