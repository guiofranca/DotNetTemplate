using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Template.Application.DTO.BlogPost;

public class BlogPostRequest
{
    [Required(ErrorMessage = "The field {0} is required")]
    [StringLength(255, MinimumLength = 10, ErrorMessage = "The field {0} must be a string with a minimum length of {2} and a maximum length of {1}")]
    [DisplayName("Title")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "The field {0} must be a string with a minimum length of {2} and a maximum length of {1}")]
    [DisplayName("Content")]
    public required string Content { get; set; }
}
