using Template.Application.DTO.User;

namespace Template.Application.DTO.BlogPost;

public class BlogPostModel
{
    public Guid Id { get; set; }
    public UserModel? User { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
