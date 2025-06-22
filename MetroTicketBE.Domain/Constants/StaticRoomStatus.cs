namespace MetroTicketBE.Domain.Constants;

public enum StaticRoomStatus
{
    Open,  // Đang chờ người khác tham gia
    Closed // Đã có 2 người, trở thành riêng tư
}