namespace Template.Core.Interfaces;

public interface IFileStorage
{
    void Save(string name, Stream stream);
    void Delete(string name);
}
