using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System.Text.Json;
using Template.Core.Interfaces;
using Template.Core.Models.Shared;

namespace Template.Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        protected readonly IDatabase redis;
        protected readonly TimeSpan _defaultTtl = TimeSpan.FromMinutes(60);
        protected readonly ILogger<CacheService> _logger;
        public CacheService(IConfiguration configuration, ILogger<CacheService> logger)
        {
            _logger = logger;
            var connectionString = configuration.GetConnectionString("CacheServer");
            if (connectionString == null) throw new ArgumentNullException("appSettings.ConnectionStrings.CacheServer");
            var connection = ConnectionMultiplexer.Connect(connectionString);
            redis = connection.GetDatabase();

            var prefix = configuration["Cache:Prefix"];
            if (prefix != null) redis = redis.WithKeyPrefix($"{prefix}_");

            var textTtlInMinutes = configuration["Cache:DefaultTtlInMinutes"];
            if (textTtlInMinutes != null)
            {
                var ttl = double.Parse(textTtlInMinutes);
                _defaultTtl = TimeSpan.FromMinutes(ttl);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null) where T : class
        {
            string json = JsonSerializer.Serialize(value);
            await redis.StringSetAsync(key, json, GetTtl(ttl));
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var content = await redis.StringGetAsync(key);
            if (content.IsNull) return null;
            T? result = JsonSerializer.Deserialize<T?>(content!);
            return result;
        }

        public async Task SetStringAsync(string key, string value, TimeSpan? ttl = null) => await redis.StringSetAsync(key, value, GetTtl(ttl));

        public async Task<string?> GetStringAsync(string key) => await redis.StringGetAsync(key);

        public async Task RemoveKey(string key) => await redis.KeyDeleteAsync(key);

        public async Task<T?> RememberModelAsync<T>(Guid id, Func<Guid, Task<T?>> action) where T : class
        {
            var key = $"{typeof(T).Name}.{id}";
            var cached = await GetAsync<T>(key);
            _logger.LogDebug("Fetched from cache! {key}", key);
            if(cached != null) return cached;

            T? fetch = await action(id);
            if(fetch == null) return fetch;

            await SetAsync<T>(key, fetch);
            _logger.LogDebug("Cached! {key}", key);

            return fetch;
        }

        public async Task<T> RememberModelAsync<T>(T model, Func<T, Task<T>> action) where T : Model
        {
            var key = $"{typeof(T).Name}.{model.Id}";
            
            await action(model);

            await SetAsync<T>(key, model);
            _logger.LogDebug("Cached! {key}", key);

            return model;
        }

        private TimeSpan GetTtl(TimeSpan? ttl) => ttl ?? _defaultTtl;
    }
}
