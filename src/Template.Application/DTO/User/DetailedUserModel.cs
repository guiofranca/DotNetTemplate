using Template.Application.DTO.Role;

namespace Template.Application.DTO.User;

public class DetailedUserModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool Verified { get; set; }
    public required IEnumerable<RoleModel> Roles { get; set; }
}
