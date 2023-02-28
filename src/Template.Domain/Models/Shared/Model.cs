namespace Template.Core.Models.Shared;

public abstract class Model
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; } = DateTime.Now;

    public void Update() => UpdatedAt = DateTime.Now;
}