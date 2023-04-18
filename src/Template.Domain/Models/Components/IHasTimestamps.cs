namespace Template.Core.Models.Components;

public interface IHasTimestamps
{
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}
