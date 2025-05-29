using MetroTicketBE.Domain.Constants;
using StackExchange.Redis;

namespace MetroTicketBE.WebAPI.Extentions
{
    public static class RedisServiceExtensions
    {
        public static WebApplicationBuilder AddRedisCache(this WebApplicationBuilder builder)
        {
            string connectionString = builder.Configuration.GetSection("Redis")[StaticConnectionString.REDIS_ConnectionString]
                                    ?? throw new ArgumentNullException("Redis connection string is not configured.");
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));
            return builder;
        }
    }
}
