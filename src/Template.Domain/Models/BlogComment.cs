using Template.Core.Models.Shared;

namespace Template.Core.Models;

public class BlogComment : Model
{
    public required Guid BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid? BlogCommentId { get; set; }
    public BlogComment? ReplyTo { get; set; }
    public required string Content { get; set; } = default!;
}
