using Template.Domain.Models.Shared;

namespace Template.Domain.Models;

public class BlogComment : Model
{
    public Guid BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid? BlogCommentId { get; set; }
    public BlogComment? ReplyTo { get; set; }
    public string Content { get; set; }
}
