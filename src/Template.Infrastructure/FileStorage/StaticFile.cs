using Template.Core.Interfaces;

namespace Template.Infrastructure.FileStorage;

public class StaticFile : IFile
{
    public required string Name { get; set; }
}
