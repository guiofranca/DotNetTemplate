using Microsoft.AspNetCore.Mvc;
using Template.Api.v1.Attributes;
using Template.Api.v1.Controllers.Shared;
using Template.Application.DTO.Role;
using Template.Application.Services;
using Template.Domain.Enums;
using Template.Domain.Interfaces;

namespace Template.Api.v1.Controllers;

[AuthorizeRole(Roles.Admin)]
public class RoleController : V1ControllerBase
{
    private readonly RoleService _roleService;
    public RoleController(IUser user, IErrorNotificator errorNotificator, RoleService roleService) : base(user, errorNotificator)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<IEnumerable<RoleModel>>>> Get() 
        => ResponseFromServiceResult(await _roleService.GetAllRolesAsync());

    [HttpPost]
    public async Task<ActionResult<ApiResult<RoleModel>>> Add(RoleRequest roleRequest) 
        => ResponseFromServiceResult(await _roleService.AddRoleToUserAsync(roleRequest));

    [HttpDelete]
    public async Task<ActionResult<ApiResult<RoleModel>>> Remove(RoleRequest roleRequest)
        => ResponseFromServiceResult(await _roleService.RemoveRoleFromUserAsync(roleRequest));
}
