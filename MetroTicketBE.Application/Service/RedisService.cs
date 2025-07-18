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

        public async Task<long> RemoveRangeByScoreAsync(string key, double start, double stop)
        {
            var cache = _redis.GetDatabase();
            return await cache.SortedSetRemoveRangeByScoreAsync(key, start, stop);
        }

        public async Task<long> SortedSetLengthAsync(string key)
        {
            var cache = _redis.GetDatabase();
            return await cache.SortedSetLengthAsync(key);
        }

        public async Task<SortedSetEntry[]> GetSortedSetDescByScoreAsync(string key, bool isAccending, int take)
        {
            var cache = _redis.GetDatabase();
            if (isAccending)
            {
                return await cache.SortedSetRangeByScoreWithScoresAsync(key, order: Order.Ascending, take: take);
            }
            else
            {
                return await cache.SortedSetRangeByScoreWithScoresAsync(key, order: Order.Descending, take: take);
            }
        }

        public async Task<bool> DeleteKeyAsync(string key)
        {
            var cache = _redis.GetDatabase();
            return await cache.KeyDeleteAsync(key);
        }

        public async Task<bool> AddToSortedSetAsync(string key, string id, double score)
        {
            var cache = _redis.GetDatabase();
            return await cache.SortedSetAddAsync(key, id, score);
        }

        public async Task<bool> ExpireKeyAsync(string key, TimeSpan expiry)
        {
            var cache = _redis.GetDatabase();
            return await cache.KeyExpireAsync(key, expiry);
        }
        public async Task<bool> AddToSetAsync(string key, string value)
        {
            var cache = _redis.GetDatabase();
            return await cache.SetAddAsync(key, value);
        }

        public async Task<bool> RemoveFromSetAsync(string key, string value)
        {
            var cache = _redis.GetDatabase();
            return await cache.SetRemoveAsync(key, value);
        }

        public async Task<long> GetSetCountAsync(string key)
        {
            var cache = _redis.GetDatabase();
            return await cache.SetLengthAsync(key);
        }
    }
}
