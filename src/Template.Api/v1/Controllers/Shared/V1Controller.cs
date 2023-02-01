using Microsoft.AspNetCore.Mvc;
using System.Net;
using Template.Application.Result;
using Template.Domain.Interfaces;

namespace Template.Api.v1.Controllers.Shared;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public abstract class V1Controller : ControllerBase
{
    public IUser _user { get; set; }
    public IErrorNotificator _errorNotificator { get; set; }

    protected V1Controller(IUser user, IErrorNotificator errorNotificator)
    {
        _user = user;
        _errorNotificator = errorNotificator;
    }

    protected ActionResult<ApiResult<T>> ResponseFromServiceResult<T>(IServiceResult<T> serviceResult) where T : class
    {
        if (_errorNotificator.HasError())
        {
            foreach (var err in _errorNotificator.Errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
            return ValidationProblem();
        }

        return new ApiResult<T>(serviceResult).Result();
    }
}
