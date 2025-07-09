
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MetroTicketBE.Application.Hub;

[Authorize]
public class ChatRoomHub: Microsoft.AspNetCore.SignalR.Hub
{
    private string GetCurrentUserId() => Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

    // Khi vào trang chat, client sẽ gọi hàm này để tham gia group của SignalR
    public async Task JoinSpecificRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }
    
    public async Task SendRoomMessage(string roomId, string message)
    {
        var senderId = GetCurrentUserId();
        // Gửi tin nhắn đến tất cả client trong group có tên là roomId
        await Clients.Group(roomId).SendAsync("ReceiveRoomMessage", senderId, message);
    }
}