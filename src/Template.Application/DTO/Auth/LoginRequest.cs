using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Template.Application.DTO.Auth;

public record LoginRequest
{
    [Required(ErrorMessage = "The field {0} is required")]
    [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
    [DisplayName("Password")]
    public required string Password { get; set; }
}
