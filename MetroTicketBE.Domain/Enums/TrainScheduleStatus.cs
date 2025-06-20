
namespace MetroTicketBE.Domain.Enum
{
    // Trạng thái lịch trình tàu  
    public enum TrainScheduleStatus
    {
        Normal = 0,     // Bình thường
        Cancelled = 1,   // Bị hủy  
        OutOfService = 2, // Tàu quay về bến không đón khách 
    }
}
