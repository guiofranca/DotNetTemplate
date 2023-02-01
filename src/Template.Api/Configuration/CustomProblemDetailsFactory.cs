using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Template.Domain.Interfaces;

namespace Template.Api.Configuration;

public class CustomProblemDetailsFactory : ProblemDetailsFactory
{
    private readonly IGlobalizer _g;
    public CustomProblemDetailsFactory(IGlobalizer g)
    {
        _g = g;
    }
    public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        return new()
        {
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance,
            Status = statusCode,            
        };
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        return new(modelStateDictionary)
        {
            Title = title ?? _g["One or more validation errors occurred"],
            Status = statusCode,
            Type = type,
            Detail = detail,
            Instance = instance
        };
    }
}
