using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Serilog;
using Template.Api.Configuration;
using Template.Application.Services;
using Template.Data.Contexts;
using Template.Data.Repositories;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Infrastructure.Cache;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Template.Application.Interfaces;
using Template.Application.Resources;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
RequestLocalizationOptions localizationOptions = new()
{
    ApplyCurrentCultureToResponseHeaders = true
};
localizationOptions.AddSupportedCultures("en-US", "pt-BR")
    .AddSupportedUICultures("en-US", "pt-BR")
    .SetDefaultCulture("en-US");

builder.Services.AddControllers()
    .AddDataAnnotationsLocalization(options => {
    options.DataAnnotationLocalizerProvider = (type, factory) =>
        factory.Create(typeof(SharedResource));
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization()
    .ConfigureJwt(builder.Configuration);


builder.ConfigureSwagger();

Log.Information("Starting Application");
Log.Logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .CreateLogger();

builder.Host.UseSerilog();


//IoC
builder.Services.AddScoped<IDbSession, MySqlSession>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IErrorNotificator, ErrorNotificator>();
builder.Services.AddScoped<ICacheService, CacheService>();
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUser, AspNetUser>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<BlogPostService>();
builder.Services.AddScoped<BlogCommentService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();
builder.Services.AddSingleton<IGlobalizer, Globalizer>();


var app = builder.Build();

app.UseRequestLocalization(localizationOptions);

app.UseApiVersioning();

app.UseSerilogRequestLogging();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.ConfigureSwagger(apiVersionDescriptionProvider);

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();