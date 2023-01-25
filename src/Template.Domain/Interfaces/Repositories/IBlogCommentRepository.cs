using Template.Domain.Interfaces.Repositories.Shared;
using Template.Domain.Models;

namespace Template.Domain.Interfaces.Repositories;

public interface IBlogCommentRepository : IBaseRepository<BlogComment>
{
     public Task<IEnumerable<BlogComment>> CommentsFromPost(Guid id);
}