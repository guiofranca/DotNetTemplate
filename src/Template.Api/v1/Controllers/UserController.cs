using Microsoft.AspNetCore.Mvc;
using Template.Api.v1.Attributes;
using Template.Api.v1.Controllers.Shared;
using Template.Application.DTO.User;
using Template.Application.Services;
using Template.Core.Enums;
using Template.Core.Interfaces;

namespace Template.Api.v1.Controllers;

[AuthorizeRole(Roles.Admin)]
public class UserController : V1ControllerBase
{
    private readonly UserService _userService;
    public UserController(IUser user,
                          IErrorNotificator errorNotificator,
                          UserService userService) : base(user, errorNotificator)
    {
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<UserModel>>> Get(Guid id) 
        => ResponseFromServiceResult(await _userService.GetAsync(id));

    [HttpGet("{id:guid}/detailed")]
    public async Task<ActionResult<ApiResult<DetailedUserModel>>> GetDetailed(Guid id)
        => ResponseFromServiceResult(await _userService.GetDetailedAsync(id));

    [HttpGet("")]
    public async Task<ActionResult<ApiResult<IEnumerable<UserModel>>>> GetAll()
        => ResponseFromServiceResult(await _userService.GetAllAsync());

    [HttpGet("detailed")]
    public async Task<ActionResult<ApiResult<IEnumerable<DetailedUserModel>>>> GetAllDetailed()
        => ResponseFromServiceResult(await _userService.GetAllDetailedAsync());
}
