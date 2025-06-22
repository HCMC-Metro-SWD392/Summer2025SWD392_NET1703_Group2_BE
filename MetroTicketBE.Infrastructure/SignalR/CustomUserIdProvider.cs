using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MetroTicketBE.Infrastructure.SignalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"[SignalR] Connected UserId: {userId}");
            return userId;
        }
    }
}
