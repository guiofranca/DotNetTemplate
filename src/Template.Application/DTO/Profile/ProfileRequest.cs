using System.ComponentModel.DataAnnotations;

namespace Template.Application.DTO.Profile;

public class ProfileRequest
{
    [Required(ErrorMessage = "The field {0} is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The field {0} must be a string with a minimum length of {2} and a maximum length of {1}")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
    public required string Email { get; set; }
}
