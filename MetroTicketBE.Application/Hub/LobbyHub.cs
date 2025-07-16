using System.Security.Claims;
using MetroTicketBE.Application.Stores;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MetroTicketBE.Application.Hub;

[Authorize]
public class LobbyHub: Microsoft.AspNetCore.SignalR.Hub
{
    private string GetCurrentUserId() => Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    // Khi client mới kết nối, gửi cho họ danh sách các phòng đang mở
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("UpdateRoomList", ChatRoomStore.GetOpenRooms());
        await base.OnConnectedAsync();
    }
    
    // User tạo một phòng chat mới
    public async Task<string> CreateRoom(string roomName)
    {
        var creatorId = GetCurrentUserId();
        var newRoom = new ChatRoom
        {
            Name = roomName,
            CreatorUserId = creatorId,
        };

        ChatRoomStore.ChatRooms.TryAdd(newRoom.Id, newRoom);

        // Thông báo cho TẤT CẢ client trong sảnh chờ rằng có phòng mới
        await Clients.All.SendAsync("NewRoomCreated", newRoom);
        
        // Trả về ID của phòng mới cho người tạo để họ có thể tự động vào phòng
        return newRoom.Id;
    }
    
    // User tham gia một phòng chat đang mở
    public async Task JoinRoom(string roomId)
    {
        var joinerId = GetCurrentUserId();

        if (ChatRoomStore.ChatRooms.TryGetValue(roomId, out var room))
        {
            // Kiểm tra điều kiện thất bại và gửi phản hồi RÕ RÀNG
            if (room.CreatorUserId == joinerId)
            {
                // Thay vì "return;" -> Gửi lỗi về cho client
                await Clients.Caller.SendAsync("JoinRoomFailed", "Bạn không thể tham gia phòng do chính mình tạo.");
                return;
            }

            if (room.Status != StaticRoomStatus.Open)
            {
                await Clients.Caller.SendAsync("JoinRoomFailed", "Phòng đã đóng hoặc không còn tồn tại.");
                return;
            }

            // Nếu mọi thứ OK, tiến hành join phòng
            room.PartnerUserId = joinerId;
            room.Status = StaticRoomStatus.Closed;
            await Clients.All.SendAsync("RoomClosed", roomId);
            await Clients.User(room.CreatorUserId).SendAsync("PartnerJoined", room);
            await Clients.Caller.SendAsync("JoinSuccess", room);
        }
        else
        {
            await Clients.Caller.SendAsync("JoinRoomFailed", "Không tìm thấy phòng với ID này.");
        }
    }
}