using MetroTicketBE.Application.IService;
using StackExchange.Redis;

namespace MetroTicketBE.Application.Service
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        }

        public Task<bool> DeleteStringAysnc(string key)
        {
            var ceche = _redis.GetDatabase();
            var result = ceche.KeyDeleteAsync(key);
            return result;
        }

        public async Task<bool> StoreKeyAsync(string key, string value, TimeSpan? expiry = null)
        {
            var cache = _redis.GetDatabase();
            var result = await cache.StringSetAsync(key, value, expiry);

            return result;
        }

        public async Task<string?> RetrieveString(string key)
        {
            var cache = _redis.GetDatabase();
            var result = await cache.StringGetAsync(key);
            return result;
        }
    }
}
