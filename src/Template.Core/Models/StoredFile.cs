using Template.Core.Interfaces;
using Template.Core.Models.Components;

namespace Template.Core.Models;

public class StoredFile : IModel, IFile
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required long Size { get; set; }
    public required string Name { get; set; }
    public string? Owner { get; set; }
    public Guid? OwnerId { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
