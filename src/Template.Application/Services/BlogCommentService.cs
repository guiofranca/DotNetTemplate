using Microsoft.Extensions.Logging;
using Template.Application.DTO.BlogComment;
using Template.Application.DTO.User;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Application.Services;

public class BlogCommentService : BaseService<BlogCommentModel>
{
    private readonly IBlogCommentRepository _blogCommentRepository;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IUserRepository _userRepository;

    public BlogCommentService(IBlogCommentRepository blogCommentRepository,
        IUserRepository userRepository,
        IBlogPostRepository blogPostRepository,
        IUnitOfWork unitOfWork,
        IErrorNotificator errorNotificator,
        ICacheService _cache,
        ILogger<AuthService> logger) : base(unitOfWork, errorNotificator, _cache, logger)
    {
        _blogCommentRepository = blogCommentRepository;
        _userRepository = userRepository;
        _blogPostRepository = blogPostRepository;
    }

    public async Task<IServiceResult<IEnumerable<BlogCommentModel>>> GetAllFromPost(Guid postId)
    {
        var blogComments = await _blogCommentRepository.CommentsFromPost(postId);
        var users = await _userRepository.FindAsync(blogComments.Select(b => b.UserId).ToArray());

        return FoundResult(blogComments.Select(b => new BlogCommentModel 
        { 
            Id = b.Id,
            Content = b.Content,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            BlogPostId = b.BlogPostId,
            User = users.Where(u => u.Id == b.UserId).Select(u => new UserModel() { Name = u.Name, Id= u.Id }).FirstOrDefault()
        }));
    }

    public async Task<IServiceResult<IEnumerable<BlogCommentModel>>> GetAllAsync()
    {
        var blogComments = await _blogCommentRepository.FindAllAsync();
        var users = await _userRepository.FindAsync(blogComments.Select(b => b.UserId).ToArray());

        var models = blogComments.Select(b => new BlogCommentModel()
        {
            Id = b.Id,
            Content = b.Content,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            User = users.Where(u => u.Id == b.UserId).Select(u => new UserModel { Id = u.Id, Name = u.Name}).FirstOrDefault(),
        });

        return FoundResult(models);
    }

    public async Task<IServiceResult<BlogCommentModel>> GetAsync(Guid id)
    {
        var blogComment = await _blogCommentRepository.FindAsync(id);
        if (blogComment == null) return NotFoundResult("Blog Comment not found");

        User? user = await _userRepository.FindAsync(blogComment.UserId);
        if (user is null) return FailureResult("User not found");

        UserModel userModel = new()
        {
            Id = user.Id,
            Name = user.Name,
        };

        var model = new BlogCommentModel()
        {
            Id = blogComment.Id,
            Content = blogComment.Content,
            CreatedAt = blogComment.CreatedAt,
            UpdatedAt = blogComment.UpdatedAt,
            User = userModel,
        };

        return FoundResult(model);
    }

    public async Task<IServiceResult<BlogCommentModel>> CreateAsync(BlogCommentRequest createBlogCommentRequest, Guid UserId)
    {
        User? user = await _userRepository.FindAsync(UserId);
        if (user is null) return FailureResult("User not found");

        BlogComment blogComment = new()
        {
            Content = createBlogCommentRequest.Content,
            BlogPostId = createBlogCommentRequest.BlogPostId,
            UserId = UserId,
            BlogCommentId = createBlogCommentRequest.BlogCommentId,
        };
        await _blogCommentRepository.CreateAsync(blogComment);

        UserModel userModel = new()
        {
            Id = UserId,
            Name = user.Name,
        };

        BlogCommentModel blogCommentModel = new()
        {
            Id = blogComment.Id,
            Content = createBlogCommentRequest.Content,
            BlogPostId = createBlogCommentRequest.BlogPostId,
            CreatedAt = blogComment.CreatedAt,
            UpdatedAt = blogComment.UpdatedAt,
            User = userModel,
        };

        return CreatedResult(blogCommentModel);
    }

    public async Task<IServiceResult<BlogCommentModel>> EditAsync(BlogCommentRequest blogCommentRequest, Guid BlogCommentId)
    {
        var blogComment = new BlogComment() {
            Id = BlogCommentId,
            BlogPostId = blogCommentRequest.BlogPostId,
            Content = blogCommentRequest.Content,
        };

        await _blogCommentRepository.UpdateAsync(blogComment);
        blogComment = await _blogCommentRepository.FindAsync(BlogCommentId);

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
}
