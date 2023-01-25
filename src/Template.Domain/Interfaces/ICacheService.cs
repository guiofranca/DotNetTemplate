using System.Text.Json;

namespace Template.Domain.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task<string?> GetStringAsync(string key);
    Task RemoveKey(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null) where T : class;
    Task SetStringAsync(string key, string value, TimeSpan? ttl = null);
}
