using Template.Core.Models.Components;

namespace Template.Core.Interfaces;

public interface ICacheService
{
    public Task<T?> GetAsync<T>(string key) where T : class;
    public Task<string?> GetStringAsync(string key);
    public Task RemoveKey(string key);
    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null) where T : class;
    public Task SetStringAsync(string key, string value, TimeSpan? ttl = null);
    public Task<T?> RememberModelAsync<T>(Guid id, Func<Guid, Task<T?>> action) where T : class, IModel;
    public Task<T> RememberModelAsync<T>(T model, Func<T, Task<T>> action) where T : class, IModel;
}
