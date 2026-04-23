namespace projectApiAngular.Services
{
    public interface IRedisCacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan ttl);
        Task<bool> RemoveAsync(string key);
        Task<TimeSpan?> GetTimeToLiveAsync(string key);
    }
}
