using Microsoft.AspNetCore.Mvc;
using Template.Api.v1.Controllers.Shared;
using Template.Application.DTO.StoredFile;
using Template.Application.Services;
using Template.Domain.Interfaces;

namespace Template.Api.v1.Controllers;

public class StoredFileController : V1Controller
{
    private readonly StoredFileService _storedFileService;
    public StoredFileController(IUser user, IErrorNotificator errorNotificator, StoredFileService storedFileService) : base(user, errorNotificator)
    {
        _storedFileService = storedFileService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<StoredFileModel>>> Get(Guid Id)
        => ResponseFromServiceResult(await _storedFileService.GetAsync(Id));

    [HttpGet("{owner}/{ownerId:guid}/all")]
    public async Task<ActionResult<ApiResult<IEnumerable<StoredFileModel>>>> GetAllFromOwner(string owner, Guid ownerId)
        => ResponseFromServiceResult(await _storedFileService.GetAllFromOwnerAsync(owner, ownerId));

    [HttpGet("{owner}/{ownerId:guid}/one")]
    public async Task<ActionResult<ApiResult<StoredFileModel>>> GetOneFromOwner(string owner, Guid ownerId)
        => ResponseFromServiceResult(await _storedFileService.GetOneFromOwnerAsync(owner, ownerId));

    [HttpPost]
    public async Task<ActionResult<ApiResult<StoredFileModel>>> Upload(IFormFile file) 
        => ResponseFromServiceResult(await _storedFileService.UploadAsync(file.OpenReadStream(), file.FileName));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult<StoredFileModel>>> Delete(Guid Id)
        => ResponseFromServiceResult(await _storedFileService.DeleteAsync(Id));

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ApiResult<StoredFileModel>>> Patch(Guid Id, AssignFileRequest assignFileRequest)
        => ResponseFromServiceResult(await _storedFileService.AssignFileAsync(Id, assignFileRequest));
}
