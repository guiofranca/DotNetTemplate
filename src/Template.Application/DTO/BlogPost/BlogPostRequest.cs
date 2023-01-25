using System.ComponentModel.DataAnnotations;

namespace Template.Application.DTO.BlogPost;

public class BlogPostRequest
{
    [Required]
    [StringLength(255, MinimumLength = 10)]
    public string Title { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Content { get; set; }
}
