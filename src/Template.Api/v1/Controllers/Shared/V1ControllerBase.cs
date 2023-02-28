using Microsoft.AspNetCore.Mvc;
using System.Net;
using Template.Application.Result;
using Template.Core.Interfaces;

namespace Template.Api.v1.Controllers.Shared;

[ApiController]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class V1ControllerBase : ControllerBase
{
    public IUser _user { get; set; }
    public IErrorNotificator _errorNotificator { get; set; }

    protected V1ControllerBase(IUser user, IErrorNotificator errorNotificator)
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
