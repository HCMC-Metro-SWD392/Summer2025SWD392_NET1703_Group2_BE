using MetroTicketBE.Application.IService;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.Service
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        }
        public async Task<bool> StoreKeyAsync(string key, string value, TimeSpan? expiry = null)
        {
            var cache = _redis.GetDatabase();
            var result = await cache.StringSetAsync(key, value, expiry);

            return result;
        }
    }
}
