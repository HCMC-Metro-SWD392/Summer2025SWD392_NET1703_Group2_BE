using Net.payOS.Types;

namespace MetroTicketBE.Domain.DTO.Payment
{
    public class DataWrapperDTO
    {
        public List<ItemData> ItemData { get; set; } = null!;
        public Guid TicketId { get; set; }
        public Guid TicketRouteId { get; set; }
    }
}