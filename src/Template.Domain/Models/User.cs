using Template.Domain.Models.Shared;

namespace Template.Domain.Models;

public class User : Model
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool Verified { get; set; }
    public IEnumerable<Role> Roles { get; set; } = new List<Role>();
}
