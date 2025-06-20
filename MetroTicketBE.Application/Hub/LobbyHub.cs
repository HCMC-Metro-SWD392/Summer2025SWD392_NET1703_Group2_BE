using System.Security.Claims;
using MetroTicketBE.Application.Stores;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MetroTicketBE.Application.Hub;

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

        if (ChatRoomStore.ChatRooms.TryGetValue(roomId, out var room) && room.Status == StaticRoomStatus.Open)
        {
            // Không cho phép tự tham gia phòng mình tạo
            if (room.CreatorUserId == joinerId) return;

            // Đánh dấu phòng đã đóng và gán người tham gia
            room.PartnerUserId = joinerId;
            room.Status = StaticRoomStatus.Closed;

            // Thông báo cho TẤT CẢ client trong sảnh chờ để xóa phòng này khỏi danh sách
            await Clients.All.SendAsync("RoomClosed", roomId);

            // Thông báo cho người tạo phòng rằng đã có người tham gia
            // và gửi thông tin phòng đã được cập nhật
            await Clients.User(room.CreatorUserId).SendAsync("PartnerJoined", room);
            
            // Thông báo cho chính người tham gia rằng họ đã vào phòng thành công
            await Clients.Caller.SendAsync("JoinSuccess", room);
        }
    }
}