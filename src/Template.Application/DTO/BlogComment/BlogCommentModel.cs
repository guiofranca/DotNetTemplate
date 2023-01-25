using System.ComponentModel.DataAnnotations;
using Template.Application.DTO.User;

namespace Template.Application.DTO.BlogComment;

public class BlogCommentModel
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public UserModel User { get; set; }
    public Guid? BlogCommentId { get; set; }

    [Required]
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
