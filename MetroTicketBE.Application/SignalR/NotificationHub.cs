using MetroTicketBE.Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MetroTicketBE.Domain.Entities
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IRedisService _redis;

        public NotificationHub(IRedisService redis)
        {
            _redis = redis;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var key = $"checkLogin:{userId}";
            await _redis.StoreKeyAsync(key, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            var key = $"checkLogin:{userId}";
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); 
                var value = await _redis.RetrieveString(key);
                if (value == Context.ConnectionId)
                {
                    await _redis.DeleteKeyAsync(key);
                }
            });
            await base.OnDisconnectedAsync(exception);
        }
    }
}
