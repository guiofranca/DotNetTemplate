﻿using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using Template.Data.Repositories.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Interfaces.Repositories.Shared;
using Template.Domain.Models;

namespace Template.Data.Repositories;

public class BlogCommentRepository : BaseRepository<BlogComment>, IBlogCommentRepository
{
    public BlogCommentRepository(IDbSession dbSession, ILogger<BlogComment> logger) : base(dbSession, logger)
    {
    }

    public async Task<IEnumerable<BlogComment>> CommentsFromPostAsync(Guid id) 
        => await _query.Where("BlogPostId", id).OrderByDesc("CreatedAt").GetAsync<BlogComment>();

    public async override Task<BlogComment> CreateAsync(BlogComment t)
    {
        await _query.InsertAsync(new 
        { 
            t.Id,
            t.UserId,
            t.BlogPostId,
            t.BlogCommentId,
            t.Content,
            t.CreatedAt, 
            t.UpdatedAt,
        });
        return t;
    }

    public async override Task<BlogComment> UpdateAsync(BlogComment t)
    {
        t.Update();
        await _query.Where("Id", t.Id).UpdateAsync(new 
        {
            t.Content,
            t.UpdatedAt,
        });
        return t;
    }
}
