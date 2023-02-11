using Template.Domain.Interfaces;
using Template.Domain.Models.Shared;

namespace Template.Domain.Models;

public class StoredFile : Model, IFile
{
    public required long Size { get; set; }
    public required string Name { get; set; }
    public string? Owner { get; set; }
    public Guid? OwnerId { get; set; }
}
