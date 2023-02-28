using Template.Core.Interfaces.Repositories.Shared;
using Template.Core.Models;

namespace Template.Core.Interfaces.Repositories;

public interface IBlogCommentRepository : IBaseRepository<BlogComment>
{
     public Task<IEnumerable<BlogComment>> CommentsFromPostAsync(Guid id);
}