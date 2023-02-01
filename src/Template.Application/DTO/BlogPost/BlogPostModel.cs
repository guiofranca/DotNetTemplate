using Template.Application.DTO.User;

namespace Template.Application.DTO.BlogPost;

public class BlogPostModel
{
    public Guid Id { get; set; }
    public UserModel? User { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
