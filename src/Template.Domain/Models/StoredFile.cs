using Template.Core.Interfaces;
using Template.Core.Models.Shared;

namespace Template.Core.Models;

public class StoredFile : Model, IFile
{
    public required long Size { get; set; }
    public required string Name { get; set; }
    public string? Owner { get; set; }
    public Guid? OwnerId { get; set; }
}
