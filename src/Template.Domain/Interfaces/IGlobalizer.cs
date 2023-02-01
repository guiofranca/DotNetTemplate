namespace Template.Domain.Interfaces;

public interface IGlobalizer
{
    string this[string key, params string[] args] { get; }
}
