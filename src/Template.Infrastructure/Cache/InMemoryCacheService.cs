using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Template.Core.Interfaces;
using Template.Core.Models.Components;

namespace Template.Infrastructure.Cache;

public class InMemoryCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    protected readonly TimeSpan _defaultTtl = TimeSpan.FromMinutes(60);
    protected readonly ILogger<InMemoryCacheService> _logger;

    public InMemoryCacheService(IDistributedCache cache, ILogger<InMemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var fromCache = await _cache.GetAsync(key);
        if (fromCache is null)
            return null;
        var result = JsonSerializer.Deserialize<T?>(fromCache);
        return result;
    }

    public async Task<string?> GetStringAsync(string key) => await _cache.GetStringAsync(key);

    public async Task RemoveKey(string key) => await _cache.RemoveAsync(key);

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null) where T : class
    {
        var content = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, content, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl ?? _defaultTtl
        });
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? ttl = null)
    {
        await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl ?? _defaultTtl
        });
    }

    public async Task<T?> RememberModelAsync<T>(Guid id, Func<Guid, Task<T?>> action) where T : class, IModel
    {
        var key = $"{typeof(T).Name}.{id}";
        var cached = await GetAsync<T>(key);
        _logger.LogDebug("Fetched from cache! {key}", key);
        if (cached != null) return cached;

        T? fetch = await action(id);
        if (fetch == null) return fetch;

        await SetAsync(key, fetch);
        _logger.LogDebug("Cached! {key}", key);

        return fetch;
    }

    public async Task<T> RememberModelAsync<T>(T model, Func<T, Task<T>> action) where T : class, IModel
    {
        var key = $"{typeof(T).Name}.{model.Id}";

        await action(model);

        await SetAsync<T>(key, model);
        _logger.LogDebug("Cached! {key}", key);

        return model;
    }
}
