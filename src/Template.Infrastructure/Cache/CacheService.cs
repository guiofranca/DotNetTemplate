using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System.Text.Json;
using Template.Domain.Interfaces;

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

            var prefix = configuration["Cache.Prefix"];
            if (prefix != null) redis.WithKeyPrefix($"{prefix}_");

            var textTtlInMinutes = configuration["Cache.DefaultTtlInMinutes"];
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
            T? result = JsonSerializer.Deserialize<T?>(content);
            return result;
        }

        public async Task SetStringAsync(string key, string value, TimeSpan? ttl = null) => await redis.StringSetAsync(key, value, GetTtl(ttl));

        public async Task<string?> GetStringAsync(string key) => await redis.StringGetAsync(key);

        public async Task RemoveKey(string key) => await redis.KeyDeleteAsync(key);

        private TimeSpan GetTtl(TimeSpan? ttl) => ttl ?? _defaultTtl;
    }
}
