using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.Shared;
using Template.Application.DTO.BlogComment;
using Template.Application.DTO.BlogPost;
using Template.Application.Services;
using Template.Domain.Interfaces;

namespace Template.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class BlogCommentController : TemplateController
{
    private readonly BlogCommentService _blogCommentService;

    public BlogCommentController(BlogCommentService blogCommentService, IUser user, IErrorNotificator errorNotificator) : base(user, errorNotificator)
    {
        _blogCommentService = blogCommentService;
    }

    [HttpGet("")]
    public async Task<ActionResult<ApiResult<IEnumerable<BlogCommentModel>>>> Index() 
        => ResponseFromServiceResult(await _blogCommentService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<BlogCommentModel>>> Get(Guid id) 
        => ResponseFromServiceResult(await _blogCommentService.GetAsync(id));

    [HttpPost("")]
    public async Task<ActionResult<ApiResult<BlogCommentModel>>> Create(BlogCommentRequest request) 
        => ResponseFromServiceResult(await _blogCommentService.CreateAsync(request, _user.Id));

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ApiResult<BlogCommentModel>>> Edit(BlogCommentRequest request, Guid id) 
        => ResponseFromServiceResult(await _blogCommentService.EditAsync(request, id));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult<BlogCommentModel>>> Delete(Guid id) =>
        ResponseFromServiceResult(await _blogCommentService.DeleteAsync(id));
}
