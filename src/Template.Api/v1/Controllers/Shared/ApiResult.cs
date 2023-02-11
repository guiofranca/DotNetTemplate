using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Template.Application.Result;

namespace Template.Api.v1.Controllers.Shared;

public class ApiResult<T> where T : class
{
    public HttpStatusCode Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public T? Data { get; set; }

    public ApiResult(T? data = null, HttpStatusCode statusCode = HttpStatusCode.OK, string? title = null, string? detail = null)
    {
        Data = data;
        Status = statusCode;
        Title = title;
        Detail = detail;
    }

    public ApiResult(IServiceResult<T> serviceResult, string? title = null, string? detail = null)
    {
        Data = serviceResult.Data;
        Status = HttpStatusCodeFromServiceResult(serviceResult);
        Title = title ?? serviceResult.Message;
        Detail = detail;
    }

    public JsonResult Result()
    {
        return Status switch
        {
            HttpStatusCode.NotFound => new(new ProblemDetails() { Status = (int)Status, Title = Title ?? "", Detail = Detail ?? "" }) { StatusCode = (int)Status },
            HttpStatusCode.BadRequest => new(new ProblemDetails() { Status = (int)Status, Title = Title ?? "", Detail = Detail ?? "" }) { StatusCode = (int)Status },
            _ => new(this) { StatusCode = (int) Status }
        };
    }

    private static HttpStatusCode HttpStatusCodeFromServiceResult(IServiceResult<T> serviceResult)
    {
        return serviceResult.Status switch
        {
            ServiceResultStatus.Ok          => HttpStatusCode.OK,
            ServiceResultStatus.NotFound    => HttpStatusCode.NotFound,
            ServiceResultStatus.Created     => HttpStatusCode.Created,
            ServiceResultStatus.Deleted     => HttpStatusCode.OK,
            ServiceResultStatus.Updated     => HttpStatusCode.OK,
            ServiceResultStatus.Error       => HttpStatusCode.BadRequest,
            _                               => HttpStatusCode.BadRequest,
        };
    }
}
