using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.Shared;
using Template.Application.DTO.BlogComment;
using Template.Application.DTO.BlogPost;
using Template.Application.Services;

namespace Template.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class BlogPostController : TemplateController
{
    public ILogger<BlogPostController> _logger;

    private readonly BlogPostService _blogPostService;
    private readonly BlogCommentService _blogCommentService;

    public BlogPostController(BlogPostService blogPostService, ILogger<BlogPostController> logger, BlogCommentService blogCommentService)
    {
        _blogPostService = blogPostService;
        _logger = logger;
        _blogCommentService = blogCommentService;
    }

    [HttpGet("")]
    public async Task<ActionResult<ApiResult<IEnumerable<BlogPostModel>>>> Index() =>
        ResponseFromServiceResult(await _blogPostService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<BlogPostModel>>> Get(Guid id) =>
        ResponseFromServiceResult(await _blogPostService.GetAsync(id));

    [HttpGet("{id:guid}/comments")]
    public async Task<ActionResult<ApiResult<IEnumerable<BlogCommentModel>>>> GetComments(Guid id) =>
        ResponseFromServiceResult(await _blogCommentService.GetAllFromPost(id));

    [HttpPost("")]
    public async Task<ActionResult<ApiResult<BlogPostModel>>> Create(BlogPostRequest request) => 
        ResponseFromServiceResult(await _blogPostService.CreateAsync(request, _user.Id));

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ApiResult<BlogPostModel>>> Edit(BlogPostRequest request, Guid id) =>
        ResponseFromServiceResult(await _blogPostService.EditAsync(request, id));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult<BlogPostModel>>> Delete(Guid id) =>
        ResponseFromServiceResult(await _blogPostService.DeleteAsync(id));
}
