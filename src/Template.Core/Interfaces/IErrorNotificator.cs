namespace Template.Core.Interfaces;

public interface IErrorNotificator
{
    Dictionary<string, string> Errors { get; }
    void AddError(string propertyName, string message);
    bool HasError();
}
