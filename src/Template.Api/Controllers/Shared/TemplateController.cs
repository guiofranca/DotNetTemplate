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
    [FromServices]
    public IUser _user { get; set; }

    [FromServices]
    public IErrorNotificator _errorNotificator { get; set; }

    //protected Guid GetUserId()
    //{
    //    return GetUserIdFromJwtToken();
    //}

    //protected string? GetJwtToken()
    //{
    //    var authorizationHeader = Request.Headers.Authorization.FirstOrDefault<string>();
    //    if (authorizationHeader == null) throw new Exception("Not logged in");

    //    var token = authorizationHeader.Replace("Bearer ", string.Empty);
    //    return token;
    //}

    //private Guid GetUserIdFromJwtToken()
    //{
    //    var token = GetJwtToken();

    //    var tokenResult = _jwtTokenService.GetPropertyFromToken(token, JwtRegisteredClaimNames.Sub);

    //    var userId = Guid.Parse(tokenResult.Data);

    //    return userId;
    //}

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
            ServiceResultStatus.NotFound => NotFound(new ApiResult<T>(serviceResult, HttpStatusCode.NotFound)),
            ServiceResultStatus.Created => Ok(new ApiResult<T>(serviceResult)),
            ServiceResultStatus.Deleted => Ok(new ApiResult<T>(serviceResult)),
            ServiceResultStatus.Updated => Ok(new ApiResult<T>(serviceResult)),
            ServiceResultStatus.Error => Problem(serviceResult.Message, statusCode: (int) HttpStatusCode.BadRequest),
            _ => Problem(serviceResult.Message, statusCode: (int)HttpStatusCode.BadRequest),
        };
    }
}
