using Template.Core.Models.Components;

namespace Template.Core.Models;

public class BlogComment : IModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid? BlogCommentId { get; set; }
    public BlogComment? ReplyTo { get; set; }
    public required string Content { get; set; } = default!;
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
