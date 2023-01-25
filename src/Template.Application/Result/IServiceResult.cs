namespace Template.Application.Result;

public interface IServiceResult<T> where T : class
{
    ServiceResultStatus Status { get; }
    bool Success => !Errors.Any();
    string Message { get; }
    List<string> Errors { get; }
    void AddError(string message);
    T? Data { get; }
    bool IsOk => Status == ServiceResultStatus.Ok;
    bool IsError => Status == ServiceResultStatus.Error;
}
