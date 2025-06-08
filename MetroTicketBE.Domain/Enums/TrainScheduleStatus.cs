
namespace MetroTicketBE.Domain.Enum
{
    // Trạng thái lịch trình tàu  
    public enum TrainScheduleStatus
    {
        Normal = 0,     // Bình thường  
        Faulted = 1,    // Gặp sự cố  
        Cancelled = 2,   // Bị hủy  
        OutOfService = 3, // Tàu quay về bến không đón khách 
    }
}
