using MetroTicketBE.Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

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
            await _redis.StoreKeyAsync($"checkLogin:{userId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            await _redis.DeleteKeyAsync($"checkLogin:{userId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
