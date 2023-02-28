using Template.Core.Interfaces;

namespace Template.Application.Services;

public class ErrorNotificator : IErrorNotificator
{
    public Dictionary<string, string> Errors { get; } = new();

    public void AddError(string propertyName, string message) => Errors.Add(propertyName, message);

    public bool HasError() => Errors.Any();
}
