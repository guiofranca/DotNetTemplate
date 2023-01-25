namespace Template.Domain.Models.Shared;

public partial class Model
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Model() {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
        UpdatedAt = CreatedAt;
    }

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