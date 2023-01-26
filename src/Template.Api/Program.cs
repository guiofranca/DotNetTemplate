using Serilog;
using Template.Api.Configuration;
using Template.Application.Services;
using Template.Data.Contexts;
using Template.Data.Repositories;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Infrastructure.Cache;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.ConfigureSwagger();

builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization()
    .ConfigureJwt(builder.Configuration);

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
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<BlogPostService>();
builder.Services.AddScoped<BlogCommentService>();
builder.Services.AddScoped<AuthService>();


var app = builder.Build();

app.UseSerilogRequestLogging();

app.ConfigureSwagger();

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
