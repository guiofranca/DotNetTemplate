namespace Template.Application.DTO.StoredFile;

public class StoredFileModel
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required long Size { get; set; }
    public string? Owner { get; set; }
    public Guid? OwnerId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
