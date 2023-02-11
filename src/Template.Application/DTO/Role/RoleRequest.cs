namespace Template.Application.DTO.Role;

public class RoleRequest
{
    public required Guid RoleId { get; set; }
    public required Guid UserId { get; set; }
}
