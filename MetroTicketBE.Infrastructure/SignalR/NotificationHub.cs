using Microsoft.AspNetCore.SignalR;

namespace MetroTicketBE.Domain.Entities
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
