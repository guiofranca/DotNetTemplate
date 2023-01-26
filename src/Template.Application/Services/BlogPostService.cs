﻿using Microsoft.Extensions.Logging;
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
        ILogger<BlogPostService> logger) : base(unitOfWork, errorNotificator, cache, logger)
    {
        _blogPostRepository = blogPostRepository;
        _userRepository = userRepository;
    }

    public async Task<IServiceResult<IEnumerable<BlogPostModel>>> GetAllAsync()
    {
        var cached = await GetFromCache<IEnumerable<BlogPostModel>>("BlogPostService.GetAllAsync");
        if (cached != null) return FoundResult(cached);

        var blogPosts = await _blogPostRepository.FindAllAsync();
        var users = await _userRepository.FindAsync(blogPosts.Select(b => b.UserId).ToArray());

        var models = blogPosts.Select(b => new BlogPostModel()
        {
            Id = b.Id,
            Title = b.Title,
            Content = b.Content,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            User = users.Where(u => u.Id == b.UserId).Select(u => new UserModel { Id = u.Id, Name = u.Name}).FirstOrDefault(),
        });

        await _cache.SetAsync("BlogPostService.GetAllAsync", models);
        return FoundResult(models);
    }

    public async Task<IServiceResult<BlogPostModel>> GetAsync(Guid id)
    {
        var cached = await GetFromCache<BlogPostModel>($"BlogPostService.GetAsync.{id}");
        if (cached != null) return FoundResult(cached);

        var blogPost = await _blogPostRepository.FindAsync(id);
        if (blogPost == null) return NotFoundResult("Blog Post not found");

        User? user = await _userRepository.FindAsync(blogPost.UserId);
        if (user is null) return FailureResult("User not found");

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
        return FoundResult(model);
    }

    public async Task<IServiceResult<BlogPostModel>> CreateAsync(BlogPostRequest createBlogPostRequest, Guid UserId)
    {
        User? user = await _userRepository.FindAsync(UserId);
        if (user is null) return FailureResult("User not found");

        BlogPost blogPost = new()
        {
            Title = createBlogPostRequest.Title,
            Content = createBlogPostRequest.Content,
            UserId = UserId
        };
        await _blogPostRepository.CreateAsync(blogPost);

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

        await _cache.RemoveKey("BlogPostService.GetAllAsync");
        return CreatedResult(blogPostModel);
    }

    public async Task<IServiceResult<BlogPostModel>> EditAsync(BlogPostRequest blogPostRequest, Guid BlogPostId)
    {
        var blogPost = await _blogPostRepository.FindAsync(BlogPostId);
        if (blogPost == null) return NotFoundResult("Post not found");

        blogPost.Title = blogPostRequest.Title;
        blogPost.Content = blogPostRequest.Content;

        await _blogPostRepository.UpdateAsync(blogPost);

        var blogPostModel = new BlogPostModel() { 
            Id = blogPost.Id,
            Title = blogPostRequest.Title,
            Content = blogPostRequest.Content,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt,
        };

        await _cache.RemoveKey($"BlogPostService.GetAsync.{BlogPostId}");
        return UpdatedResult(blogPostModel);
    }

    public async Task<IServiceResult<BlogPostModel>> DeleteAsync(Guid id)
    {
        var deleted = await _blogPostRepository.DeleteAsync(id);
        return deleted 
            ? DeletedResult(null, "Post Deleted") 
            : NotFoundResult("Post not found");
    }
}
