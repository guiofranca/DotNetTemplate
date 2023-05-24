using Template.Core.Models.Components;

namespace Template.Core.Models;

public class BlogPost : IModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public IEnumerable<BlogComment> BlogComments { get; set; } = Enumerable.Empty<BlogComment>();
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
