using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Template.Application.DTO.Auth;

public record RegisterRequest
{
    [Required(ErrorMessage = "The field {0} is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The field {0} must be a string with a minimum length of {2} and a maximum length of {1}")]
    [DisplayName("Name")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
    [DisplayName("Password")]
    public required string Password { get; set; }

    [Compare("Password", ErrorMessage = "The passwords do not match")]
    public required string PasswordConfirmation { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    public bool AcceptTerms { get; set; }
}

