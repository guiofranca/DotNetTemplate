using Template.Domain.Models.Shared;

namespace Template.Domain.Models;

public class User : Model
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required bool Verified { get; set; }
    public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();
}
