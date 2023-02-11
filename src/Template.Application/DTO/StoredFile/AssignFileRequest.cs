using System.ComponentModel.DataAnnotations;

namespace Template.Application.DTO.StoredFile;

public class AssignFileRequest
{
    [Required(ErrorMessage = "The field {0} is required")]
    public required string Owner { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    public required Guid OwnerId { get; set; }
}
