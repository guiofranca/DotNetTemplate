using Template.Core.Interfaces;
using Template.Core.Models.Shared;

namespace Template.Core.Models;

public class User : Model
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required bool Verified { get; set; }
    public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();
}
