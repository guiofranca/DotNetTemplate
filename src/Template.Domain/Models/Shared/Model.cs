namespace Template.Domain.Models.Shared;

public partial class Model
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Model() { }

    public Model(Guid id)
    {
        Id = id;
    }

    public Model(Guid id, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void Update() => UpdatedAt = DateTime.Now;
}