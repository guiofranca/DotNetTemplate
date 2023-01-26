using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Template.Api.Configuration;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app/*, ILogger<Exception> logger*/)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    //logger.LogError($"Something went wrong: {contextFeature.Error}");
                    await context.Response.WriteAsync(JsonSerializer.Serialize(
                        new ProblemDetails()
                        {
                            Status = context.Response?.StatusCode,
                            Title = "Internal Server Error.",
                        })
                    );
                }
            });
        });
    }
}
