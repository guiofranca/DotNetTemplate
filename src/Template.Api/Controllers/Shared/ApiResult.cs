using System.Net;
using Template.Application.Result;

namespace Template.Api.Controllers.Shared;

public class ApiResult<T> where T : class
{
    public HttpStatusCode Status { get; set; }
    public string? Title { get; set; }
    public T? Data { get; set; }

    public ApiResult(T? data = null, HttpStatusCode statusCode = HttpStatusCode.OK, string? title = null)
    {
        Data = data;
        Status = statusCode;
        Title = title;
    }

    public ApiResult(IServiceResult<T> serviceResult, HttpStatusCode statusCode = HttpStatusCode.OK, string? title = null)
    {
        Data = serviceResult.Data;
        Status = statusCode;
        Title = title ?? serviceResult.Message;
    }
}
