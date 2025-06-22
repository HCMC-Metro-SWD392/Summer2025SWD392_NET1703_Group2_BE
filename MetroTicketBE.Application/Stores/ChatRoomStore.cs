using System.Collections.Concurrent;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Application.Stores;

public class ChatRoomStore
{
    public static ConcurrentDictionary<string, ChatRoom> ChatRooms { get; } = new();
    public static List<ChatRoom> GetOpenRooms()
    {
        return ChatRooms.Values.Where(r => r.Status == StaticRoomStatus.Open).ToList();
    }
}