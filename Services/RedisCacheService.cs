using System.Text.Json;
using StackExchange.Redis;

namespace projectApiAngular.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _multiplexer;

        public RedisCacheService(IConnectionMultiplexer multiplexer)
        {
            _multiplexer = multiplexer;
        }

        private IDatabase Db => _multiplexer.GetDatabase();

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await Db.StringGetAsync(key);
            if (!value.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString()!);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan ttl)
        {
            var payload = JsonSerializer.Serialize(value);
            return Db.StringSetAsync(key, payload, ttl);
        }

        public Task<bool> RemoveAsync(string key)
        {
            return Db.KeyDeleteAsync(key);
        }

        public Task<TimeSpan?> GetTimeToLiveAsync(string key)
        {
            return Db.KeyTimeToLiveAsync(key);
        }
    }
}
