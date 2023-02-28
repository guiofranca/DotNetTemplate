using Serilog;
using Template.Api.Configuration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Template.Application.Resources;
using Template.Infrastructure.Queue.Configuration;
using Template.Infrastructure.Queue;
using Coravel.Queuing.Interfaces;
using Coravel;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
});

//IoC
builder.Services.ConfigureDependencies();

builder.Services.AddCoravelQueue();
//builder.Services.AddScoped<JobOne>();

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseRequestLocalization(localizationOptions);

app.UseSerilogRequestLogging();

app.ConfigureSwagger();

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
