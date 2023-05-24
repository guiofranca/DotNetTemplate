using Template.Core.Models.Components;

namespace Template.Core.Models;

public class User : IModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required bool Verified { get; set; }
    public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
