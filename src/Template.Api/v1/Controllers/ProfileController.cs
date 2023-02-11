using Microsoft.AspNetCore.Mvc;
using Template.Api.v1.Attributes;
using Template.Api.v1.Controllers.Shared;
using Template.Application.DTO.Profile;
using Template.Application.DTO.User;
using Template.Application.Services;
using Template.Domain.Enums;
using Template.Domain.Interfaces;

namespace Template.Api.v1.Controllers;

[AuthorizeRole(Roles.Admin)]
public class ProfileController : V1Controller
{
    private readonly ProfileService _profileService;
    public ProfileController(IUser user,
                          IErrorNotificator errorNotificator,
                          ProfileService profileService) : base(user, errorNotificator)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<ProfileModel>>> Get() 
        => ResponseFromServiceResult(await _profileService.GetAsync());

    [HttpPatch]
    public async Task<ActionResult<ApiResult<ProfileModel>>> Patch(ProfileRequest request)
        => ResponseFromServiceResult(await _profileService.UpdateAsync(request));

    [HttpPatch("password")]
    public async Task<ActionResult<ApiResult<ProfileModel>>> UpdatePassword(PasswordRequest request)
        => ResponseFromServiceResult(await _profileService.UpdatePasswordAsync(request));
}
