using System.ComponentModel.DataAnnotations;

namespace Template.Application.DTO.BlogComment;

public class BlogCommentRequest
{
    [Required]
    public Guid BlogPostId { get; set; }
    public Guid? BlogCommentId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Content { get; set; }
}
