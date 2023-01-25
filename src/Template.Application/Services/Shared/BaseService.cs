using Microsoft.Extensions.Logging;
using Template.Application.DTO.BlogPost;
using Template.Application.Result;
using Template.Domain.Interfaces;

namespace Template.Application.Services.Shared;

public abstract class BaseService<T> where T : class
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IErrorNotificator _errorNotificator;
    protected readonly ICacheService _cache;
    protected readonly ILogger _logger;

    protected BaseService(IUnitOfWork unitOfWork,
        IErrorNotificator errorNotificator,
        ICacheService cache,
        ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _errorNotificator = errorNotificator;
        _cache = cache;
        _logger = logger;
    }

    protected IServiceResult<T> OkResult(T result, string message = "")   => new ServiceResult<T>(result, ServiceResultStatus.Ok, message: message);
    protected IServiceResult<TR> OkResult<TR>(TR result, string message = "") where TR : class => new ServiceResult<TR>(result, ServiceResultStatus.Ok, message: message);
    protected IServiceResult<T> FoundResult(T result, string message = "")   => new ServiceResult<T>(result, ServiceResultStatus.Found, message: message);
    protected IServiceResult<TCached> FoundResult<TCached>(TCached result, string message = "") where TCached : class  => new ServiceResult<TCached>(result, ServiceResultStatus.Found, message: message);
    protected IServiceResult<IEnumerable<T>> FoundResult(IEnumerable<T> result, string message = "")   => new ServiceResult<IEnumerable<T>>(result, ServiceResultStatus.Found, message: message);
    protected IServiceResult<T> NotFoundResult(string message = "")          => new ServiceResult<T>(status: ServiceResultStatus.NotFound, errorMessage: message);
    protected IServiceResult<T> CreatedResult(T result, string message = "") => new ServiceResult<T>(result, ServiceResultStatus.Created, message: message);
    protected IServiceResult<T> UpdatedResult(T result, string message = "") => new ServiceResult<T>(result, ServiceResultStatus.Updated, message: message);
    protected IServiceResult<T> DeletedResult(T? result = null, string message = "") => new ServiceResult<T>(result, ServiceResultStatus.Deleted, message: message);
    protected IServiceResult<T> FailureResult(string errorMessage)
    {
        var result = new ServiceResult<T>(status: ServiceResultStatus.Error, message: errorMessage, errorMessage: errorMessage);
        return result;
    }
    protected IServiceResult<TR> FailureResult<TR>(string errorMessage) where TR : class
    {
        var result = new ServiceResult<TR>(status: ServiceResultStatus.Error);
        result.AddError(errorMessage);
        return result;
    }

    protected async Task<TCached?> GetFromCache<TCached>(string key) where TCached : class
    {
        var cached = await _cache.GetAsync<TCached>(key);
        if (cached == null) return null;

        _logger.LogDebug($"Cache hit on {key}!");
        return cached;
    }
}
