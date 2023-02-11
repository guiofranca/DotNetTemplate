using Template.Domain.Models.Shared;

namespace Template.Domain.Models;

public class BlogPost : Model
{
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public IEnumerable<BlogComment> BlogComments { get; set; } = Enumerable.Empty<BlogComment>();
}
