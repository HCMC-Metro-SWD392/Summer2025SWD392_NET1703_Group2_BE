using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace MetroTicketBE.Infrastructure.SignalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        private readonly ILogger<CustomUserIdProvider> _logger;

        public CustomUserIdProvider(ILogger<CustomUserIdProvider> logger)
        {
            _logger = logger;
        }

        public string? GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("[SignalR] Connected UserId: {UserId}", userId);
            return userId;
        }
    }
}
