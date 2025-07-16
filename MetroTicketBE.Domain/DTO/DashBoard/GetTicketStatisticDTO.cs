using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.DTO.DashBoard
{
    public class GetTicketStatisticDTO
    {
        public long OrderCode { get; set; }
        public string UserFullName { get; set; } = null!;
        public List<string>? DetailTicket { get; set; }
        public int TotalPrice { get; set; }
        public string TimeOfPurchase { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
    }
}
