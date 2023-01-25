using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using Template.Data.Repositories.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Data.Repositories;

public class BlogPostRepository : BaseRepository<BlogPost>, IBlogPostRepository
{
    public BlogPostRepository(IDbSession dbSession, ILogger<BlogPost> logger) : base(dbSession, logger)
    {

    }

    public override async Task<BlogPost> CreateAsync(BlogPost t)
    {
        await _query.InsertAsync(new {
            t.Id,
            t.UserId,
            t.Title,
            t.Content,
            t.CreatedAt,
            t.UpdatedAt,
        });

        return t;
    }

    public override async Task<BlogPost> UpdateAsync(BlogPost t)
    {
        t.Update();
        await _query.Where(nameof(t.Id), t.Id).UpdateAsync(new {
            t.Title,
            t.Content,
            t.UpdatedAt,
        });

        return t;
    }
}