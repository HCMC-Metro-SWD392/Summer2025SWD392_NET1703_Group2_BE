using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MetroTicketBE.Application.Service;

public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    // Add your message service/repository here if you want persistence
    // private readonly IMessageService _messageService;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User connected without valid identifier");
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            _logger.LogInformation($"User {userId} connected with connection {Context.ConnectionId}");
            
            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnConnectedAsync");
            Context.Abort();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation($"User {userId} disconnected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnDisconnectedAsync");
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string toUserId, string message, string messageId)
    {
        try
        {
            var fromUserId = Context.UserIdentifier;
        
            if (string.IsNullOrEmpty(toUserId) || string.IsNullOrEmpty(fromUserId) || string.IsNullOrEmpty(message))
            {
                await Clients.Caller.SendAsync("MessageError", messageId, "Invalid message parameters");
                return;
            }

            // Send to recipient ONLY (not to sender)
            await Clients.Group(toUserId).SendAsync("ReceiveMessage", fromUserId, message.Trim(), messageId);
        
            // Send confirmation back to sender (different event)
            await Clients.Caller.SendAsync("MessageSent", messageId);
        
        } catch (Exception ex)
        {
            await Clients.Caller.SendAsync("MessageError", messageId, "Failed to send message");
        }
    }

    // Optional: Add typing indicator
    public async Task StartTyping(string toUserId)
    {
        try
        {
            var fromUserId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(fromUserId) && !string.IsNullOrEmpty(toUserId))
            {
                await Clients.Group(toUserId).SendAsync("UserStartedTyping", fromUserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in StartTyping");
        }
    }

    public async Task StopTyping(string toUserId)
    {
        try
        {
            var fromUserId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(fromUserId) && !string.IsNullOrEmpty(toUserId))
            {
                await Clients.Group(toUserId).SendAsync("UserStoppedTyping", fromUserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in StopTyping");
        }
    }

    // Optional: Get chat history
    public async Task JoinChatRoom(string otherUserId)
    {
        try
        {
            var currentUserId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(otherUserId))
                return;

            // Optional: Load and send chat history
            // var messages = await _messageService.GetChatHistoryAsync(currentUserId, otherUserId);
            // await Clients.Caller.SendAsync("ChatHistory", messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining chat room");
        }
    }
}