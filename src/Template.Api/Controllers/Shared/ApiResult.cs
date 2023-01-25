using System.Net;
using Template.Application.Result;

namespace Template.Api.Controllers.Shared;

public class ApiResult<T> where T : class
{
    public HttpStatusCode Status { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public ApiResult(T? data = null, HttpStatusCode statusCode = HttpStatusCode.OK, string? message = null)
    {
        Data = data;
        Status = statusCode;
        Message = message;
    }

    public ApiResult(IServiceResult<T> serviceResult, HttpStatusCode statusCode = HttpStatusCode.OK, string? message = null)
    {
        Data = serviceResult.Data;
        Status = statusCode;
        Message = message ?? serviceResult.Message;
    }
}
