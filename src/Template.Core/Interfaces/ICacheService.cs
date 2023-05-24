using Template.Core.Models.Components;

namespace Template.Core.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task<string?> GetStringAsync(string key);
    Task RemoveKey(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null) where T : class;
    Task SetStringAsync(string key, string value, TimeSpan? ttl = null);
    Task<T?> RememberModelAsync<T>(Guid id, Func<Guid, Task<T?>> action) where T : class, IModel;
    Task<T> RememberModelAsync<T>(T model, Func<T, Task<T>> action) where T : class, IModel;
}
