namespace Template.Domain.Interfaces;

public interface IErrorNotificator
{
    Dictionary<string, string> Errors { get; }
    void AddError(string propertyName, string message);
    bool HasError();
}
