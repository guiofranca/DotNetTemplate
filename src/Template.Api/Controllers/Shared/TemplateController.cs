using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Template.Application.Result;
using Template.Application.Services;
using Template.Domain.Interfaces;

namespace Template.Api.Controllers.Shared;

[ApiController]
public abstract class TemplateController : ControllerBase
{
    public IUser _user { get; set; }
    public IErrorNotificator _errorNotificator { get; set; }

    protected TemplateController(IUser user, IErrorNotificator errorNotificator)
    {
        _user = user;
        _errorNotificator = errorNotificator;
    }

    protected ActionResult<ApiResult<T>> ResponseFromServiceResult<T>(IServiceResult<T> serviceResult) where T : class
    {
        if(_errorNotificator.HasError())
        {
            foreach(var err in _errorNotificator.Errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
            return ValidationProblem();
        }

        return serviceResult.Status switch
        {
            ServiceResultStatus.Ok => Ok(new ApiResult<T>(serviceResult)),
            ServiceResultStatus.Found => Ok(new ApiResult<T>(serviceResult)),
            ServiceResultStatus.NotFound => Problem(title: serviceResult.Message, statusCode: (int) HttpStatusCode.NotFound),
            ServiceResultStatus.Created => Ok(new ApiResult<T>(serviceResult)),
            ServiceResultStatus.Deleted => Ok(new ApiResult<T>(serviceResult)),
            ServiceResultStatus.Updated => Ok(new ApiResult<T>(serviceResult)),
            ServiceResultStatus.Error => Problem(serviceResult.Message, statusCode: (int) HttpStatusCode.BadRequest),
            _ => Problem(title: serviceResult.Message, statusCode: (int)HttpStatusCode.BadRequest),
        };
    }
}
