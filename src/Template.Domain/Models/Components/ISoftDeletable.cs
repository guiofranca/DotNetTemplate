namespace Template.Core.Models.Components;

public interface ISoftDeletable
{
    public DateTime DeletedAt { get; set; }
}
