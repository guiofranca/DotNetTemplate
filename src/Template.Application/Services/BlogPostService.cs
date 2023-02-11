using Microsoft.Extensions.Logging;
using Template.Application.DTO.BlogPost;
using Template.Application.DTO.User;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Application.Services;

public class BlogPostService : BaseService<BlogPostModel>
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IUserRepository _userRepository;

    public BlogPostService(IBlogPostRepository blogPostRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IErrorNotificator errorNotificator,
        ICacheService cache,
        ILogger<BlogPostService> logger,
        IGlobalizer globalizer) : base(unitOfWork, errorNotificator, cache, logger, globalizer)
    {
        _blogPostRepository = blogPostRepository;
        _userRepository = userRepository;
    }

    public async Task<IServiceResult<IEnumerable<BlogPostModel>>> GetAllAsync()
    {
        var cached = await GetFromCache<IEnumerable<BlogPostModel>>("BlogPostService.GetAllAsync");
        if (cached != null) return OkResult(cached);

        var blogPosts = await _blogPostRepository.FindAllAsync();
        var users = await _userRepository.FindAsync(blogPosts.Select(b => b.UserId).Distinct().ToArray());

        var models = blogPosts.Select(b => new BlogPostModel()
        {
            Id = b.Id,
            Title = b.Title,
            Content = b.Content,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            User = users.Where(u => u.Id == b.UserId).Select(u => new UserModel { Id = u.Id, Name = u.Name}).FirstOrDefault(),
        });

        return OkResult(models);
    }

    public async Task<IServiceResult<BlogPostModel>> GetAsync(Guid id)
    {
        var blogPost = await _cache.RememberModelAsync(id, _blogPostRepository.FindAsync);
        if (blogPost == null) return NotFoundResult(_g["Blog Post not found"]);

        User? user = await _cache.RememberModelAsync(blogPost.UserId, _userRepository.FindAsync);
        if (user is null) return FailureResult(_g["User not found"]);

        UserModel userModel = new()
        {
            Id = user.Id,
            Name = user.Name,
        };

        var model = new BlogPostModel()
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt,
            User = userModel,
        };

        await _cache.SetAsync($"BlogPostService.GetAsync.{id}", model);
        return OkResult(model);
    }

    public async Task<IServiceResult<BlogPostModel>> CreateAsync(BlogPostRequest createBlogPostRequest, Guid UserId)
    {
        User? user = await _cache.RememberModelAsync(UserId, _userRepository.FindAsync);
        if (user is null) return FailureResult(_g["User not found"]);

        BlogPost blogPost = new()
        {
            Title = createBlogPostRequest.Title,
            Content = createBlogPostRequest.Content,
            UserId = UserId
        };
        await _cache.RememberModelAsync(blogPost, _blogPostRepository.CreateAsync);

        UserModel userModel = new()
        {
            Id = UserId,
            Name = user.Name,
        };

        BlogPostModel blogPostModel = new()
        {
            Id = blogPost.Id,
            Title = createBlogPostRequest.Title,
            Content = createBlogPostRequest.Content,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt,
            User = userModel,
        };

        return CreatedResult(blogPostModel);
    }

    public async Task<IServiceResult<BlogPostModel>> EditAsync(BlogPostRequest blogPostRequest, Guid BlogPostId)
    {
        var blogPost = await _cache.RememberModelAsync(BlogPostId, _blogPostRepository.FindAsync);
        if (blogPost == null) return NotFoundResult(_g["Blog Post not found"]);

        blogPost.Title = blogPostRequest.Title;
        blogPost.Content = blogPostRequest.Content;

        await _cache.RememberModelAsync(blogPost, _blogPostRepository.UpdateAsync);

        var blogPostModel = new BlogPostModel() { 
            Id = blogPost.Id,
            Title = blogPostRequest.Title,
            Content = blogPostRequest.Content,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt,
        };

        return UpdatedResult(blogPostModel);
    }

    public async Task<IServiceResult<BlogPostModel>> DeleteAsync(Guid id)
    {
        var cacheKey = $"{typeof(BlogPost).Name}.{id}";
        var deleted = await _blogPostRepository.DeleteAsync(id);
        await _cache.RemoveKey(cacheKey);
        return deleted 
            ? DeletedResult(null, _g["Blog Post Deleted"]) 
            : NotFoundResult(_g["Blog Post not found"]);
    }
}
