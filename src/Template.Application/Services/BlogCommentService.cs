using Microsoft.Extensions.Logging;
using Template.Application.DTO.BlogComment;
using Template.Application.DTO.User;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Core.Interfaces;
using Template.Core.Interfaces.Repositories;
using Template.Core.Models;

namespace Template.Application.Services;

public class BlogCommentService : BaseService<BlogCommentModel>
{
    private readonly IBlogCommentRepository _blogCommentRepository;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IUserRepository _userRepository;

    public BlogCommentService(IBlogCommentRepository blogCommentRepository, IUserRepository userRepository,
                              IBlogPostRepository blogPostRepository, IUnitOfWork unitOfWork,
                              IErrorNotificator errorNotificator, ICacheService _cache, ILogger<AuthService> logger,
                              IGlobalizer globalizer) : base(unitOfWork, errorNotificator, _cache, logger, globalizer)
    {
        _blogCommentRepository = blogCommentRepository;
        _userRepository = userRepository;
        _blogPostRepository = blogPostRepository;
    }

    public async Task<IServiceResult<IEnumerable<BlogCommentModel>>> GetAllFromPost(Guid postId)
    {
        var blogComments = await _blogCommentRepository.CommentsFromPostAsync(postId);
        var users = await _userRepository.FindAsync(blogComments.Select(b => b.UserId).Distinct().ToArray());

        return OkResult(blogComments.Select(b => new BlogCommentModel 
        { 
            Id = b.Id,
            Content = b.Content,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            BlogPostId = b.BlogPostId,
            User = users.Where(u => u.Id == b.UserId).Select(u => new UserModel() { Name = u.Name, Id = u.Id }).FirstOrDefault(),
        }));
    }

    public async Task<IServiceResult<IEnumerable<BlogCommentModel>>> GetAllAsync()
    {
        var blogComments = await _blogCommentRepository.FindAllAsync();
        var users = await _userRepository.FindAsync(blogComments.Select(b => b.UserId).Distinct().ToArray());

        var models = blogComments.Select(b => new BlogCommentModel()
        {
            Id = b.Id,
            Content = b.Content,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            User = users.Where(u => u.Id == b.UserId).Select(u => new UserModel { Id = u.Id, Name = u.Name}).FirstOrDefault()!,
        });

        return OkResult(models);
    }

    public async Task<IServiceResult<BlogCommentModel>> GetAsync(Guid id)
    {
        var blogComment = await _cache.RememberModelAsync(id, _blogCommentRepository.FindAsync);
        if (blogComment == null) return NotFoundResult(_g["Blog Comment not found"]);

        User? user = await _cache.RememberModelAsync(blogComment.UserId, _userRepository.FindAsync);
        if (user is null) return FailureResult(_g["User not found"]);

        UserModel userModel = new()
        {
            Id = user.Id,
            Name = user.Name,
        };

        var model = new BlogCommentModel()
        {
            Id = blogComment.Id,
            BlogPostId = blogComment.BlogPostId,
            Content = blogComment.Content,
            CreatedAt = blogComment.CreatedAt,
            UpdatedAt = blogComment.UpdatedAt,
            User = userModel,
        };

        return OkResult(model);
    }

    public async Task<IServiceResult<BlogCommentModel>> CreateAsync(BlogCommentRequest createBlogCommentRequest, Guid UserId)
    {
        User? user = await _cache.RememberModelAsync(UserId, _userRepository.FindAsync);
        if (user is null) return FailureResult(_g["User not found"]);

        BlogPost? blogPost = await _cache.RememberModelAsync(createBlogCommentRequest.BlogPostId,_blogPostRepository.FindAsync);
        if (blogPost == null) return NotFoundResult(_g["Blog Post not found"]);

        BlogComment blogComment = new()
        {
            Content = createBlogCommentRequest.Content,
            BlogPostId = createBlogCommentRequest.BlogPostId,
            UserId = UserId,
            BlogCommentId = createBlogCommentRequest.BlogCommentId,
        };
        await _cache.RememberModelAsync(blogComment, _blogCommentRepository.CreateAsync);

        UserModel userModel = new()
        {
            Id = UserId,
            Name = user.Name,
        };

        BlogCommentModel model = new()
        {
            Id = blogComment.Id,
            Content = createBlogCommentRequest.Content,
            BlogPostId = createBlogCommentRequest.BlogPostId,
            CreatedAt = blogComment.CreatedAt,
            UpdatedAt = blogComment.UpdatedAt,
            User = userModel,
        };

        return CreatedResult(model);
    }

    public async Task<IServiceResult<BlogCommentModel>> EditAsync(BlogCommentRequest blogCommentRequest, Guid BlogCommentId)
    {
        var blogComment = await _cache.RememberModelAsync(BlogCommentId, _blogCommentRepository.FindAsync);
        if (blogComment == null) return NotFoundResult(_g["Blog Comment not found"]);

        blogComment.Content = blogCommentRequest.Content;

        await _cache.RememberModelAsync(blogComment, _blogCommentRepository.UpdateAsync);


        var blogCommentModel = new BlogCommentModel() { 
            Id = blogComment.Id,
            BlogPostId = blogComment.BlogPostId,
            BlogCommentId = blogComment.BlogCommentId,
            Content = blogCommentRequest.Content,
            CreatedAt = blogComment.CreatedAt,
            UpdatedAt = blogComment.UpdatedAt,
        };

        return UpdatedResult(blogCommentModel);
    }

    public async Task<IServiceResult<BlogCommentModel>> DeleteAsync(Guid id)
    {
        var cacheKey = $"{typeof(BlogComment).Name}.{id}";
        var deleted = await _blogCommentRepository.DeleteAsync(id);
        await _cache.RemoveKey(cacheKey);
        return deleted 
            ? DeletedResult(null, _g["Blog Comment Deleted"]) 
            : NotFoundResult(_g["Blog Comment not found"]);
    }
}
