namespace Template.Application.Result;

public class ServiceResult<T> : IServiceResult<T> where T : class
{
    public ServiceResultStatus Status { get; } = ServiceResultStatus.Ok;
    public string Message { get; } = string.Empty;
    public List<string> Errors { get; } = new();
    public void AddError(string message) => Errors.Add(message);

    public T? Data { get; } = null;

    public ServiceResult()
    {
    }

    public ServiceResult(T? data = null, ServiceResultStatus status = ServiceResultStatus.Found, string? message = null, string? errorMessage = null)
    {
        Data = data;
        Status = status;
        Message = message ?? string.Empty;
        if(errorMessage is not null) AddError(errorMessage);
    }

    public ServiceResult(T data, string message)
    {
        Data = data;
        Message = message;
    }
}
