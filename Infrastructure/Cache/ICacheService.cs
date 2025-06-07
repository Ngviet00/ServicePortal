namespace ServicePortal.Infrastructure.Cache
{
    public interface ICacheService
    {
        void Set(string key, object value, int expireMinutes = 5);
        T? Get<T>(string key);
        void Remove(string key);
        T GetOrCreate<T>(string key, Func<T> getData, int expireMinutes = 5);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getData, int expireMinutes = 5);
    }
}
